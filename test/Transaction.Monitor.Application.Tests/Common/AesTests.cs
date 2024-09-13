using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AElf.Client;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json;
using Transaction.Monitor.AddressConfigs;
using Transaction.Monitor.Common;
using Transaction.Monitor.TransactionHistorys;
using Xunit;
using Xunit.Abstractions;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Transaction.Monitor.Books;

public class AesTests


{
    private readonly ITestOutputHelper _output;

    public AesTests(ITestOutputHelper output)
    {
        _output = output;
    }

    // [Fact]
    // public void EncryptTest()
    // {
    //     {
    //         CreateAddressConfigDto dto = new CreateAddressConfigDto
    //         {
    //             AppId = "aaa",
    //             ToAddress = "xxxxx",
    //             CallbackUrl = "http......."
    //         };
    //         string json = JsonSerializer.Serialize(dto);
    //         _output.WriteLine("json\t" + json);
    //         // string key = AesHelper.GenerateKey();
    //         string key = "57Uz8uJDgxG8wrS1aVyNX/6ESO5KXBy6CHVaU+V3R4k=";
    //         string encoded = AesHelper.Encrypt(json, key);
    //         _output.WriteLine("encoded\t" + encoded);
    //     }
    //     {
    //         UpdateAddressConfigDto dto = new UpdateAddressConfigDto()
    //         {
    //             AppId = "aaa",
    //             ToAddress = "xxxxx",
    //             ReviseToAddress = "xxxxx1",
    //             ReviseCallbackUrl = "http.......1",
    //         };
    //         string json = JsonSerializer.Serialize(dto);
    //         _output.WriteLine("json\t" + json);
    //         // string key = AesHelper.GenerateKey();
    //         string key = "57Uz8uJDgxG8wrS1aVyNX/6ESO5KXBy6CHVaU+V3R4k=";
    //         string encoded = AesHelper.Encrypt(json, key);
    //         _output.WriteLine("encoded\t" + encoded);
    //     }
    //     {
    //         AddressConfigListDto dto = new AddressConfigListDto()
    //         {
    //             AppId = "aaa",
    //         };
    //         string json = JsonSerializer.Serialize(dto);
    //         _output.WriteLine("json\t" + json);
    //         // string key = AesHelper.GenerateKey();
    //         string key = "57Uz8uJDgxG8wrS1aVyNX/6ESO5KXBy6CHVaU+V3R4k=";
    //         string encoded = AesHelper.Encrypt(json, key);
    //         _output.WriteLine("encoded\t" + encoded);
    //     }
    // }

    [Fact]
    public void ThreadTest()
    {
        Task.Run(() => start());
        Task.Run(() => start());
        Task.Run(() => start());
        Task.Run(() => start());
        Thread.Sleep(11000);
        _output.WriteLine("======2");
    }

    public void start()
    {
        Thread.Sleep(10000);
        _output.WriteLine("======");
    }

    [Fact]
    public async void SQLTest()
    {
        try
        {
            GraphQLHttpClient _graphQLClient = new GraphQLHttpClient(
                "https://gcptest-indexer-api.aefinder.io/api/app/graphql/dailyholderapp", new NewtonsoftJsonSerializer());
            var graphQLResponse = await _graphQLClient.SendQueryAsync<TransferInfoResponse>(new GraphQLRequest()
            {
                Query = @"query($skipCount:Int!,$maxResultCount:Int!,$beginBlockHeight:Long!,$endBlockHeight:Long!,$chainId:String,$toList: [String])
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
                                    formatAmount
                                    token {symbol, collectionSymbol, type, decimals}
                                }
                            }
                        }",
                Variables = new
                {
                    skipCount = 0,
                    maxResultCount = 100,
                    beginBlockHeight = 239026577,
                    endBlockHeight = 239026613,
                    chainId = "AELF",
                    toList = new List<string> { "JRmBduh4nXWi1aXgdUsj5gJrzeZb2LxmrAbf7W99faZSvoAaE" },
                }
            });
            TransferInfoResponse resp = graphQLResponse.Data;
            _output.WriteLine(resp.transferInfoByBlock.TotalCount.ToString());
            _output.WriteLine(graphQLResponse.Data.ToString());
        }
        catch (Exception e)
        {
            _output.WriteLine(e.StackTrace);
            throw;
        }
    }
    [Fact]
    public async void SQLHeight()
    {
        try
        {
            var client = new AElfClient("https://aelf-test-node.aelf.io");
            await client.IsConnectedAsync();
            var h =  await client.GetBlockHeightAsync();
            _output.WriteLine(h.ToString());
        }
        catch (Exception e)
        {
            _output.WriteLine(e.StackTrace);
            throw;
        }
    }
    [Fact]
    public async void FromatAddress()
    {
        _output.WriteLine(AddressHelper.FormatAddress("ELF_dv5NqgJSFpJjWuoQLrUs5qo558TZbBVwQy5QsfQtbKrUdZCaf_AELF"));
        _output.WriteLine(AddressHelper.FormatAddress("ELF_dv5NqgJSFpJjWuoQLrUs5qo558TZbBVwQy5QsfQtbKrUdZCaf"));
        _output.WriteLine(AddressHelper.FormatFromAddress("dv5NqgJSFpJjWuoQLrUs5qo558TZbBVwQy5QsfQtbKrUdZCaf","dv5NqgJSFpJjWuoQLrUs5qo558TZbBVwQy5QsfQtbKrUdZCaf"));
        _output.WriteLine(AddressHelper.FormatFromAddress("dv5NqgJSFpJjWuoQLrUs5qo558TZbBVwQy5QsfQtbKrUdZCaf","ELF_dv5NqgJSFpJjWuoQLrUs5qo558TZbBVwQy5QsfQtbKrUdZCaf_AELF"));
    }
    [Fact]
    public async void FromatIndexer()
    {
        string resp = await HttpHelper.HttpGet("https://gcptest-indexer-api.aefinder.io/api/apps/sync-state/blockchainapp");
        IndexerResponse rootObject = JsonConvert.DeserializeObject<IndexerResponse>(resp);

        _output.WriteLine(resp);
    }
}