using System;
using System.Text.Json;
using System.Threading.Tasks;
using Transaction.Monitor.AddressConfigs;
using Transaction.Monitor.AddressConfigs.Provider;
using Transaction.Monitor.Common;
using Transaction.Monitor.GuardKeys;
using Transaction.Monitor.GuardKeys.Provider;
using Transaction.Monitor.TransactionHistorys;
using Transaction.Monitor.TransactionHistorys.Provider;
using Serilog;


public class CallbackService
{
    private ITransactionHistoryProvider _transactionHistoryProvider;
    private IAddressConfigProvider _addressConfigProvider;
    private IGuardKeyProvider _guardKeyProvider;

    public CallbackService(ITransactionHistoryProvider transactionHistoryProvider,
        IAddressConfigProvider addressConfigProvider,
        IGuardKeyProvider guardKeyProvider)
    {
        _transactionHistoryProvider = transactionHistoryProvider;
        _addressConfigProvider = addressConfigProvider;
        _guardKeyProvider = guardKeyProvider;
    }

    public async Task<bool> RetryHistory(TransactionHistoryDto history, AddressConfigDto config)
    {
        try
        {
            Log.Information($"RetryHistory history = {JsonSerializer.Serialize(history)} config = {config.AppId} {config.ToAddress} {config.CallbackUrl}");
            history.FromAddress = AddressHelper.FormatFromAddress(history.FromAddress, config.ToAddress);
            
            GuardKeyDto key = await _guardKeyProvider.MustGetGuardKey(config.AppId);
            TransactionHistoryCallDto callDto = ConvertHelper.TransactionHistoryToCall(history);
            string signature = AesHelper.Encrypt(JsonSerializer.Serialize(callDto), key.Key);

            ParamDto<TransactionHistoryCallDto> param = new ParamDto<TransactionHistoryCallDto>
            {
                Data = callDto, Signature = signature
            };
            bool result = await callback(param, config.CallbackUrl);

            history.Status = result ? 1 : 0;
            await _transactionHistoryProvider.UpdateStatus(history);
            return result;
        }
        catch (Exception e)
        {
            Log.Error(e, $"RetryHistory {history.Tx} has error");
            return false;
        }
    }

    async Task<bool> callback(ParamDto<TransactionHistoryCallDto> param, string url)
    {
        try
        {
            string resp = await HttpHelper.HttpPost(url, JsonSerializer.Serialize(param));
            JsonDocument json = JsonDocument.Parse(resp);
            bool result = json.RootElement.GetProperty("data").GetBoolean();
            return result;
        }
        catch (Exception e)
        {
            Log.Error(e, $"callback {url} has error",e.Message);
            return false;
        }
    }
}