using TourismTracking.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace TourismTracking.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class TourismTrackingController : AbpControllerBase
{
    protected TourismTrackingController()
    {
        LocalizationResource = typeof(TourismTrackingResource);
    }
}
