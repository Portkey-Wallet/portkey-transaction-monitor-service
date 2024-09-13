using System.Collections.Generic;
using System.Text.RegularExpressions;
using Transaction.Monitor.AddressConfigs;

namespace Transaction.Monitor.Common;

public static class AddressHelper
{
    public static string FormatAddress(string address)
    {
        if (string.IsNullOrEmpty(address))
        {
            throw new ParamException("FormatAddress address is null or empty.");
        }

        if (!address.Contains("_"))
        {
            return address;
        }

        return address.Split("_")[1];
    }
    
    public static string FormatFromAddress(string fromAddress, string configAddress)
    {
        string pattern = @"_(.*?)_";
        return Regex.Replace(configAddress, pattern, $"_{fromAddress}_");
    }
    
    public static AddressConfigDto GetConfig(List<AddressConfigDto> configList, string toAddress)
    {
        return configList.Find(c => c.ToAddress.Contains(toAddress));
    }
}