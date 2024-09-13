using Volo.Abp.Modularity;

namespace Transaction.Monitor;

/* Inherit from this class for your domain layer tests. */
public abstract class MonitorDomainTestBase<TStartupModule> : MonitorTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
