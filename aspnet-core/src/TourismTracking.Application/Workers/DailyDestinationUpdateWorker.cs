using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TourismTracking.Destinations;
using TourismTracking.Experiences;
using TourismTracking.Metrics;
using TourismTracking.Notifications;
using TourismTracking.Ticketmaster;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Threading;

namespace TourismTracking.Workers
{
    public class DailyDestinationUpdateWorker : AsyncPeriodicBackgroundWorkerBase
    {
        public DailyDestinationUpdateWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory) 
            : base(timer, serviceScopeFactory)
        {
            if (Timer != null)
            {
                Timer.Period = 86400000; // 24 horas (86,400,000 ms)
            }
        }

        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            await ProcessDestinationUpdatesAsync(workerContext.ServiceProvider);
        }

        public async Task ProcessDestinationUpdatesAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<DailyDestinationUpdateWorker>>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var favRepo = serviceProvider.GetRequiredService<IRepository<FavoriteListItem, Guid>>();
            var destRepo = serviceProvider.GetRequiredService<IRepository<Destination, Guid>>();
            var notifRepo = serviceProvider.GetRequiredService<IRepository<Notification, Guid>>();
            var metricRepo = serviceProvider.GetRequiredService<IRepository<ApiMetric, Guid>>();
            var guidGenerator = serviceProvider.GetRequiredService<IGuidGenerator>();
            var ticketmasterService = serviceProvider.GetRequiredService<ITicketmasterService>();

            logger.LogInformation("Iniciando revisión diaria de destinos favoritos para detectar cambios y eventos.");

            // R3: Si la API key no está configurada, loguear warning y salir limpiamente
            var apiKey = configuration["TicketMaster:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                logger.LogWarning("TicketMaster API key no está configurada en appsettings o User Secrets. Omitiendo consulta de eventos.");
                return;
            }

            // Recuperar destinos que están en listas de favoritos activas
            var activeFavs = await favRepo.GetListAsync(cancellationToken: cancellationToken);
            var destinationIds = activeFavs.Select(x => x.DestinationId).Distinct().ToList();

            if (!destinationIds.Any())
            {
                logger.LogInformation("No hay destinos favoritos registrados. Worker finalizado.");
                return;
            }

            foreach (var destId in destinationIds)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var destination = await destRepo.FindAsync(destId, cancellationToken: cancellationToken);
                if (destination == null)
                {
                    continue;
                }

                var startTime = DateTime.UtcNow;
                bool isSuccess = false;
                string? errorMessage = null;
                var sanitizedEndpoint = $"https://app.ticketmaster.com/discovery/v2/events.json?city={Uri.EscapeDataString(destination.Name)}";
                List<TicketmasterEventDto> events = new();

                try
                {
                    logger.LogInformation("Consultando Ticketmaster API para eventos en: {DestinationName}", destination.Name);
                    events = await ticketmasterService.GetEventsByCityAsync(destination.Name, cancellationToken);
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    logger.LogError(ex, "Error consultando eventos de Ticketmaster para {DestinationName}", destination.Name);
                }
                finally
                {
                    var duration = (int)(DateTime.UtcNow - startTime).TotalMilliseconds;
                    await metricRepo.InsertAsync(new ApiMetric(
                        guidGenerator.Create(),
                        "Ticketmaster",
                        sanitizedEndpoint,
                        isSuccess,
                        duration,
                        errorMessage
                    ), cancellationToken: cancellationToken);
                }

                // R4: Si la API no devuelve eventos para un destino, NO generar notificación
                if (events == null || !events.Any())
                {
                    logger.LogInformation("No se detectaron eventos en Ticketmaster para {DestinationName}. Omitiendo notificación.", destination.Name);
                    continue;
                }

                var firstEvent = events.First();
                var eventName = !string.IsNullOrWhiteSpace(firstEvent.Name) ? firstEvent.Name : "Evento Cultural";
                var eventDate = !string.IsNullOrWhiteSpace(firstEvent.FormattedDate) ? firstEvent.FormattedDate : DateTime.UtcNow.ToString("dd-MM-yyyy");

                // Generar notificación con datos reales a los usuarios que tienen este destino en favoritos
                var affectedUsers = activeFavs.Where(f => f.DestinationId == destId).Select(f => f.UserId).Distinct().ToList();
                foreach (var userId in affectedUsers)
                {
                    await notifRepo.InsertAsync(new Notification(
                        guidGenerator.Create(),
                        userId,
                        $"Novedades en {destination.Name}",
                        $"Se ha detectado un evento relevante: '{eventName}' el día {eventDate}."
                    ), cancellationToken: cancellationToken);

                    logger.LogInformation("Notificación registrada para el usuario {UserId} sobre {DestinationName}.", userId, destination.Name);
                }
            }
        }
    }
}
