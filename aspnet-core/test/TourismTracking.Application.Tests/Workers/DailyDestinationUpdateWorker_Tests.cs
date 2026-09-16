using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Threading;
using Xunit;
using TourismTracking.Destinations;
using TourismTracking.Experiences;
using TourismTracking.Metrics;
using TourismTracking.Notifications;
using TourismTracking.Ticketmaster;
using TourismTracking.Workers;

namespace TourismTracking.Application.Tests.Workers
{
    public class DailyDestinationUpdateWorker_Tests
    {
        private readonly IRepository<FavoriteListItem, Guid> _favRepo;
        private readonly IRepository<Destination, Guid> _destRepo;
        private readonly IRepository<Notification, Guid> _notifRepo;
        private readonly IRepository<ApiMetric, Guid> _metricRepo;
        private readonly IGuidGenerator _guidGenerator;
        private readonly ITicketmasterService _ticketmasterService;
        private readonly ILogger<DailyDestinationUpdateWorker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly DailyDestinationUpdateWorker _worker;

        public DailyDestinationUpdateWorker_Tests()
        {
            _favRepo = Substitute.For<IRepository<FavoriteListItem, Guid>>();
            _destRepo = Substitute.For<IRepository<Destination, Guid>>();
            _notifRepo = Substitute.For<IRepository<Notification, Guid>>();
            _metricRepo = Substitute.For<IRepository<ApiMetric, Guid>>();
            _guidGenerator = Substitute.For<IGuidGenerator>();
            _ticketmasterService = Substitute.For<ITicketmasterService>();
            _logger = Substitute.For<ILogger<DailyDestinationUpdateWorker>>();
            _configuration = Substitute.For<IConfiguration>();

            _configuration["TicketMaster:ApiKey"].Returns("TEST_API_KEY_VALID_123");
            _guidGenerator.Create().Returns(_ => Guid.NewGuid());

            _serviceProvider = Substitute.For<IServiceProvider>();
            _serviceProvider.GetService(typeof(IRepository<FavoriteListItem, Guid>)).Returns(_favRepo);
            _serviceProvider.GetService(typeof(IRepository<Destination, Guid>)).Returns(_destRepo);
            _serviceProvider.GetService(typeof(IRepository<Notification, Guid>)).Returns(_notifRepo);
            _serviceProvider.GetService(typeof(IRepository<ApiMetric, Guid>)).Returns(_metricRepo);
            _serviceProvider.GetService(typeof(IGuidGenerator)).Returns(_guidGenerator);
            _serviceProvider.GetService(typeof(ITicketmasterService)).Returns(_ticketmasterService);
            _serviceProvider.GetService(typeof(ILogger<DailyDestinationUpdateWorker>)).Returns(_logger);
            _serviceProvider.GetService(typeof(IConfiguration)).Returns(_configuration);

            var asyncTimer = new AbpAsyncTimer();
            _worker = new DailyDestinationUpdateWorker(asyncTimer, null!);
        }

