using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;
using Volo.Abp.Domain.Repositories;
using TourismTracking.Destinations;
using TourismTracking.Experiences;
using TourismTracking.Notifications;
using TourismTracking.Metrics;
using Volo.Abp.Guids;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace TourismTracking.Workers
{
    public class DailyDestinationUpdateWorker : AsyncPeriodicBackgroundWorkerBase
    {
        public DailyDestinationUpdateWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory) 
            : base(timer, serviceScopeFactory)
        {
            Timer.Period = 86400000; // 24 horas (En prod, ajustar vía crontab o Quartz)
        }

        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            var logger = workerContext.ServiceProvider.GetRequiredService<ILogger<DailyDestinationUpdateWorker>>();
            var favRepo = workerContext.ServiceProvider.GetRequiredService<IRepository<FavoriteListItem, Guid>>();
            var destRepo = workerContext.ServiceProvider.GetRequiredService<IRepository<Destination, Guid>>();
            var notifRepo = workerContext.ServiceProvider.GetRequiredService<IRepository<Notification, Guid>>();
            var metricRepo = workerContext.ServiceProvider.GetRequiredService<IRepository<ApiMetric, Guid>>();
            var guidGenerator = workerContext.ServiceProvider.GetRequiredService<IGuidGenerator>();
            var httpClientFactory = workerContext.ServiceProvider.GetRequiredService<IHttpClientFactory>();

            logger.LogInformation("Iniciando revisión diaria de destinos favoritos para detectar cambios y eventos.");

            // Tomamos los destinos que estén en alguna lista de favoritos
            var activeFavs = await favRepo.GetListAsync();
            var destinationIds = activeFavs.Select(x => x.DestinationId).Distinct().ToList();

            if (!destinationIds.Any())
            {
                logger.LogInformation("No hay destinos favoritos registrados. Worker finalizado.");
                return;
            }

            var client = httpClientFactory.CreateClient();

            foreach (var destId in destinationIds)
            {
                var destination = await destRepo.FindAsync(destId);
                if (destination == null) continue;

                var startTime = DateTime.UtcNow;
                bool isSuccess = false;
                string errorMessage = null;
                string eventName = "Feria de Turismo y Cultura Local";
                string eventDate = DateTime.UtcNow.AddDays(7).ToString("dd-MM-yyyy");
                var url = $"https://app.ticketmaster.com/discovery/v2/events.json?apikey=DUMMY_KEY&city={Uri.EscapeDataString(destination.Name)}";

                try
                {
                    logger.LogInformation($"Consultando Ticketmaster API para eventos en: {destination.Name}");
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        isSuccess = true;
                    }
                    else
                    {
                        errorMessage = $"API falló con código {response.StatusCode}";
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    logger.LogError(ex, $"Error consultando eventos de Ticketmaster para {destination.Name}");
                }
                finally
                {
                    var duration = (int)(DateTime.UtcNow - startTime).TotalMilliseconds;
                    await metricRepo.InsertAsync(new ApiMetric(guidGenerator.Create(), "Ticketmaster", url, isSuccess, duration, errorMessage));
                }

                // Generar notificación a los usuarios afectados
                var affectedUsers = activeFavs.Where(f => f.DestinationId == destId).Select(f => f.UserId).ToList();
                foreach (var userId in affectedUsers)
                {
                    await notifRepo.InsertAsync(new Notification(
                        guidGenerator.Create(),
                        userId,
                        $"Novedades en {destination.Name}",
                        $"Se ha detectado un evento relevante: '{eventName}' el día {eventDate}."
                    ));
                    logger.LogInformation($"Notificación registrada para el usuario {userId} sobre {destination.Name}.");
                }
            }
        }
    }
}
