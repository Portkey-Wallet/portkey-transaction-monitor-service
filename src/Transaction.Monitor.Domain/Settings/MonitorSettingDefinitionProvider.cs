using Volo.Abp.Settings;

namespace Transaction.Monitor.Settings;

public class MonitorSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(MonitorSettings.MySetting1));
    }
}
