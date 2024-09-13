using System.Collections.Generic;

namespace Transaction.Monitor.TransactionHistorys;

public class IndexerResponse
{
    public VersionInfo CurrentVersion { get; set; }
    public VersionInfo PendingVersion { get; set; }
}

public class ChainInfo
{
    public string ChainId { get; set; }
    public string LongestChainBlockHash { get; set; }
    public long LongestChainHeight { get; set; }
    public string BestChainBlockHash { get; set; }
    public long BestChainHeight { get; set; }
    public string LastIrreversibleBlockHash { get; set; }
    public long LastIrreversibleBlockHeight { get; set; }
}

public class VersionInfo
{
    public string Version { get; set; }
    public List<ChainInfo> Items { get; set; }
}