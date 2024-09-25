using System.Collections.Generic;

namespace Transaction.Monitor.TransactionHistorys;

public class TransferInfoResponse
{
    public TransferInfo transferInfoByBlock { get; set; }
}

public class TransferInfo
{
    public int TotalCount { get; set; }
    public List<Item> Items { get; set; }
}

public class Item
{
    public string Id { get; set; }
    public Metadata Metadata { get; set; }
    public string TransactionId { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public string Method { get; set; }
    public decimal Amount { get; set; }
    public string Memo { get; set; }
    public string FormatAmount { get; set; }
    public Token Token { get; set; }
}

public class Metadata
{
    public string ChainId { get; set; }
    public Block Block { get; set; }
}

public class Block
{
    public string BlockHash { get; set; }
    public string BlockTime { get; set; }
    public long BlockHeight { get; set; }
}

public class Token
{
    public string Symbol { get; set; }
    public string CollectionSymbol { get; set; }
    public string Type { get; set; }
    public int Decimals { get; set; }
}