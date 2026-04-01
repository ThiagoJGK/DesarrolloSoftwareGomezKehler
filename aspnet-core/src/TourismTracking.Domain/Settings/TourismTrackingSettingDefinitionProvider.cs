using Volo.Abp.Settings;

namespace TourismTracking.Settings;

public class TourismTrackingSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(TourismTrackingSettings.MySetting1));
    }
}
