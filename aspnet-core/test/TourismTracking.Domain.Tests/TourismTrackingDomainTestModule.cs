using Volo.Abp.Modularity;

namespace TourismTracking;

[DependsOn(
    typeof(TourismTrackingDomainModule),
    typeof(TourismTrackingTestBaseModule)
)]
public class TourismTrackingDomainTestModule : AbpModule
{

}
