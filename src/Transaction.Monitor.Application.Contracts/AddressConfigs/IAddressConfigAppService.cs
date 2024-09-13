using System.Collections.Generic;
using System.Threading.Tasks;
using Transaction.Monitor.Common;

namespace Transaction.Monitor.AddressConfigs;

public interface IAddressConfigAppService
{
    Task<bool> CreateConfig(ParamDto<CreateAddressConfigDto> input);
    
    Task<AddressConfigDto> UpdateConfig(ParamDto<UpdateAddressConfigDto> input);
    
    Task<List<AddressConfigDto>> ConfigList(ParamDto<AddressConfigListDto> input);
}