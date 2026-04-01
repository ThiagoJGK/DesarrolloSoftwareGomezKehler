using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TourismTracking.Data;
using Volo.Abp.DependencyInjection;

namespace TourismTracking.EntityFrameworkCore;

public class EntityFrameworkCoreTourismTrackingDbSchemaMigrator
    : ITourismTrackingDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreTourismTrackingDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the TourismTrackingDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<TourismTrackingDbContext>()
            .Database
            .MigrateAsync();
    }
}
