using Volo.Abp.Modularity;

namespace Transaction.Monitor;

[DependsOn(
    typeof(MonitorDomainModule),
    typeof(MonitorTestBaseModule)
)]
public class MonitorDomainTestModule : AbpModule
{

}
