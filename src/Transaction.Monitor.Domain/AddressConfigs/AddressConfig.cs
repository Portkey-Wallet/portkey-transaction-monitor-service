using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Transaction.Monitor.AddressConfigs;

public class AddressConfig: AuditedAggregateRoot<Guid>
{
    public string AppId { get; set; }

    public string ToAddress { get; set; }

    public string CallbackUrl { get; set; }

}