using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TourismTracking.Ticketmaster
{
    /// <summary>
    /// DTO de alto nivel normalizado para consumo del Worker y capas de presentación.
    /// </summary>
    public class TicketmasterEventDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string FormattedDate { get; set; } = string.Empty;
        public string? Url { get; set; }
        public string? Venue { get; set; }
        public string? City { get; set; }
    }

    /// <summary>
    /// Raíz del JSON retornado por Ticketmaster Discovery API v2.
    /// Si no hay eventos, _embedded se omite en el payload externo.
    /// </summary>
    public class TicketmasterResponseRawDto
    {
        [JsonPropertyName("_embedded")]
        public TicketmasterEmbeddedRawDto? Embedded { get; set; }

        [JsonPropertyName("page")]
        public TicketmasterPageInfoRawDto? Page { get; set; }
    }

    public class TicketmasterEmbeddedRawDto
    {
        [JsonPropertyName("events")]
        public List<TicketmasterEventRawDto>? Events { get; set; }
    }

    public class TicketmasterEventRawDto
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("dates")]
        public TicketmasterDatesRawDto? Dates { get; set; }

        [JsonPropertyName("_embedded")]
        public TicketmasterEventEmbeddedRawDto? Embedded { get; set; }
    }

    public class TicketmasterDatesRawDto
    {
        [JsonPropertyName("start")]
        public TicketmasterStartRawDto? Start { get; set; }

        [JsonPropertyName("timezone")]
        public string? Timezone { get; set; }
    }

    public class TicketmasterStartRawDto
    {
        [JsonPropertyName("localDate")]
        public string? LocalDate { get; set; }

        [JsonPropertyName("localTime")]
        public string? LocalTime { get; set; }

        [JsonPropertyName("dateTime")]
        public DateTime? DateTime { get; set; }
    }

    public class TicketmasterEventEmbeddedRawDto
    {
        [JsonPropertyName("venues")]
        public List<TicketmasterVenueRawDto>? Venues { get; set; }
    }

    public class TicketmasterVenueRawDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("city")]
        public TicketmasterCityRawDto? City { get; set; }

        [JsonPropertyName("country")]
        public TicketmasterCountryRawDto? Country { get; set; }
    }

    public class TicketmasterCityRawDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class TicketmasterCountryRawDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("countryCode")]
        public string? CountryCode { get; set; }
    }

    public class TicketmasterPageInfoRawDto
    {
        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("totalElements")]
        public int TotalElements { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("number")]
        public int Number { get; set; }
    }
}
