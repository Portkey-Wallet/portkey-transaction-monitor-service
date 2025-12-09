using System;
using Volo.Abp.Application.Dtos;

namespace Transaction.Monitor.AddressConfigs;

public class AddressConfigDto
{
    public string AppId { get; set; }

    public string ToAddress { get; set; }

    public string CallbackUrl { get; set; }

}