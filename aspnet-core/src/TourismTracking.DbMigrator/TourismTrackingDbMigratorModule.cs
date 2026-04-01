using TourismTracking.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace TourismTracking.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(TourismTrackingEntityFrameworkCoreModule),
    typeof(TourismTrackingApplicationContractsModule)
    )]
public class TourismTrackingDbMigratorModule : AbpModule
{
}
