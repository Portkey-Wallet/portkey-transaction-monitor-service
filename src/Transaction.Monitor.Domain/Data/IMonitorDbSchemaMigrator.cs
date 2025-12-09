using System.Threading.Tasks;

namespace Transaction.Monitor.Data;

public interface IMonitorDbSchemaMigrator
{
    Task MigrateAsync();
}
