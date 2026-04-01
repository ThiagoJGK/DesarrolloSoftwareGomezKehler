using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace TourismTracking.Destinations
{
    /// <summary>
    /// Representa un destino turístico guardado en la base de datos local
    /// </summary>
    public class Destination : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public string Country { get; private set; }
        public long Population { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public string ImageUrl { get; private set; }
        public DateTime LastExternalUpdate { get; private set; }

        protected Destination()
        {
            // Constructor requerido por EF Core
        }

        public Destination(Guid id, string name, string country, long population, double lat, double lon, string imageUrl, DateTime lastExternalUpdate)
            : base(id)
        {
            SetName(name);
            SetCountry(country);
            Population = population;
            Latitude = lat;
            Longitude = lon;
            ImageUrl = imageUrl;
            LastExternalUpdate = lastExternalUpdate;
        }

        // Métodos de comportamiento DDD para modificar estado
        public void UpdateDetails(long population, string imageUrl)
        {
            Population = population;
            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                ImageUrl = imageUrl;
            }
            LastExternalUpdate = DateTime.UtcNow;
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("El nombre del destino no puede estar vacío.", nameof(name));
            }
            Name = name;
        }

        private void SetCountry(string country)
        {
            if (string.IsNullOrWhiteSpace(country))
            {
                throw new ArgumentException("El nombre del país no puede estar vacío.", nameof(country));
            }
            Country = country;
        }
    }
}
