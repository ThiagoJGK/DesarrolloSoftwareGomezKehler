using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace TourismTracking.Ticketmaster
{
    /// <summary>
    /// Servicio cliente tipado para interactuar con la API Discovery v2 de TicketMaster.
    /// </summary>
    public interface ITicketmasterService : ITransientDependency
    {
        /// <summary>
        /// Consulta eventos culturales, deportivos y musicales para una ciudad determinada.
        /// </summary>
        /// <param name="cityName">Nombre de la ciudad del destino turístico.</param>
        /// <param name="cancellationToken">Token de cancelación opcional.</param>
        /// <returns>Lista de eventos encontrados mapeados a DTOs normalizados.</returns>
        Task<List<TicketmasterEventDto>> GetEventsByCityAsync(string cityName, CancellationToken cancellationToken = default);
    }
}
