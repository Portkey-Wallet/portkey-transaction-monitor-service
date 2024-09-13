using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Transaction.Monitor.TransactionHistorys;

public class TransactionHistory: AuditedAggregateRoot<Guid>
{
    public string Tx { get; set; }
    public long BlockNumber { get; set; }
    public string ToAddress { get; set; }
    public string FromAddress { get; set; }
    public string Symbol { get; set; }
    public string Amount { get; set; }
    public int Status { get; set; }
    public int RetryTimes { get; set; }
}