using Volo.Abp.Modularity;

namespace TourismTracking;

public abstract class TourismTrackingApplicationTestBase<TStartupModule> : TourismTrackingTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
