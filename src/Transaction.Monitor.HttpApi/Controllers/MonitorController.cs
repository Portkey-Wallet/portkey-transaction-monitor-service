using Transaction.Monitor.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Transaction.Monitor.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class MonitorController : AbpControllerBase
{
    protected MonitorController()
    {
        LocalizationResource = typeof(MonitorResource);
    }
}
