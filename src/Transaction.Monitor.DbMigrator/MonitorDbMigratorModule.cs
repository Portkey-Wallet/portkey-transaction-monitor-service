using Transaction.Monitor.MongoDB;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Transaction.Monitor.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(MonitorMongoDbModule),
    typeof(MonitorApplicationContractsModule)
)]
public class MonitorDbMigratorModule : AbpModule
{
}
