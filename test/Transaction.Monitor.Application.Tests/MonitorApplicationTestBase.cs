using Volo.Abp.Modularity;

namespace Transaction.Monitor;

public abstract class MonitorApplicationTestBase<TStartupModule> : MonitorTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
