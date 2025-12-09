using System;
using System.Threading.Tasks;
using Transaction.Monitor.Common;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Transaction.Monitor.GuardKeys.Provider;

public interface IGuardKeyProvider :
    ICrudAppService< 
        GuardKeyDto,
        Guid, 
        PagedAndSortedResultRequestDto, 
        GuardKeyDto> 
{
    Task<string> CreateGuardKey(CreateGuardKeyDto input);
    
    Task<GuardKeyDto> MustGetGuardKey(string appId);
}
public class GuardKeyProvider :
    CrudAppService<
        GuardKey,
        GuardKeyDto,
        Guid,
        PagedAndSortedResultRequestDto,
        GuardKeyDto>,
    IGuardKeyProvider
{
    public GuardKeyProvider(IRepository<GuardKey, Guid> repository)
        : base(repository)
    {
    }

    public async Task<string> CreateGuardKey(CreateGuardKeyDto input)
    {
        GuardKey guardKey = await Repository.FindAsync(p => p.AppId == input.AppId);
        if (null != guardKey)
        {
            return "The appId already exists.";
        }
        string Key = AesHelper.GenerateKey();
        GuardKey entity = new GuardKey { AppId = input.AppId, Key = Key };
        await Repository.InsertAsync(entity, autoSave: true);

        return Key;
    }

    public  async Task<GuardKeyDto> MustGetGuardKey(string appId)
    {
        GuardKey guardKey = await Repository.FindAsync(p => p.AppId == appId);
        if (null == guardKey)
        {
            throw new ParamException($"MustGetGuardKey AppId = {appId} Not Found");
        }
        return ConvertHelper.GuardKeyToDto(guardKey);
    }
}