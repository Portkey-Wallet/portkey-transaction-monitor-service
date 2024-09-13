using System;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;

namespace Transaction.Monitor.MongoDB;

[DependsOn(
    typeof(MonitorApplicationTestModule),
    typeof(MonitorMongoDbModule)
)]
public class MonitorMongoDbTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = MonitorMongoDbFixture.GetRandomConnectionString();
        });
    }
}
