using System;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Transaction.Monitor.Common;
using Transaction.Monitor.GuardKeys;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Transaction.Monitor.Controllers.GuardKeys;

[RemoteService]
[Area("app")]
[ControllerName("GuardKey")]
[Route("api/guardKey")]
public class GuardKeyController : AbpController
{
    private IGuardKeyAppService _guardKeyAppService;

    public GuardKeyController(IGuardKeyAppService guardKeyAppService)
    {
        _guardKeyAppService = guardKeyAppService;
    }

    [HttpPost("create")]
    public virtual async Task<ResultDto<string>> Create(CreateGuardKeyDto input)
    {
        string key = await _guardKeyAppService.CreateGuardKey(input);
        
        return ResultDto<string>.SuccessResult(key);
    }
}