        [Fact]
        public async Task ProcessDestinationUpdates_WhenEventsReturned_ShouldCreateNotificationWithRealEventDetails()
        {
            // Arrange
            var destinationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var destination = new Destination(
                destinationId, "Mendoza", "Argentina", 120000, -32.8894, -68.8458,
                "https://example.com/mendoza.jpg", DateTime.UtcNow
            );
            var favorite = new FavoriteListItem(Guid.NewGuid(), destinationId, userId);

            _favRepo.GetListAsync(cancellationToken: Arg.Any<CancellationToken>()).Returns(Task.FromResult(new List<FavoriteListItem> { favorite }));
            _destRepo.FindAsync(destinationId, cancellationToken: Arg.Any<CancellationToken>()).Returns(Task.FromResult<Destination?>(destination));

            var realEvents = new List<TicketmasterEventDto>
            {
                new TicketmasterEventDto
                {
                    Id = "ev-vendimia-2026",
                    Name = "Fiesta Nacional de la Vendimia 2026",
                    FormattedDate = "07-03-2026",
                    Url = "https://www.ticketmaster.com/event/vendimia2026",
                    City = "Mendoza"
                }
            };

            _ticketmasterService.GetEventsByCityAsync("Mendoza", Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(realEvents));

            // Act
            await _worker.ProcessDestinationUpdatesAsync(_serviceProvider);

            // Assert
            await _notifRepo.Received(1).InsertAsync(Arg.Is<Notification>(n =>
                n.UserId == userId &&
                n.Title.Contains("Mendoza") &&
                n.Message.Contains("Fiesta Nacional de la Vendimia 2026") &&
                n.Message.Contains("07-03-2026")
            ), cancellationToken: Arg.Any<CancellationToken>());

            await _metricRepo.Received(1).InsertAsync(Arg.Is<ApiMetric>(m =>
                m.ApiName == "Ticketmaster" &&
                m.IsSuccess == true &&
                m.Endpoint.Contains("Mendoza") &&
                !m.Endpoint.Contains("TEST_API_KEY_VALID_123")
            ), cancellationToken: Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ProcessDestinationUpdates_WhenZeroEventsReturned_ShouldNotCreateAnyNotification()
        {
            // Arrange
            var destinationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var destination = new Destination(
                destinationId, "Bariloche", "Argentina", 130000, -41.1335, -71.3103,
                "https://example.com/bariloche.jpg", DateTime.UtcNow
            );
            var favorite = new FavoriteListItem(Guid.NewGuid(), destinationId, userId);

            _favRepo.GetListAsync(cancellationToken: Arg.Any<CancellationToken>()).Returns(Task.FromResult(new List<FavoriteListItem> { favorite }));
            _destRepo.FindAsync(destinationId, cancellationToken: Arg.Any<CancellationToken>()).Returns(Task.FromResult<Destination?>(destination));

            _ticketmasterService.GetEventsByCityAsync("Bariloche", Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new List<TicketmasterEventDto>()));

            // Act
            await _worker.ProcessDestinationUpdatesAsync(_serviceProvider);

            // Assert
            await _notifRepo.DidNotReceive().InsertAsync(Arg.Any<Notification>(), cancellationToken: Arg.Any<CancellationToken>());

            await _metricRepo.Received(1).InsertAsync(Arg.Is<ApiMetric>(m =>
                m.ApiName == "Ticketmaster" &&
                m.IsSuccess == true &&
                m.Endpoint.Contains("Bariloche")
            ), cancellationToken: Arg.Any<CancellationToken>());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ProcessDestinationUpdates_WhenApiKeyMissingOrEmpty_ShouldLogWarningAndCompleteWithoutThrowing(string? emptyApiKey)
        {
            // Arrange
            _configuration["TicketMaster:ApiKey"].Returns(emptyApiKey);
            var destinationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var favorite = new FavoriteListItem(Guid.NewGuid(), destinationId, userId);

            _favRepo.GetListAsync(cancellationToken: Arg.Any<CancellationToken>()).Returns(Task.FromResult(new List<FavoriteListItem> { favorite }));

            // Act & Assert
            await Should.NotThrowAsync(async () =>
            {
                await _worker.ProcessDestinationUpdatesAsync(_serviceProvider);
            });

            await _ticketmasterService.DidNotReceive().GetEventsByCityAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
            await _notifRepo.DidNotReceive().InsertAsync(Arg.Any<Notification>(), cancellationToken: Arg.Any<CancellationToken>());

            _logger.Received().Log(
                LogLevel.Warning,
                Arg.Any<EventId>(),
                Arg.Is<object>(o => o.ToString()!.Contains("TicketMaster") || o.ToString()!.Contains("API key")),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception?, string>>()
            );
        }

        [Fact]
        public async Task ProcessDestinationUpdates_WhenHttpOrServiceThrowsException_ShouldHandleGracefullyAndRecordFailedMetric()
        {
            // Arrange
            var destinationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var destination = new Destination(
                destinationId, "Salta", "Argentina", 140000, -24.7821, -65.4232,
                "https://example.com/salta.jpg", DateTime.UtcNow
            );
            var favorite = new FavoriteListItem(Guid.NewGuid(), destinationId, userId);

            _favRepo.GetListAsync(cancellationToken: Arg.Any<CancellationToken>()).Returns(Task.FromResult(new List<FavoriteListItem> { favorite }));
            _destRepo.FindAsync(destinationId, cancellationToken: Arg.Any<CancellationToken>()).Returns(Task.FromResult<Destination?>(destination));

            _ticketmasterService.GetEventsByCityAsync("Salta", Arg.Any<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("503 (Service Unavailable)"));

            // Act & Assert
            await Should.NotThrowAsync(async () =>
            {
                await _worker.ProcessDestinationUpdatesAsync(_serviceProvider);
            });

            await _notifRepo.DidNotReceive().InsertAsync(Arg.Any<Notification>(), cancellationToken: Arg.Any<CancellationToken>());

            await _metricRepo.Received(1).InsertAsync(Arg.Is<ApiMetric>(m =>
                m.ApiName == "Ticketmaster" &&
                m.IsSuccess == false &&
                m.ErrorMessage.Contains("503 (Service Unavailable)")
            ), cancellationToken: Arg.Any<CancellationToken>());
        }

        [Fact]
        public void TicketmasterResponseRawDto_Deserialization_ShouldParseSampleJsonPayload()
        {
            // Arrange
            var sampleJson = @"{
                ""_embedded"": {
                    ""events"": [
                        {
                            ""id"": ""vvG1GZ9u4t-123"",
                            ""name"": ""Festival de la Vendimia 2026"",
                            ""url"": ""https://www.ticketmaster.com/event/vvG1GZ9u4t-123"",
                            ""dates"": {
                                ""start"": {
                                    ""localDate"": ""2026-03-07"",
                                    ""localTime"": ""20:00:00""
                                },
                                ""timezone"": ""America/Argentina/Mendoza""
                            },
                            ""_embedded"": {
                                ""venues"": [
                                    {
                                        ""name"": ""Teatro Griego Frank Romero Day"",
                                        ""city"": { ""name"": ""Mendoza"" },
                                        ""country"": { ""name"": ""Argentina"", ""countryCode"": ""AR"" }
                                    }
                                ]
                            }
                        }
                    ]
                },
                ""page"": {
                    ""size"": 5,
                    ""totalElements"": 1,
                    ""totalPages"": 1,
                    ""number"": 0
                }
            }";

            // Act
            var parsed = JsonSerializer.Deserialize<TicketmasterResponseRawDto>(sampleJson);

            // Assert
            parsed.ShouldNotBeNull();
            parsed.Embedded.ShouldNotBeNull();
            parsed.Embedded.Events.ShouldNotBeNull();
            parsed.Embedded.Events.Count.ShouldBe(1);

            var first = parsed.Embedded.Events[0];
            first.Id.ShouldBe("vvG1GZ9u4t-123");
            first.Name.ShouldBe("Festival de la Vendimia 2026");
            first.Dates.ShouldNotBeNull();
            first.Dates.Start.ShouldNotBeNull();
            first.Dates.Start.LocalDate.ShouldBe("2026-03-07");
            first.Embedded?.Venues?.Count.ShouldBe(1);
            first.Embedded?.Venues?[0].City?.Name.ShouldBe("Mendoza");
        }
    }
}
