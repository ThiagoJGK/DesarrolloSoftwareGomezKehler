using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using TourismTracking.Destinations;
using TourismTracking.Experiences;
using System.Text;

namespace TourismTracking.Metrics
{
    [Authorize(Roles = "admin")]
    public class AdminMetricsAppService : ApplicationService, IAdminMetricsAppService
    {
        private readonly IRepository<ApiMetric, Guid> _apiMetricRepository;
        private readonly IRepository<Destination, Guid> _destinationRepository;
        private readonly IRepository<Experience, Guid> _experienceRepository;
        private readonly IRepository<Review, Guid> _reviewRepository;
        private readonly IRepository<IdentityUser, Guid> _userRepository;

        public AdminMetricsAppService(
            IRepository<ApiMetric, Guid> apiMetricRepository,
            IRepository<Destination, Guid> destinationRepository,
            IRepository<Experience, Guid> experienceRepository,
            IRepository<Review, Guid> reviewRepository,
            IRepository<IdentityUser, Guid> userRepository)
        {
            _apiMetricRepository = apiMetricRepository;
            _destinationRepository = destinationRepository;
            _experienceRepository = experienceRepository;
            _reviewRepository = reviewRepository;
            _userRepository = userRepository;
        }

        public async Task<List<ApiUsageSummaryDto>> GetApiUsageSummaryAsync()
        {
            var queryable = await _apiMetricRepository.GetQueryableAsync();
            var metrics = await AsyncExecuter.ToListAsync(queryable);

            var summary = metrics
                .GroupBy(m => m.ApiName)
                .Select(g => new ApiUsageSummaryDto
                {
                    ApiName = g.Key,
                    TotalCalls = g.Count(),
                    SuccessCalls = g.Count(x => x.IsSuccess),
                    FailedCalls = g.Count(x => !x.IsSuccess),
                    AverageResponseTimeMs = g.Any() ? Math.Round(g.Average(x => x.ResponseTimeMs), 1) : 0,
                    SuccessRate = g.Any() ? Math.Round((double)g.Count(x => x.IsSuccess) / g.Count() * 100, 1) : 0
                })
                .ToList();

            return summary;
        }

        public async Task<SystemStatisticsDto> GetSystemStatisticsAsync()
        {
            var totalUsers = await _userRepository.GetCountAsync();
            var totalExperiences = await _experienceRepository.GetCountAsync();
            var totalDestinations = await _destinationRepository.GetCountAsync();

            var reviewsQuery = await _reviewRepository.GetQueryableAsync();
            var destQuery = await _destinationRepository.GetQueryableAsync();

            // Destinos más reseñados (Top 5)
            var reviews = await AsyncExecuter.ToListAsync(reviewsQuery);
            var destinations = await AsyncExecuter.ToListAsync(destQuery);

            var mostReviewed = reviews
                .GroupBy(r => r.DestinationId)
                .Select(g =>
                {
                    var dest = destinations.FirstOrDefault(d => d.Id == g.Key);
                    return new MostReviewedDestinationDto
                    {
                        DestinationName = dest?.Name ?? "Desconocido",
                        Country = dest?.Country ?? "Desconocido",
                        ReviewsCount = g.Count(),
                        AverageRating = Math.Round(g.Average(r => r.Rating), 1)
                    };
                })
                .OrderByDescending(x => x.ReviewsCount)
                .Take(5)
                .ToList();

            return new SystemStatisticsDto
            {
                TotalUsers = (int)totalUsers,
                TotalExperiences = (int)totalExperiences,
                TotalSavedDestinations = (int)totalDestinations,
                MostReviewedDestinations = mostReviewed
            };
        }

        public async Task<List<ApiMetricDto>> GetRecentApiCallsAsync()
        {
            var queryable = await _apiMetricRepository.GetQueryableAsync();
            var metrics = await AsyncExecuter.ToListAsync(
                queryable.OrderByDescending(m => m.CreationTime).Take(50)
            );

            return ObjectMapper.Map<List<ApiMetric>, List<ApiMetricDto>>(metrics);
        }

        public async Task<string> ExportMetricsReportAsync(string format)
        {
            var queryable = await _apiMetricRepository.GetQueryableAsync();
            var metrics = await AsyncExecuter.ToListAsync(queryable.OrderByDescending(m => m.CreationTime));

            var csv = new StringBuilder();
            csv.AppendLine("Fecha,API,Endpoint,Exito,TiempoMs,Error");

            foreach (var m in metrics)
            {
                csv.AppendLine($"{m.CreationTime:yyyy-MM-dd HH:mm:ss},{m.ApiName},{m.Endpoint},{m.IsSuccess},{m.ResponseTimeMs},{m.ErrorMessage?.Replace(",", ";")}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return Convert.ToBase64String(bytes);
        }
    }
}
