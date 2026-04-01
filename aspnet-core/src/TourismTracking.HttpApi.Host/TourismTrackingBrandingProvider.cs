using Microsoft.Extensions.Localization;
using TourismTracking.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace TourismTracking;

[Dependency(ReplaceServices = true)]
public class TourismTrackingBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<TourismTrackingResource> _localizer;

    public TourismTrackingBrandingProvider(IStringLocalizer<TourismTrackingResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
