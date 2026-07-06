using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using System.Net.Http;
using System.Net.Http.Json;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using TourismTracking.Experiences;
using TourismTracking.Metrics;

namespace TourismTracking.Destinations
{
    [Authorize]
    public class DestinationAppService : ApplicationService, IDestinationAppService
    {
        private readonly IRepository<Destination, Guid> _destinationRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRepository<Review, Guid> _reviewRepository;
        private readonly IRepository<Experience, Guid> _experienceRepository;
        private readonly IRepository<FavoriteListItem, Guid> _favoritesRepository;
        private readonly IRepository<ApiMetric, Guid> _apiMetricRepository;

        public DestinationAppService(
            IRepository<Destination, Guid> destinationRepository, 
            IHttpClientFactory httpClientFactory,
            IRepository<Review, Guid> reviewRepository,
            IRepository<Experience, Guid> experienceRepository,
            IRepository<FavoriteListItem, Guid> favoritesRepository,
            IRepository<ApiMetric, Guid> apiMetricRepository)
        {
            _destinationRepository = destinationRepository;
            _httpClientFactory = httpClientFactory;
            _reviewRepository = reviewRepository;
            _experienceRepository = experienceRepository;
            _favoritesRepository = favoritesRepository;
            _apiMetricRepository = apiMetricRepository;
        }

        public async Task<List<DestinationDto>> SearchExternalDestinationsAsync(
            string nameQuery, 
            string countryCode = null, 
            string regionQuery = null, 
            int? minPopulation = null)
        {
            var client = _httpClientFactory.CreateClient();
            var queryParams = new List<string>
            {
                $"namePrefix={Uri.EscapeDataString(nameQuery)}",
                "limit=10"
            };

            if (!string.IsNullOrEmpty(countryCode))
            {
                queryParams.Add($"countryIds={Uri.EscapeDataString(countryCode)}");
            }

            if (minPopulation.HasValue)
            {
                queryParams.Add($"minPopulation={minPopulation.Value}");
            }

            var queryString = string.Join("&", queryParams);
            var url = $"http://geodb-free-service.wirefreethought.com/v1/geo/cities?{queryString}";
            
            var list = new List<DestinationDto>();
            var startTime = DateTime.UtcNow;
            bool isSuccess = false;
            string errorMessage = null;

            try
            {
                var response = await client.GetFromJsonAsync<GeoDBCitiesResponse>(url);
                isSuccess = true;
                if (response?.Data != null)
                {
                    foreach (var r in response.Data)
                    {
                        // Filtro de región en memoria local
                        if (!string.IsNullOrEmpty(regionQuery) && 
                            (r.Region == null || !r.Region.Contains(regionQuery, StringComparison.OrdinalIgnoreCase)))
                        {
                            continue;
                        }

                        list.Add(new DestinationDto
                        {
                            Name = r.City ?? r.Name,
                            Country = r.Country ?? "Unknown",
                            Population = r.Population,
                            Latitude = r.Latitude,
                            Longitude = r.Longitude,
                            ImageUrl = null
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                Logger.LogError(ex, "Error al consultar la API externa GeoDB Cities para la URL: {Url}", url);
            }
            finally
            {
                var duration = (int)(DateTime.UtcNow - startTime).TotalMilliseconds;
                await _apiMetricRepository.InsertAsync(new ApiMetric(GuidGenerator.Create(), "GeoDB", url, isSuccess, duration, errorMessage));
            }
            
            return list;
        }

        private class GeoDBCitiesResponse
        {
            public List<GeoDBCityResult> Data { get; set; }
        }

        private class GeoDBCityResult
        {
            public string Name { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
            public string CountryCode { get; set; }
            public string Region { get; set; }
            public long Population { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        public async Task<DestinationDto> SaveDestinationToInternalDbAsync(SaveDestinationInput input)
        {
            // Validar si ya existe
            var existing = await _destinationRepository.FirstOrDefaultAsync(d => d.Name == input.Name && d.Country == input.Country);
            if (existing != null)
            {
                // Si existe, actualizamos la data en vez de crear uno nuevo
                existing.UpdateDetails(input.Population, input.ImageUrl);
                await _destinationRepository.UpdateAsync(existing);
                return ObjectMapper.Map<Destination, DestinationDto>(existing);
            }

            var dest = new Destination(
                id: GuidGenerator.Create(),
                name: input.Name,
                country: input.Country,
                population: input.Population,
                lat: input.Latitude,
                lon: input.Longitude,
                imageUrl: input.ImageUrl,
                lastExternalUpdate: DateTime.UtcNow
            );

            await _destinationRepository.InsertAsync(dest);
            return ObjectMapper.Map<Destination, DestinationDto>(dest);
        }

        public async Task<List<DestinationDto>> GetSavedDestinationsAsync()
        {
            var items = await _destinationRepository.GetListAsync();
            return ObjectMapper.Map<List<Destination>, List<DestinationDto>>(items);
        }

        public async Task DeleteSavedDestinationAsync(Guid id)
        {
            // Borrar favoritos
            await _favoritesRepository.DeleteAsync(f => f.DestinationId == id);
            // Borrar experiencias
            await _experienceRepository.DeleteAsync(e => e.DestinationId == id);
            // Borrar reseñas
            await _reviewRepository.DeleteAsync(r => r.DestinationId == id);
            // Borrar el destino
            await _destinationRepository.DeleteAsync(id);
        }
    }
}
