using System.Threading.Tasks;

namespace Transaction.Monitor.GuardKeys;

public interface IGuardKeyAppService
{
    Task<string> CreateGuardKey(CreateGuardKeyDto input);
}