using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Guids;
using Volo.Abp.Linq;
using Xunit;
using TourismTracking.Destinations;
using TourismTracking.Experiences;
using TourismTracking.Metrics;

namespace TourismTracking.Application.Tests.Metrics
{
    public class AdminMetricsAppService_Tests
    {
        private readonly IRepository<ApiMetric, Guid> _apiMetricRepository;
        private readonly IRepository<Destination, Guid> _destinationRepository;
        private readonly IRepository<Experience, Guid> _experienceRepository;
        private readonly IRepository<Review, Guid> _reviewRepository;
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly AdminMetricsAppService _service;

        public AdminMetricsAppService_Tests()
        {
            _apiMetricRepository = Substitute.For<IRepository<ApiMetric, Guid>>();
            _destinationRepository = Substitute.For<IRepository<Destination, Guid>>();
            _experienceRepository = Substitute.For<IRepository<Experience, Guid>>();
            _reviewRepository = Substitute.For<IRepository<Review, Guid>>();
            _userRepository = Substitute.For<IRepository<IdentityUser, Guid>>();

            var serviceProvider = Substitute.For<IServiceProvider>();
            var objectMapper = new SimpleTestObjectMapper();
            var guidGenerator = Substitute.For<IGuidGenerator>();
            guidGenerator.Create().Returns(Guid.NewGuid());
            
            var asyncExecuter = Substitute.For<IAsyncQueryableExecuter>();
            asyncExecuter.ToListAsync(Arg.Any<IQueryable<ApiMetric>>(), Arg.Any<CancellationToken>())
                .Returns(callInfo => Task.FromResult(callInfo.Arg<IQueryable<ApiMetric>>().ToList()));
            asyncExecuter.ToListAsync(Arg.Any<IQueryable<Review>>(), Arg.Any<CancellationToken>())
                .Returns(callInfo => Task.FromResult(callInfo.Arg<IQueryable<Review>>().ToList()));
            asyncExecuter.ToListAsync(Arg.Any<IQueryable<Destination>>(), Arg.Any<CancellationToken>())
                .Returns(callInfo => Task.FromResult(callInfo.Arg<IQueryable<Destination>>().ToList()));

            serviceProvider.GetService(typeof(IObjectMapper)).Returns(objectMapper);
            serviceProvider.GetService(typeof(IGuidGenerator)).Returns(guidGenerator);
            serviceProvider.GetService(typeof(IAsyncQueryableExecuter)).Returns(asyncExecuter);

            var lazyServiceProvider = new AbpLazyServiceProvider(serviceProvider);

            _service = new AdminMetricsAppService(
                _apiMetricRepository,
                _destinationRepository,
                _experienceRepository,
                _reviewRepository,
                _userRepository
            );

            _service.LazyServiceProvider = lazyServiceProvider;
        }

        [Fact]
        public async Task GetApiUsageSummary_Should_Calculate_Aggregates_Correctly()
        {
            var metrics = new List<ApiMetric>
            {
                new ApiMetric(Guid.NewGuid(), "OpenMeteo", "https://api.open-meteo.com/v1", true, 100, null),
                new ApiMetric(Guid.NewGuid(), "OpenMeteo", "https://api.open-meteo.com/v2", true, 200, null),
                new ApiMetric(Guid.NewGuid(), "OpenMeteo", "https://api.open-meteo.com/v3", false, 300, "Timeout"),
                new ApiMetric(Guid.NewGuid(), "Ticketmaster", "https://api.ticketmaster.com", true, 150, null)
            };

            _apiMetricRepository.GetQueryableAsync().Returns(Task.FromResult(metrics.AsQueryable()));

            var summary = await _service.GetApiUsageSummaryAsync();

            summary.ShouldNotBeNull();
            summary.Count.ShouldBe(2);

            var openMeteo = summary.FirstOrDefault(s => s.ApiName == "OpenMeteo");
            openMeteo.ShouldNotBeNull();
            openMeteo.TotalCalls.ShouldBe(3);
            openMeteo.SuccessCalls.ShouldBe(2);
            openMeteo.FailedCalls.ShouldBe(1);
            openMeteo.AverageResponseTimeMs.ShouldBe(200.0);
            openMeteo.SuccessRate.ShouldBe(66.7);

            var tm = summary.FirstOrDefault(s => s.ApiName == "Ticketmaster");
            tm.ShouldNotBeNull();
            tm.TotalCalls.ShouldBe(1);
            tm.SuccessCalls.ShouldBe(1);
            tm.SuccessRate.ShouldBe(100.0);
        }

        [Fact]
        public async Task ExportMetricsReport_Should_Generate_Base64_Csv()
        {
            var metrics = new List<ApiMetric>
            {
                new ApiMetric(Guid.NewGuid(), "OpenMeteo", "https://api.open-meteo.com/v1", true, 120, null)
            };

            _apiMetricRepository.GetQueryableAsync().Returns(Task.FromResult(metrics.AsQueryable()));

            var base64Csv = await _service.ExportMetricsReportAsync("csv");

            base64Csv.ShouldNotBeNullOrEmpty();
            var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64Csv));
            decoded.ShouldContain("Fecha,API,Endpoint,Exito,TiempoMs,Error");
            decoded.ShouldContain("OpenMeteo");
            decoded.ShouldContain("https://api.open-meteo.com/v1");
        }
    }
}
