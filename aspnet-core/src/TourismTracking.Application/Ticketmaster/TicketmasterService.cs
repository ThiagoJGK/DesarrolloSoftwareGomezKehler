using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TourismTracking.Ticketmaster
{
    /// <summary>
    /// Implementación concreta de ITicketmasterService consumiendo la Discovery API v2 de TicketMaster.
    /// </summary>
    public class TicketmasterService : ITicketmasterService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TicketmasterService> _logger;

        public const string BaseEndpoint = "https://app.ticketmaster.com/discovery/v2/events.json";

        public TicketmasterService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<TicketmasterService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<List<TicketmasterEventDto>> GetEventsByCityAsync(string cityName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(cityName))
            {
                return new List<TicketmasterEventDto>();
            }

            var apiKey = _configuration["TicketMaster:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.LogWarning("TicketMaster:ApiKey no está configurada en appsettings ni User Secrets. Omitiendo llamada a API externa.");
                return new List<TicketmasterEventDto>();
            }

            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(10);

            var queryUrl = $"{BaseEndpoint}?apikey={Uri.EscapeDataString(apiKey)}&city={Uri.EscapeDataString(cityName)}&size=5&sort=date,asc";

            try
            {
                _logger.LogInformation("Consultando Ticketmaster Discovery API para ciudad: {City}", cityName);
                var response = await client.GetAsync(queryUrl, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Ticketmaster Discovery API retornó HTTP status {StatusCode} para ciudad {City}", response.StatusCode, cityName);
                    return new List<TicketmasterEventDto>();
                }

                var rawResponse = await response.Content.ReadFromJsonAsync<TicketmasterResponseRawDto>(cancellationToken: cancellationToken);

                var events = rawResponse?.Embedded?.Events;
                if (events == null || !events.Any())
                {
                    _logger.LogInformation("No se encontraron eventos en Ticketmaster para la ciudad {City}.", cityName);
                    return new List<TicketmasterEventDto>();
                }

                var result = new List<TicketmasterEventDto>();
                foreach (var ev in events)
                {
                    string formattedDate;
                    if (!string.IsNullOrWhiteSpace(ev.Dates?.Start?.LocalDate) && 
                        DateTime.TryParse(ev.Dates.Start.LocalDate, out var parsedDate))
                    {
                        formattedDate = parsedDate.ToString("dd-MM-yyyy");
                    }
                    else if (ev.Dates?.Start?.DateTime.HasValue == true)
                    {
                        formattedDate = ev.Dates.Start.DateTime.Value.ToString("dd-MM-yyyy");
                    }
                    else if (!string.IsNullOrWhiteSpace(ev.Dates?.Start?.LocalDate))
                    {
                        formattedDate = ev.Dates.Start.LocalDate;
                    }
                    else
                    {
                        formattedDate = DateTime.UtcNow.ToString("dd-MM-yyyy");
                    }

                    result.Add(new TicketmasterEventDto
                    {
                        Id = ev.Id ?? string.Empty,
                        Name = !string.IsNullOrWhiteSpace(ev.Name) ? ev.Name : "Evento Cultural",
                        FormattedDate = formattedDate,
                        Url = ev.Url,
                        Venue = ev.Embedded?.Venues?.FirstOrDefault()?.Name,
                        City = ev.Embedded?.Venues?.FirstOrDefault()?.City?.Name ?? cityName
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción al consultar Ticketmaster Discovery API para ciudad {City}", cityName);
                throw;
            }
        }
    }
}
