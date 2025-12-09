namespace Transaction.Monitor.Common;

public class RateLimitConstant
{
    public const string SendTxPolicy = "SendTx";
    public const int SendTxPermitLimit = 10;
    public const int SendTxWindow = 1;
    public const int SendTxSegmentsPerWindow = 1;
    public const int SendTxQueueLimit = 1000;
}