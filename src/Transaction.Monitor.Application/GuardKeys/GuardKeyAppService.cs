using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Transaction.Monitor.GuardKeys.Provider;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Transaction.Monitor.GuardKeys;

public class GuardKeyAppService :
    IGuardKeyAppService
{
    IGuardKeyProvider _guardKeyProvider;

    public GuardKeyAppService(IGuardKeyProvider guardKeyProvider)
    {
        _guardKeyProvider = guardKeyProvider;
    }

    public async Task<string> CreateGuardKey(CreateGuardKeyDto input)
    {
        string key = await _guardKeyProvider.CreateGuardKey(input);
        return key;
    }
    
    
}