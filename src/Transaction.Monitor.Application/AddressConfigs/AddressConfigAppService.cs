using System.Collections.Generic;
using System.Threading.Tasks;
using Transaction.Monitor.AddressConfigs.Provider;
using Transaction.Monitor.Common;
using Transaction.Monitor.GuardKeys;
using Transaction.Monitor.GuardKeys.Provider;

namespace Transaction.Monitor.AddressConfigs;

public class AddressConfigAppService :
    IAddressConfigAppService
{
    private IAddressConfigProvider _addressConfigProvider;
    private IGuardKeyProvider _guardKeyProvider;

    public AddressConfigAppService(IGuardKeyProvider guardKeyProvider, IAddressConfigProvider addressConfigProvider)
    {
        _guardKeyProvider = guardKeyProvider;
        _addressConfigProvider = addressConfigProvider;
    }

    public async Task<bool> CreateConfig(ParamDto<CreateAddressConfigDto> input)
    {
        GuardKeyDto guardKeyDto = await _guardKeyProvider.MustGetGuardKey(input.Data.AppId);
        if (!AesHelper.Check(input, guardKeyDto.Key))
        {
            throw new ParamException("CreateConfig signature is invalid");
        }

        await _addressConfigProvider.InsertConfig(input.Data);
        return true;
    }

    public async Task<AddressConfigDto> UpdateConfig(ParamDto<UpdateAddressConfigDto> input)
    {
        GuardKeyDto guardKeyDto = await _guardKeyProvider.MustGetGuardKey(input.Data.AppId);
        if (!AesHelper.Check(input, guardKeyDto.Key))
        {
            throw new ParamException("UpdateConfig signature is invalid");
        }

        AddressConfigDto addressConfigDto = await _addressConfigProvider.UpdateConfig(input.Data);
        return addressConfigDto;
    }

    public async Task<List<AddressConfigDto>> ConfigList(ParamDto<AddressConfigListDto> input)
    {
        GuardKeyDto guardKeyDto = await _guardKeyProvider.MustGetGuardKey(input.Data.AppId);
        if (!AesHelper.Check(input, guardKeyDto.Key))
        {
            throw new ParamException("UpdateConfig signature is invalid");
        }

        List<AddressConfigDto> list = await _addressConfigProvider.GetList(input.Data.AppId);
        return list;
    }
}