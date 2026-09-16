using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using TourismTracking.Destinations;
using TourismTracking.Experiences;
using TourismTracking.Metrics;
using TourismTracking.Notifications;
using TourismTracking.Ticketmaster;
using TourismTracking.Workers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Threading;
using Xunit;

namespace TourismTracking.Application.Tests.Workers
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public MockHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_handler(request));
        }
    }

    public class DailyDestinationUpdateWorker_AdversarialTests
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

        public DailyDestinationUpdateWorker_AdversarialTests()
        {
            _favRepo = Substitute.For<IRepository<FavoriteListItem, Guid>>();
            _destRepo = Substitute.For<IRepository<Destination, Guid>>();
            _notifRepo = Substitute.For<IRepository<Notification, Guid>>();
            _metricRepo = Substitute.For<IRepository<ApiMetric, Guid>>();
            _guidGenerator = Substitute.For<IGuidGenerator>();
            _ticketmasterService = Substitute.For<ITicketmasterService>();
            _logger = Substitute.For<ILogger<DailyDestinationUpdateWorker>>();
            _configuration = Substitute.For<IConfiguration>();

            _configuration["TicketMaster:ApiKey"].Returns("ADVERSARIAL_TEST_KEY_456");
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
        public async Task StressTest_MultipleDestinations_OnlyDestinationsWithEventsTriggerNotifications()
        {
            var destId1 = Guid.NewGuid();
            var destId2 = Guid.NewGuid();
            var destId3 = Guid.NewGuid();

            var user1 = Guid.NewGuid();
            var user2 = Guid.NewGuid();
            var user3 = Guid.NewGuid();

            var dest1 = new Destination(destId1, "Mendoza", "Argentina", 120000, -32.88, -68.84, "img1", DateTime.UtcNow);
            var dest2 = new Destination(destId2, "Bariloche", "Argentina", 130000, -41.13, -71.31, "img2", DateTime.UtcNow);
            var dest3 = new Destination(destId3, "Salta", "Argentina", 140000, -24.78, -65.42, "img3", DateTime.UtcNow);

            var favorites = new List<FavoriteListItem>
            {
                new(Guid.NewGuid(), destId1, user1),
                new(Guid.NewGuid(), destId1, user2),
                new(Guid.NewGuid(), destId2, user2),
                new(Guid.NewGuid(), destId3, user3),
            };

            _favRepo.GetListAsync(cancellationToken: Arg.Any<CancellationToken>()).Returns(Task.FromResult(favorites));
            _destRepo.FindAsync(destId1, cancellationToken: Arg.Any<CancellationToken>()).Returns(Task.FromResult<Destination?>(dest1));
            _destRepo.FindAsync(destId2, cancellationToken: Arg.Any<CancellationToken>()).Returns(Task.FromResult<Destination?>(dest2));
            _destRepo.FindAsync(destId3, cancellationToken: Arg.Any<CancellationToken>()).Returns(Task.FromResult<Destination?>(dest3));

            _ticketmasterService.GetEventsByCityAsync("Mendoza", Arg.Any<CancellationToken>()).Returns(Task.FromResult(new List<TicketmasterEventDto>
            {
                new() { Name = "Vendimia 2026", FormattedDate = "07-03-2026" }
            }));

            _ticketmasterService.GetEventsByCityAsync("Bariloche", Arg.Any<CancellationToken>()).Returns(Task.FromResult(new List<TicketmasterEventDto>()));

            _ticketmasterService.GetEventsByCityAsync("Salta", Arg.Any<CancellationToken>()).ThrowsAsync(new HttpRequestException("Connection refused"));

            // Act
            await _worker.ProcessDestinationUpdatesAsync(_serviceProvider);

            // Assert: Exactly 2 notifications created (for Mendoza: user1 and user2)
            await _notifRepo.Received(2).InsertAsync(Arg.Any<Notification>(), cancellationToken: Arg.Any<CancellationToken>());
            
            await _notifRepo.Received(1).InsertAsync(Arg.Is<Notification>(n =>
                n.UserId == user1 && n.Title.Contains("Mendoza") && n.Message.Contains("Vendimia 2026") && n.Message.Contains("07-03-2026")
            ), cancellationToken: Arg.Any<CancellationToken>());

            await _notifRepo.Received(1).InsertAsync(Arg.Is<Notification>(n =>
                n.UserId == user2 && n.Title.Contains("Mendoza") && n.Message.Contains("Vendimia 2026") && n.Message.Contains("07-03-2026")
            ), cancellationToken: Arg.Any<CancellationToken>());

            // Metrics: 3 metrics recorded (2 success, 1 failure)
            await _metricRepo.Received(3).InsertAsync(Arg.Any<ApiMetric>(), cancellationToken: Arg.Any<CancellationToken>());
            await _metricRepo.Received(1).InsertAsync(Arg.Is<ApiMetric>(m => m.Endpoint.Contains("Mendoza") && m.IsSuccess), cancellationToken: Arg.Any<CancellationToken>());
            await _metricRepo.Received(1).InsertAsync(Arg.Is<ApiMetric>(m => m.Endpoint.Contains("Bariloche") && m.IsSuccess), cancellationToken: Arg.Any<CancellationToken>());
            await _metricRepo.Received(1).InsertAsync(Arg.Is<ApiMetric>(m => m.Endpoint.Contains("Salta") && !m.IsSuccess && m.ErrorMessage!.Contains("Connection refused")), cancellationToken: Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task StressTest_DuplicateFavoritesBySameUser_DoesNotSendDuplicateNotifications()
        {
            var destId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var dest = new Destination(destId, "Ushuaia", "Argentina", 80000, -54.80, -68.30, "img", DateTime.UtcNow);

            var favorites = new List<FavoriteListItem>
            {
                new(Guid.NewGuid(), destId, userId),
                new(Guid.NewGuid(), destId, userId)
            };

            _favRepo.GetListAsync(cancellationToken: Arg.Any<CancellationToken>()).Returns(Task.FromResult(favorites));
            _destRepo.FindAsync(destId, cancellationToken: Arg.Any<CancellationToken>()).Returns(Task.FromResult<Destination?>(dest));
            _ticketmasterService.GetEventsByCityAsync("Ushuaia", Arg.Any<CancellationToken>()).Returns(Task.FromResult(new List<TicketmasterEventDto>
            {
                new() { Name = "Festival Fin del Mundo", FormattedDate = "21-06-2026" }
            }));

            // Act
            await _worker.ProcessDestinationUpdatesAsync(_serviceProvider);

            // Assert: Exactly 1 notification despite duplicate favorite entries
            await _notifRepo.Received(1).InsertAsync(Arg.Is<Notification>(n => n.UserId == userId), cancellationToken: Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task StressTest_DestinationNotFoundInRepository_SkipsGracefully()
        {
            var destId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var favorites = new List<FavoriteListItem>
            {
                new(Guid.NewGuid(), destId, userId)
            };

            _favRepo.GetListAsync(cancellationToken: Arg.Any<CancellationToken>()).Returns(Task.FromResult(favorites));
            _destRepo.FindAsync(destId, cancellationToken: Arg.Any<CancellationToken>()).Returns(Task.FromResult<Destination?>(null));

            // Act
            await _worker.ProcessDestinationUpdatesAsync(_serviceProvider);

            // Assert
            await _ticketmasterService.DidNotReceive().GetEventsByCityAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
            await _notifRepo.DidNotReceive().InsertAsync(Arg.Any<Notification>(), cancellationToken: Arg.Any<CancellationToken>());
            await _metricRepo.DidNotReceive().InsertAsync(Arg.Any<ApiMetric>(), cancellationToken: Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task StressTest_EmptyFavoritesList_WorkerCompletesImmediately()
        {
            _favRepo.GetListAsync(cancellationToken: Arg.Any<CancellationToken>()).Returns(Task.FromResult(new List<FavoriteListItem>()));

            // Act
            await _worker.ProcessDestinationUpdatesAsync(_serviceProvider);

            // Assert
            await _destRepo.DidNotReceive().FindAsync(Arg.Any<Guid>(), cancellationToken: Arg.Any<CancellationToken>());
            await _ticketmasterService.DidNotReceive().GetEventsByCityAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
            await _notifRepo.DidNotReceive().InsertAsync(Arg.Any<Notification>(), cancellationToken: Arg.Any<CancellationToken>());
        }

        [Theory]
        [InlineData("{}")]
        [InlineData("{\"_embedded\": null}")]
        [InlineData("{\"_embedded\": {}}")]
        [InlineData("{\"_embedded\": {\"events\": []}}")]
        [InlineData("{\"page\": {\"totalElements\": 0}}")]
        public void StressTest_TicketmasterResponseRawDto_HandlesEmptyPayloads(string json)
        {
            var result = JsonSerializer.Deserialize<TicketmasterResponseRawDto>(json);
            result.ShouldNotBeNull();
            (result.Embedded?.Events?.Count ?? 0).ShouldBe(0);
        }

        [Fact]
        public void StressTest_TicketmasterResponseRawDto_HandlesPartialAndCorruptFieldStructures()
        {
            var corruptJson = @"{
                ""_embedded"": {
                    ""events"": [
                        {
                            ""id"": null,
                            ""name"": null,
                            ""url"": null,
                            ""dates"": null,
                            ""_embedded"": null
                        },
                        {
                            ""id"": ""ev2"",
                            ""name"": ""Concert 2"",
                            ""dates"": {
                                ""start"": null
                            },
                            ""_embedded"": {
                                ""venues"": []
                            }
                        },
                        {
                            ""id"": ""ev3"",
                            ""name"": ""Concert 3"",
                            ""dates"": {
                                ""start"": {
                                    ""localDate"": null,
                                    ""dateTime"": ""2026-11-20T21:00:00Z""
                                }
                            },
                            ""_embedded"": {
                                ""venues"": [
                                    {
                                        ""name"": null,
                                        ""city"": null,
                                        ""country"": null
                                    }
                                ]
                            }
                        }
                    ]
                }
            }";

            var result = JsonSerializer.Deserialize<TicketmasterResponseRawDto>(corruptJson);
            result.ShouldNotBeNull();
            result.Embedded.ShouldNotBeNull();
            result.Embedded.Events.ShouldNotBeNull();
            result.Embedded.Events.Count.ShouldBe(3);

            var ev1 = result.Embedded.Events[0];
            ev1.Id.ShouldBeNull();
            ev1.Name.ShouldBeNull();
            ev1.Dates.ShouldBeNull();
            ev1.Embedded.ShouldBeNull();

            var ev2 = result.Embedded.Events[1];
            ev2.Id.ShouldBe("ev2");
            ev2.Dates!.Start.ShouldBeNull();
            ev2.Embedded!.Venues.ShouldBeEmpty();

            var ev3 = result.Embedded.Events[2];
            ev3.Dates!.Start!.DateTime.ShouldBe(new DateTime(2026, 11, 20, 21, 0, 0, DateTimeKind.Utc));
            ev3.Embedded!.Venues![0].Name.ShouldBeNull();
            ev3.Embedded!.Venues![0].City.ShouldBeNull();
        }

        [Fact]
        public async Task StressTest_TicketmasterService_DirectExecutionWithMockedHttp()
        {
            // Test 1: HTTP 200 with complete and varied date structures
            var jsonResponse = @"{
                ""_embedded"": {
                    ""events"": [
                        {
                            ""id"": ""ev-1"",
                            ""name"": ""Show Acrobático"",
                            ""dates"": {
                                ""start"": {
                                    ""localDate"": ""2026-10-15""
                                }
                            },
                            ""_embedded"": {
                                ""venues"": [{ ""name"": ""Arena 1"", ""city"": { ""name"": ""Rosario"" } }]
                            }
                        },
                        {
                            ""id"": ""ev-2"",
                            ""name"": null,
                            ""dates"": {
                                ""start"": {
                                    ""dateTime"": ""2026-11-05T18:00:00Z""
                                }
                            }
                        },
                        {
                            ""id"": ""ev-3"",
                            ""name"": ""Fiesta Provincial"",
                            ""dates"": null
                        }
                    ]
                }
            }";

            var handler = new MockHttpMessageHandler(req =>
            {
                req.RequestUri!.Query.ShouldContain("apikey=VALID_KEY");
                req.RequestUri!.Query.ShouldContain("city=Rosario");
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
                };
            });

            var httpClient = new HttpClient(handler);
            var factory = Substitute.For<IHttpClientFactory>();
            factory.CreateClient(Arg.Any<string>()).Returns(httpClient);

            var config = Substitute.For<IConfiguration>();
            config["TicketMaster:ApiKey"].Returns("VALID_KEY");
            var logger = Substitute.For<ILogger<TicketmasterService>>();

            var service = new TicketmasterService(factory, config, logger);

            var events = await service.GetEventsByCityAsync("Rosario");

            events.Count.ShouldBe(3);
            events[0].Name.ShouldBe("Show Acrobático");
            events[0].FormattedDate.ShouldBe("15-10-2026");
            events[0].Venue.ShouldBe("Arena 1");
            events[0].City.ShouldBe("Rosario");

            // ev-2: null name falls back to "Evento Cultural", dateTime formats to 05-11-2026
            events[1].Name.ShouldBe("Evento Cultural");
            events[1].FormattedDate.ShouldBe("05-11-2026");

            // ev-3: null dates falls back to current date
            events[2].Name.ShouldBe("Fiesta Provincial");
            events[2].FormattedDate.ShouldNotBeNullOrWhiteSpace();

            // Test 2: HTTP 401 Unauthorized returns empty list without crashing
            var handler401 = new MockHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.Unauthorized));
            var client401 = new HttpClient(handler401);
            factory.CreateClient(Arg.Any<string>()).Returns(client401);

            var events401 = await service.GetEventsByCityAsync("Rosario");
            events401.ShouldBeEmpty();

            // Test 3: Blank API key returns empty list without calling HTTP
            config["TicketMaster:ApiKey"].Returns("");
            var eventsBlankKey = await service.GetEventsByCityAsync("Rosario");
            eventsBlankKey.ShouldBeEmpty();

            // Test 4: Blank City returns empty list
            var eventsBlankCity = await service.GetEventsByCityAsync("  ");
            eventsBlankCity.ShouldBeEmpty();
        }
    }
}
