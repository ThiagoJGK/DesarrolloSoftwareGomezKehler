using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using System.Net.Http;
using System.Net.Http.Json;
using System.Linq;

namespace TourismTracking.Destinations
{
    public class DestinationAppService : ApplicationService, IDestinationAppService
    {
        private readonly IRepository<Destination, Guid> _destinationRepository;
        private readonly IHttpClientFactory _httpClientFactory;

        public DestinationAppService(
            IRepository<Destination, Guid> destinationRepository, 
            IHttpClientFactory httpClientFactory)
        {
            _destinationRepository = destinationRepository;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<DestinationDto>> SearchExternalDestinationsAsync(string nameQuery, string countryCode = null)
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(nameQuery)}&count=10&language=en&format=json";
            
            var list = new List<DestinationDto>();

            try
            {
                var response = await client.GetFromJsonAsync<OpenMeteoResponse>(url);
                if (response?.Results != null)
                {
                    foreach(var r in response.Results)
                    {
                        if (string.IsNullOrEmpty(countryCode) || string.Equals(r.Country_Code, countryCode, StringComparison.OrdinalIgnoreCase))
                        {
                            list.Add(new DestinationDto
                            {
                                Name = r.Name,
                                Country = r.Country ?? "Unknown", // sometimes country might be missing for some features
                                Population = r.Population,
                                Latitude = r.Latitude,
                                Longitude = r.Longitude,
                                ImageUrl = null
                            });
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                // Return an empty list or handle error if API fails
            }
            
            return list;
        }

        private class OpenMeteoResponse
        {
            public List<OpenMeteoResult> Results { get; set; }
        }

        private class OpenMeteoResult
        {
            public string Name { get; set; }
            public string Country { get; set; }
            public string Country_Code { get; set; }
            public int Population { get; set; }
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
            await _destinationRepository.DeleteAsync(id);
        }
    }
}
