using Volo.Abp.Modularity;

namespace TourismTracking;

/* Inherit from this class for your domain layer tests. */
public abstract class TourismTrackingDomainTestBase<TStartupModule> : TourismTrackingTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
