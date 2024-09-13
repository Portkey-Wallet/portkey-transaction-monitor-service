using Transaction.Monitor.AddressConfigs;
using Transaction.Monitor.GuardKeys;
using Transaction.Monitor.TransactionHistorys;

namespace Transaction.Monitor.Common;

public static class ConvertHelper
{
    public static GuardKeyDto GuardKeyToDto(GuardKey input)
    {
        return new GuardKeyDto { AppId = input.AppId, Key = input.Key };
    }

    public static GuardKey GuardKeyFromDto(GuardKeyDto input)
    {
        return new GuardKey { AppId = input.AppId, Key = input.Key };
    }


    public static AddressConfigDto AddressConfigToDto(AddressConfig input)
    {
        return new AddressConfigDto
            { AppId = input.AppId, ToAddress = input.ToAddress, CallbackUrl = input.CallbackUrl };
    }

    public static AddressConfig AddressConfigFromDto(AddressConfigDto input)
    {
        return new AddressConfig { AppId = input.AppId, ToAddress = input.ToAddress, CallbackUrl = input.CallbackUrl };
    }

    public static AddressConfig AddressConfigFromCreateDto(CreateAddressConfigDto input)
    {
        return new AddressConfig { AppId = input.AppId, ToAddress = input.ToAddress, CallbackUrl = input.CallbackUrl };
    }

    public static TransactionHistory TransactionHistoryFromDto(TransactionHistoryDto input)
    {
        return new TransactionHistory
        {
            Tx = input.Tx,
            BlockNumber = input.BlockNumber,
            ToAddress = input.ToAddress,
            FromAddress = input.FromAddress,
            Symbol = input.Symbol,
            Amount = input.Amount,
            Status = input.Status,
            RetryTimes = input.RetryTimes
        };
    }

    public static TransactionHistoryDto TransactionHistoryToDto(TransactionHistory input)
    {
        return new TransactionHistoryDto
        {
            Tx = input.Tx,
            BlockNumber = input.BlockNumber,
            ToAddress = input.ToAddress,
            FromAddress = input.FromAddress,
            Symbol = input.Symbol,
            Amount = input.Amount,
            Status = input.Status,
            RetryTimes = input.RetryTimes
        };
    }

    public static TransactionHistoryCallDto TransactionHistoryToCall(TransactionHistoryDto input)
    {
        return new TransactionHistoryCallDto
        {
            Tx = input.Tx,
            ToAddress = input.ToAddress,
            FromAddress = input.FromAddress,
            Symbol = input.Symbol,
            Amount = input.Amount,
        };
    }
}