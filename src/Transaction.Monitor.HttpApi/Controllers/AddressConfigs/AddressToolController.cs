using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using Transaction.Monitor.AddressConfigs;
using Transaction.Monitor.Common;
using Transaction.Monitor.TransactionHistorys;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Transaction.Monitor.Controllers.AddressConfigs;

[RemoteService]
[Area("app")]
[ControllerName("AddressTool")]
[Route("api/tools")]
public class AddressToolController : AbpController
{
    public AddressToolController()
    {
    }

    [HttpGet("encrypt/add")]
    public virtual ResultDto<ParamDto<CreateAddressConfigDto>> Create(CreateAddressConfigDto input, string key)
    {
        string signature = AesHelper.Encrypt(JsonSerializer.Serialize(input), key);
        ParamDto<CreateAddressConfigDto> r = new ParamDto<CreateAddressConfigDto>
        {
            Data = input,
            Signature = signature
        };
        return ResultDto<ParamDto<CreateAddressConfigDto>>.SuccessResult(r);
    }

    [HttpGet("encrypt/update")]
    public virtual ResultDto<ParamDto<UpdateAddressConfigDto>> Update(UpdateAddressConfigDto input, string key)
    {
        string signature = AesHelper.Encrypt(JsonSerializer.Serialize(input), key);
        ParamDto<UpdateAddressConfigDto> r = new ParamDto<UpdateAddressConfigDto>
        {
            Data = input,
            Signature = signature
        };
        return ResultDto<ParamDto<UpdateAddressConfigDto>>.SuccessResult(r);
    }

    [HttpGet("encrypt/getList")]
    public virtual ResultDto<ParamDto<AddressConfigListDto>> GetList(AddressConfigListDto input, string key)
    {
        string signature = AesHelper.Encrypt(JsonSerializer.Serialize(input), key);
        ParamDto<AddressConfigListDto> r = new ParamDto<AddressConfigListDto>
        {
            Data = input,
            Signature = signature
        };
        return ResultDto<ParamDto<AddressConfigListDto>>.SuccessResult(r);
    }

    [HttpPost("callback")]
    public virtual ResultDto<bool> callback(ParamDto<TransactionHistoryCallDto> input)
    {
        Log.Information($"callback input = {JsonSerializer.Serialize(input)}");
        return ResultDto<bool>.SuccessResult(true);
    }
}