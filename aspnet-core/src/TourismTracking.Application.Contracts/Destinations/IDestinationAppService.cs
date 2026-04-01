using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using System.ComponentModel.DataAnnotations;

namespace TourismTracking.Destinations
{
    public interface IDestinationAppService : IApplicationService
    {
        // Métodos hacia la API externa
        Task<List<DestinationDto>> SearchExternalDestinationsAsync(string nameQuery, string countryCode = null);
        
        // Métodos hacia la Base de Datos Interna
        Task<DestinationDto> SaveDestinationToInternalDbAsync(SaveDestinationInput input);
        Task<List<DestinationDto>> GetSavedDestinationsAsync();
        Task DeleteSavedDestinationAsync(Guid id);
    }

    // --- DTOs ---
    public class DestinationDto
    {
        public Guid? Id { get; set; } // Puede ser nulo si viene de la API y aún no se guardó
        public string Name { get; set; }
        public string Country { get; set; }
        public long Population { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ImageUrl { get; set; }
        public DateTime LastExternalUpdate { get; set; }
    }

    public class SaveDestinationInput
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Country { get; set; }
        public long Population { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ImageUrl { get; set; }
    }
}
