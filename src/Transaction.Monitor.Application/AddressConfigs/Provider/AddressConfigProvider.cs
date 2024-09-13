using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transaction.Monitor.Common;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Transaction.Monitor.AddressConfigs.Provider;

public interface IAddressConfigProvider :
    ICrudAppService<
        AddressConfigDto,
        Guid,
        PagedAndSortedResultRequestDto,
        AddressConfigDto>
{
    Task<bool> InsertConfig(CreateAddressConfigDto dto);

    Task<AddressConfigDto> UpdateConfig(UpdateAddressConfigDto input);

    Task<List<AddressConfigDto>> GetList(string appId);

    Task<List<AddressConfigDto>> GetAll();
}

public class AddressConfigProvider :
    CrudAppService<
        AddressConfig,
        AddressConfigDto,
        Guid,
        PagedAndSortedResultRequestDto,
        AddressConfigDto>,
    IAddressConfigProvider
{
    public AddressConfigProvider(IRepository<AddressConfig, Guid> repository)
        : base(repository)
    {
    }

    public async Task<bool> InsertConfig(CreateAddressConfigDto input)
    {
        AddressConfig addressConfig = ConvertHelper.AddressConfigFromCreateDto(input);
        await Repository.InsertAsync(addressConfig, autoSave: true);

        return true;
    }

    public async Task<AddressConfigDto> UpdateConfig(UpdateAddressConfigDto input)
    {
        AddressConfig addressConfig =
            await Repository.FindAsync(p => p.AppId == input.AppId && p.ToAddress == input.ToAddress);
        if (null == addressConfig)
        {
            throw new ParamException($"MustGetConfig appId = {input.AppId} toAddress = {input.ToAddress} Not Found");
        }

        if (!string.IsNullOrWhiteSpace(input.ReviseToAddress))
        {
            addressConfig.ToAddress = input.ReviseToAddress;
        }

        if (!string.IsNullOrWhiteSpace(input.ReviseCallbackUrl))
        {
            addressConfig.CallbackUrl = input.ReviseCallbackUrl;
        }

        await Repository.UpdateAsync(addressConfig);

        return ConvertHelper.AddressConfigToDto(addressConfig);
    }

    public async Task<List<AddressConfigDto>> GetList(string appId)
    {
        var list = await Repository.GetListAsync(p => p.AppId == appId) ?? new List<AddressConfig>();
        return list.Select(ConvertHelper.AddressConfigToDto).ToList();
    }

    public async Task<List<AddressConfigDto>> GetAll()
    {
        var list = await Repository.GetListAsync() ?? new List<AddressConfig>();
        return list.Select(ConvertHelper.AddressConfigToDto).ToList();
    }
}