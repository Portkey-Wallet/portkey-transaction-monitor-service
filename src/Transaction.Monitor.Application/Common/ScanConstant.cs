using System.Collections.Generic;

namespace Transaction.Monitor.Common;

public class ScanConstant
{
    public static readonly Dictionary<string, decimal> LARGE_AMOUNT = new Dictionary<string, decimal>
    {
        { "ELF", 200m },
        { "USDT", 100m },
        { "ETH", 1m },
        { "BNB", 1m },
        { "DAI", 100m }
    };
}