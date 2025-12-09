using System.Collections.Generic;

namespace Transaction.Monitor.Options;

public class ChainOptions
{
    public Dictionary<string, ChainInfo> ChainInfos { get; set; }
    public string GraphQLUrl { get; set; }
    public string IndexerUrl { get; set; }
    public bool LIB { get; set; }
}

public class ChainInfo
{
    public string ChainId { get; set; }
    public string BaseUrl { get; set; }
}