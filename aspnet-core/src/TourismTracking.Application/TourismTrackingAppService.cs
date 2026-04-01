using System;
using System.Collections.Generic;
using System.Text;
using TourismTracking.Localization;
using Volo.Abp.Application.Services;

namespace TourismTracking;

/* Inherit your application services from this class.
 */
public abstract class TourismTrackingAppService : ApplicationService
{
    protected TourismTrackingAppService()
    {
        LocalizationResource = typeof(TourismTrackingResource);
    }
}
