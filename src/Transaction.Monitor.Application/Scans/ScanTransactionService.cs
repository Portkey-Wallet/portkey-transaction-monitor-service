using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AElf.Client;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using Transaction.Monitor.AddressConfigs;
using Transaction.Monitor.AddressConfigs.Provider;
using Transaction.Monitor.Common;
using Transaction.Monitor.Options;
using Transaction.Monitor.ScanHeights.Provider;
using Transaction.Monitor.TransactionHistorys;
using Transaction.Monitor.TransactionHistorys.Provider;
using ChainInfo = Transaction.Monitor.TransactionHistorys.ChainInfo;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Transaction.Monitor.Scans;

public interface IScanTransactionService
{
}

public class ScanTransactionService
    : IHostedService, IDisposable

{
    private readonly ChainOptions _chainOptions;
    private IScanHeightProvider _scanHeightProvider;
    private ITransactionHistoryProvider _transactionHistoryProvider;
    private CallbackService _callbackService;
    private IAddressConfigProvider _addressConfigProvider;
    private readonly GraphQLHttpClient _graphQLClient;

    public ScanTransactionService(IScanHeightProvider scanHeightProvider,
        ITransactionHistoryProvider transactionHistoryProvider,
        CallbackService callbackService,
        IAddressConfigProvider addressConfigProvider,
        IOptions<ChainOptions> chainOptions)
    {
        _scanHeightProvider = scanHeightProvider;
        _transactionHistoryProvider = transactionHistoryProvider;
        _callbackService = callbackService;
        _addressConfigProvider = addressConfigProvider;
        _chainOptions = chainOptions.Value;
        _graphQLClient = new GraphQLHttpClient(_chainOptions.GraphQLUrl, new NewtonsoftJsonSerializer());
    }


    private List<Timer> _timers;
    private const int Period = 30000;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _timers = new List<Timer>();
            int taskInterval = 12345;
            foreach (var chainInfo in _chainOptions.ChainInfos)
            {
                Timer timer = new Timer(Scan, new { chainId = chainInfo.Value.ChainId, baseUrl = chainInfo.Value.BaseUrl },
                    TimeSpan.FromMilliseconds(taskInterval),
                    TimeSpan.FromMilliseconds(Period));
                _timers.Add(timer);
                taskInterval += taskInterval;
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "StartAsync has error");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Log.Information("ScanTransactionService StopAsync");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timers.ForEach(x => x.Dispose());
    }


    async void Scan(object state)
    {
        try
        {
            var scanParams = (dynamic)state;
            string chainId = scanParams.chainId;
            string baseUrl = scanParams.baseUrl;

            long beginHeight = await _scanHeightProvider.GetHeight(chainId);
            ChainInfo chainInfo = await getLatestHeight(chainId);
            if (null == chainInfo)
            {
                Log.Error($"Scan ${chainId} chainInfo is null");
                return;
            }

            Log.Information($"Scan ${chainId} {beginHeight} {JsonSerializer.Serialize(chainInfo)}");

            if (beginHeight == 0)
            {
                await _scanHeightProvider.UpdateHeight(chainId, chainInfo.BestChainHeight);
                return;
            }

            if (beginHeight == chainInfo.BestChainHeight)
            {
                return;
            }

            List<AddressConfigDto> configList = await _addressConfigProvider.GetAll();
            if (configList?.Count == 0)
            {
                Log.Information($"Scan ${chainId} {beginHeight} {chainInfo.BestChainHeight} config list is null");
                return;
            }


            List<string> addressList = configList.Select(p => AddressHelper.FormatAddress(p.ToAddress)).ToList();
            while (true)
            {
                List<TransactionHistoryDto> historyList = await getHistoryList(chainId, beginHeight, chainInfo.BestChainHeight, addressList);
                if (historyList.Count == 0)
                {
                    return;
                }

                List<TransactionHistoryDto> filteredList = filterLargeAmount(historyList, chainInfo);
                beginHeight = handleHistoryList(chainId, filteredList, configList);
            }
        }
        catch (Exception e)
        {
            Log.Error(e, $"Scan has error");
        }
    }

    private long handleHistoryList(string chainId, List<TransactionHistoryDto> historyList, List<AddressConfigDto> configList)
    {
        // batch insert
        _transactionHistoryProvider.BatchInsert(historyList);
        long updateHeight = 1 + historyList.Max(h => h.BlockNumber);
        _scanHeightProvider.UpdateHeight(chainId, updateHeight);
        for (var i = 0; i < historyList.Count; i++)
        {
            TransactionHistoryDto history = historyList[i];
            AddressConfigDto config = AddressHelper.GetConfig(configList, history.ToAddress);
            if (null == config)
            {
                Log.Error($"handleHistoryList {history.Tx} ${history.ToAddress} not found config");
                continue;
            }

            Task.Run(() => _callbackService.RetryHistory(history, config));
        }

        return updateHeight;
    }

    private async Task<List<TransactionHistoryDto>> getHistoryList(string chainId, long from, long to, List<string> addressList)
    {
        if (from >= to)
        {
            Log.Error($"EndBlockHeight should be higher than StartBlockHeight {chainId} {from} to {to}");
            return new List<TransactionHistoryDto>();
        }

        var graphQLResponse = await _graphQLClient.SendQueryAsync<TransferInfoResponse>(new GraphQLRequest()
        {
            Query =
                @"query($skipCount:Int!,$maxResultCount:Int!,$beginBlockHeight:Long!,$endBlockHeight:Long!,$chainId:String,$toList: [String])
                        {transferInfoByBlock(input:{skipCount:$skipCount,maxResultCount:$maxResultCount,beginBlockHeight:$beginBlockHeight,endBlockHeight:$endBlockHeight,chainId:$chainId,toList:$toList})
                            {
                                totalCount,
                                items{
                                    id,
                                    metadata{chainId,block{blockHash,blockTime,blockHeight}},
                                    transactionId,
                                    from,
                                    to,
                                    method,
                                    amount,
                                    memo,
                                    formatAmount
                                    token {symbol, collectionSymbol, type, decimals}
                                }
                            }
                        }",
            Variables = new
            {
                skipCount = 0,
                maxResultCount = 1000,
                beginBlockHeight = from,
                endBlockHeight = to,
                chainId = chainId,
                toList = addressList,
            }
        });
        TransferInfoResponse resp = graphQLResponse.Data;
        List<TransactionHistoryDto> historyList = getFromGQL(resp);
        Log.Information($"Scan getHistoryList {chainId} {from} to {to} transaction size = {historyList.Count}");

        return historyList;
    }

    private List<TransactionHistoryDto> getFromGQL(TransferInfoResponse resp)
    {
        List<TransactionHistoryDto> dtoList = new List<TransactionHistoryDto>();
        if (null == resp || null == resp.transferInfoByBlock || 0 == resp.transferInfoByBlock.TotalCount)
        {
            return dtoList;
        }

        foreach (var item in resp.transferInfoByBlock.Items)
        {
            Log.Information($"Scan getHistoryList getFromGQL {item.TransactionId}");

            TransactionHistoryDto dto = new TransactionHistoryDto();
            dtoList.Add(dto);
            dto.Tx = item.TransactionId;
            dto.BlockNumber = item.Metadata.Block.BlockHeight;
            dto.FromAddress = item.From;
            dto.ToAddress = item.To;
            dto.Symbol = item.Token.Symbol;
            dto.Amount = item.FormatAmount;
            dto.Memo = item.Memo;
            dto.Status = 0;
            dto.RetryTimes = 0;
        }

        return dtoList;
    }

    private async Task<ChainInfo> getLatestHeight(string chainId)
    {
        string resp = await HttpHelper.HttpGet(_chainOptions.IndexerUrl);
        IndexerResponse indexerResponse = JsonConvert.DeserializeObject<IndexerResponse>(resp);
        foreach (var info in indexerResponse.CurrentVersion.Items)
        {
            if (string.Equals(info.ChainId, chainId, StringComparison.OrdinalIgnoreCase))
            {
                return info;
            }
        }

        return null;
    }

    private List<TransactionHistoryDto> filterLargeAmount(List<TransactionHistoryDto> historyList, ChainInfo chainInfo)
    {
        var filterBN = historyList.Where(p =>
        {
            return ScanConstant.LARGE_AMOUNT.ContainsKey(p.Symbol)
                   && ScanConstant.LARGE_AMOUNT[p.Symbol] < decimal.Parse(p.Amount);
        }).ToList();
        if (filterBN.Count == 0)
        {
            return historyList;
        }

        return historyList.Where(p => p.BlockNumber <= chainInfo.LastIrreversibleBlockHeight).ToList();
    }
}