using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace TourismTracking.Data;

/* This is used if database provider does't define
 * ITourismTrackingDbSchemaMigrator implementation.
 */
public class NullTourismTrackingDbSchemaMigrator : ITourismTrackingDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
