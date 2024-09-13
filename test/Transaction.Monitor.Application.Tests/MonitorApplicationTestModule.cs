using Volo.Abp.Modularity;

namespace Transaction.Monitor;

[DependsOn(
    typeof(MonitorApplicationModule),
    typeof(MonitorDomainTestModule)
)]
public class MonitorApplicationTestModule : AbpModule
{

}
