using Volo.Abp.Modularity;

namespace TourismTracking;

[DependsOn(
    typeof(TourismTrackingApplicationModule),
    typeof(TourismTrackingDomainTestModule)
)]
public class TourismTrackingApplicationTestModule : AbpModule
{

}
