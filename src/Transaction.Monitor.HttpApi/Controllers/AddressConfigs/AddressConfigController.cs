using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Transaction.Monitor.AddressConfigs;
using Transaction.Monitor.Common;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Transaction.Monitor.Controllers.AddressConfigs;

[RemoteService]
[Area("app")]
[ControllerName("AddressConfig")]
[Route("api/config")]
public class AddressConfigController : AbpController
{
    private IAddressConfigAppService _addressConfigAppService;


    public AddressConfigController(IAddressConfigAppService addressConfigAppService)
    {
        _addressConfigAppService = addressConfigAppService;
    }

    [HttpPost("add")]
    public virtual async Task<ResultDto<bool>> Create(ParamDto<CreateAddressConfigDto> input)
    {
        try
        {
            bool result = await _addressConfigAppService.CreateConfig(input);
            return ResultDto<bool>.SuccessResult(result);
        }
        catch (Exception e)
        {
            Log.Error(e, "addConfig exist error, input:{input}", input);
            return ResultDto<bool>.FailureResult(e.Message);
        }
    }

    [HttpPost("update")]
    public virtual async Task<ResultDto<AddressConfigDto>> Update(ParamDto<UpdateAddressConfigDto> input)
    {
        try
        {
            AddressConfigDto result = await _addressConfigAppService.UpdateConfig(input);
            return ResultDto<AddressConfigDto>.SuccessResult(result);
        }
        catch (Exception e)
        {
            Log.Error(e, "update exist error, input:{input}", input);
            return ResultDto<AddressConfigDto>.FailureResult(e.Message);
        }
    }

    [HttpPost("getList")]
    public virtual async Task<ResultDto<List<AddressConfigDto>>> GetList(ParamDto<AddressConfigListDto> input)
    {
        try
        {
            List<AddressConfigDto> result = await _addressConfigAppService.ConfigList(input);
            return ResultDto<List<AddressConfigDto>>.SuccessResult(result);
        }
        catch (Exception e)
        {
            Log.Error(e, "getList exist error, input:{input}", input);
            return ResultDto<List<AddressConfigDto>>.FailureResult(e.Message);
        }
    }
}