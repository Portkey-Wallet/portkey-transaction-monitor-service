using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Transaction.Monitor.Data;

/* This is used if database provider does't define
 * IMonitorDbSchemaMigrator implementation.
 */
public class NullMonitorDbSchemaMigrator : IMonitorDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
