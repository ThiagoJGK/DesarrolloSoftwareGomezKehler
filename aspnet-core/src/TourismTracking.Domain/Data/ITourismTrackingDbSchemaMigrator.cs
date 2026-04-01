using System.Threading.Tasks;

namespace TourismTracking.Data;

public interface ITourismTrackingDbSchemaMigrator
{
    Task MigrateAsync();
}
