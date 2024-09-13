using Transaction.Monitor.Localization;
using Volo.Abp.Application.Services;

namespace Transaction.Monitor;

/* Inherit your application services from this class.
 */
public abstract class MonitorAppService : ApplicationService
{
    protected MonitorAppService()
    {
        LocalizationResource = typeof(MonitorResource);
    }
}
