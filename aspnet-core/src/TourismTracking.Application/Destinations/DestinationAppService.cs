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

        // Búsqueda Externa (Delegado a API GeoDB Cities o similar)
        public async Task<List<DestinationDto>> SearchExternalDestinationsAsync(string nameQuery, string countryCode = null)
        {
            var client = _httpClientFactory.CreateClient("GeoDbApi");
            
            // Simulación o llamada real a GeoDB. En p.ej RapidAPI:
            var url = $"/v1/geo/cities?namePrefix={nameQuery}";
            if (!string.IsNullOrEmpty(countryCode))
            {
                url += $"&countryIds={countryCode}";
            }
            
            // var response = await client.GetFromJsonAsync<GeoDbResponse>(url);
            // var mapped = MapResponseToDto(response);
            
            // Para fines de la estructura del Hito 2 devolvemos mock u omisión del mapeo profundo
            var list = new List<DestinationDto>(); 
            return await Task.FromResult(list);
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
