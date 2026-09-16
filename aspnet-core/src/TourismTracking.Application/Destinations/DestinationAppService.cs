using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using System.Net.Http;
using System.Net.Http.Json;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using TourismTracking.Experiences;
using TourismTracking.Metrics;

namespace TourismTracking.Destinations
{
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

        [AllowAnonymous]
        public async Task<List<DestinationDto>> SearchExternalDestinationsAsync(
            string nameQuery, 
            string countryCode = null, 
            string regionQuery = null, 
            int? minPopulation = null)
        {
            if (string.IsNullOrWhiteSpace(nameQuery))
            {
                return new List<DestinationDto>();
            }

            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(6);
            if (!client.DefaultRequestHeaders.Contains("User-Agent"))
            {
                client.DefaultRequestHeaders.Add("User-Agent", "TourismTrackingApp/1.0 (contact@tourismtracking.local)");
            }

            var encodedQuery = Uri.EscapeDataString(nameQuery.Trim());
            var url = $"https://geocoding-api.open-meteo.com/v1/search?name={encodedQuery}&count=10&language=es&format=json";

            var list = new List<DestinationDto>();
            var startTime = DateTime.UtcNow;
            var isSuccess = false;
            string errorMessage = null;

            try
            {
                var response = await client.GetFromJsonAsync<OpenMeteoResponse>(url);
                if (response?.Results != null && response.Results.Any())
                {
                    var filtered = response.Results.AsEnumerable();

                    if (!string.IsNullOrWhiteSpace(countryCode))
                    {
                        var code = countryCode.Trim().ToUpperInvariant();
                        filtered = filtered.Where(r => string.Equals(r.CountryCode, code, StringComparison.OrdinalIgnoreCase));
                    }

                    if (!string.IsNullOrWhiteSpace(regionQuery))
                    {
                        var reg = regionQuery.Trim();
                        filtered = filtered.Where(r => 
                            (!string.IsNullOrEmpty(r.Admin1) && r.Admin1.Contains(reg, StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrEmpty(r.Admin2) && r.Admin2.Contains(reg, StringComparison.OrdinalIgnoreCase))
                        );
                    }

                    if (minPopulation.HasValue && minPopulation.Value > 0)
                    {
                        filtered = filtered.Where(r => r.Population.HasValue && r.Population.Value >= minPopulation.Value);
                    }

                    foreach (var city in filtered)
                    {
                        var imageTask = FetchWikimediaImageAsync(client, city.Name, city.Country);
                        var resolvedImage = await imageTask;

                        list.Add(new DestinationDto
                        {
                            Name = city.Name,
                            Country = !string.IsNullOrEmpty(city.Admin1) ? $"{city.Admin1}, {city.Country}" : city.Country,
                            Population = (int)(city.Population ?? 0),
                            Latitude = city.Latitude,
                            Longitude = city.Longitude,
                            ImageUrl = resolvedImage
                        });
                    }
                }
                isSuccess = true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                Logger.LogError(ex, "Error buscando destinos en Open-Meteo Geocoding API.");
            }
            finally
            {
                var duration = (int)(DateTime.UtcNow - startTime).TotalMilliseconds;
                await _apiMetricRepository.InsertAsync(new ApiMetric(GuidGenerator.Create(), "OpenMeteo", url, isSuccess, duration, errorMessage));
            }
            
            return list;
        }

        private async Task<string> FetchWikimediaImageAsync(HttpClient client, string cityName, string countryName)
        {
            if (string.IsNullOrWhiteSpace(cityName)) return null;

            var imgUrl = await QueryWikiSummaryAsync(client, "es", cityName);
            if (!string.IsNullOrEmpty(imgUrl)) return imgUrl;

            if (!string.IsNullOrWhiteSpace(countryName))
            {
                imgUrl = await QueryWikiSummaryAsync(client, "es", $"{cityName},_{countryName}");
                if (!string.IsNullOrEmpty(imgUrl)) return imgUrl;
            }

            imgUrl = await QueryWikiSummaryAsync(client, "en", cityName);
            if (!string.IsNullOrEmpty(imgUrl)) return imgUrl;

            if (!string.IsNullOrWhiteSpace(countryName))
            {
                imgUrl = await QueryWikiSummaryAsync(client, "en", $"{cityName},_{countryName}");
                if (!string.IsNullOrEmpty(imgUrl)) return imgUrl;
            }

            return null;
        }

        private async Task<string> QueryWikiSummaryAsync(HttpClient client, string lang, string title)
        {
            try
            {
                var wikiUrl = $"https://{lang}.wikipedia.org/api/rest_v1/page/summary/{Uri.EscapeDataString(title)}";
                var wikiRes = await client.GetFromJsonAsync<WikiSummaryResponse>(wikiUrl);
                if (wikiRes?.OriginalImage?.Source != null)
                {
                    return wikiRes.OriginalImage.Source;
                }
                if (wikiRes?.Thumbnail?.Source != null)
                {
                    return wikiRes.Thumbnail.Source;
                }
            }
            catch
            {
                // Fallback silencioso
            }
            return null;
        }

        private class WikiSummaryResponse
        {
            [JsonPropertyName("thumbnail")]
            public WikiImage Thumbnail { get; set; }

            [JsonPropertyName("originalimage")]
            public WikiImage OriginalImage { get; set; }
        }

        private class WikiImage
        {
            [JsonPropertyName("source")]
            public string Source { get; set; }
        }

        private class OpenMeteoResponse
        {
            [JsonPropertyName("results")]
            public List<OpenMeteoCityResult> Results { get; set; }
        }

        private class OpenMeteoCityResult
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("country")]
            public string Country { get; set; }

            [JsonPropertyName("country_code")]
            public string CountryCode { get; set; }

            [JsonPropertyName("admin1")]
            public string Admin1 { get; set; }

            [JsonPropertyName("admin2")]
            public string Admin2 { get; set; }

            [JsonPropertyName("population")]
            public long? Population { get; set; }

            [JsonPropertyName("latitude")]
            public double Latitude { get; set; }

            [JsonPropertyName("longitude")]
            public double Longitude { get; set; }
        }

        [AllowAnonymous]
        public async Task<DestinationDto> SaveDestinationToInternalDbAsync(SaveDestinationInput input)
        {
            var existing = await _destinationRepository.FirstOrDefaultAsync(d => d.Name == input.Name && d.Country == input.Country);
            if (existing != null)
            {
                var imgToUpdate = !string.IsNullOrEmpty(input.ImageUrl) ? input.ImageUrl : existing.ImageUrl;
                existing.UpdateDetails(input.Population, imgToUpdate);
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

        [AllowAnonymous]
        public async Task<List<DestinationDto>> GetSavedDestinationsAsync()
        {
            var items = await _destinationRepository.GetListAsync();
            return ObjectMapper.Map<List<Destination>, List<DestinationDto>>(items);
        }

        [Authorize]
        public async Task DeleteSavedDestinationAsync(Guid id)
        {
            await _favoritesRepository.DeleteAsync(f => f.DestinationId == id);
            await _experienceRepository.DeleteAsync(e => e.DestinationId == id);
            await _reviewRepository.DeleteAsync(r => r.DestinationId == id);
            await _destinationRepository.DeleteAsync(id);
        }
    }
}
