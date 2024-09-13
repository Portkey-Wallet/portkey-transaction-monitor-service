using Microsoft.Extensions.Localization;
using Transaction.Monitor.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Transaction.Monitor;

[Dependency(ReplaceServices = true)]
public class MonitorBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<MonitorResource> _localizer;

    public MonitorBrandingProvider(IStringLocalizer<MonitorResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
