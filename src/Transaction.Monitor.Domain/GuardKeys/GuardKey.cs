using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Transaction.Monitor.GuardKeys;

public class GuardKey : AuditedAggregateRoot<Guid>
{
    public string AppId { get; set; }

    public string Key { get; set; }
}