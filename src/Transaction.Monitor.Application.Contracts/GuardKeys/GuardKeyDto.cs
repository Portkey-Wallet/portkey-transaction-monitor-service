using System;
using Volo.Abp.Application.Dtos;

namespace Transaction.Monitor.GuardKeys;

public class GuardKeyDto : AuditedEntityDto<Guid>
{
    public string AppId { get; set; }

    public string Key { get; set; }
}
