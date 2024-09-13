using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Transaction.Monitor.ScanHeights;

public class ScanHeight : AuditedAggregateRoot<Guid>
{
    public string ChainId { get; set; }
    public long Height { get; set; }
}