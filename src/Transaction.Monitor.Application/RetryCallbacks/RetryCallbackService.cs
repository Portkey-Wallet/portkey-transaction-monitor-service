using System.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Transaction.Monitor.AddressConfigs;
using Transaction.Monitor.AddressConfigs.Provider;
using Transaction.Monitor.Common;
using Transaction.Monitor.GuardKeys;
using Transaction.Monitor.GuardKeys.Provider;
using Transaction.Monitor.TransactionHistorys;
using Transaction.Monitor.TransactionHistorys.Provider;
using Serilog;


public class RetryCallbackService : IHostedService, IDisposable

{
    private Timer? _timer;
    private const int Period = 60000;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(Retry, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(Period));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Log.Information("RetryCallbackService StopAsync");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }


    private ITransactionHistoryProvider _transactionHistoryProvider;
    private IAddressConfigProvider _addressConfigProvider;
    private IGuardKeyProvider _guardKeyProvider;
    private CallbackService _callbackService;

    public RetryCallbackService(ITransactionHistoryProvider transactionHistoryProvider,
        IAddressConfigProvider addressConfigProvider,
        IGuardKeyProvider guardKeyProvider,
        CallbackService callbackService)
    {
        _transactionHistoryProvider = transactionHistoryProvider;
        _addressConfigProvider = addressConfigProvider;
        _guardKeyProvider = guardKeyProvider;
        _callbackService = callbackService;
    }

    async void Retry(object? state)
    {
        try
        {
            List<TransactionHistoryDto> historyList = await _transactionHistoryProvider.UndoneList();
            if (historyList.Count == 0)
            {
                Log.Information("Retry historyList size = 0");
                return;
            }

            List<AddressConfigDto> configList = await _addressConfigProvider.GetAll();
            for (var i = 0; i < historyList.Count; i++)
            {
                TransactionHistoryDto history = historyList[i];
                AddressConfigDto config = AddressHelper.GetConfig(configList, history.ToAddress);
                if (null == config)
                {
                    Log.Error($"Retry {history.Tx} ${history.ToAddress} not found config");
                    continue;
                }
                await _callbackService.RetryHistory(history, config);
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "Retry has error");
        }
    }

}