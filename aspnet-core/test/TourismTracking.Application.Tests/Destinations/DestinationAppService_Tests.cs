using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using NSubstitute;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Xunit;
using TourismTracking.Destinations;
using TourismTracking.Experiences;
using System.Net.Http;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using TourismTracking.Metrics;

namespace TourismTracking.Application.Tests.Destinations
{
    public class DestinationAppService_Tests
    {
        private readonly IRepository<Destination, Guid> _destinationRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRepository<Review, Guid> _reviewRepository;
        private readonly IRepository<Experience, Guid> _experienceRepository;
        private readonly IRepository<FavoriteListItem, Guid> _favoritesRepository;
        private readonly IRepository<ApiMetric, Guid> _apiMetricRepository;
        private readonly DestinationAppService _appService;

        public DestinationAppService_Tests()
        {
            _destinationRepository = Substitute.For<IRepository<Destination, Guid>>();
            _httpClientFactory = Substitute.For<IHttpClientFactory>();
            _reviewRepository = Substitute.For<IRepository<Review, Guid>>();
            _experienceRepository = Substitute.For<IRepository<Experience, Guid>>();
            _favoritesRepository = Substitute.For<IRepository<FavoriteListItem, Guid>>();
            _apiMetricRepository = Substitute.For<IRepository<ApiMetric, Guid>>();

            var serviceProvider = Substitute.For<IServiceProvider>();
            
            var objectMapper = new SimpleTestObjectMapper();
            var guidGenerator = Substitute.For<IGuidGenerator>();
            guidGenerator.Create().Returns(_ => Guid.NewGuid());

            serviceProvider.GetService(typeof(IObjectMapper)).Returns(objectMapper);
            serviceProvider.GetService(typeof(IGuidGenerator)).Returns(guidGenerator);

            var lazyServiceProvider = new AbpLazyServiceProvider(serviceProvider);

            _appService = new DestinationAppService(
                _destinationRepository,
                _httpClientFactory,
                _reviewRepository,
                _experienceRepository,
                _favoritesRepository,
                _apiMetricRepository
            );
            _appService.LazyServiceProvider = lazyServiceProvider;
        }

        [Fact]
        public async Task GetSavedDestinationsAsync_Should_Return_Mapped_Destinations()
        {
            // Arrange
            var destinations = new List<Destination>
            {
                new Destination(Guid.NewGuid(), "Mendoza", "Argentina", 120000, -32.8894, -68.8458, "url", DateTime.UtcNow),
                new Destination(Guid.NewGuid(), "Paris", "France", 2200000, 48.8566, 2.3522, "url", DateTime.UtcNow)
            };

            _destinationRepository.GetListAsync().Returns(Task.FromResult(destinations));

            // Act
            var result = await _appService.GetSavedDestinationsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result[0].Name.ShouldBe("Mendoza");
            result[1].Name.ShouldBe("Paris");
        }

        [Fact]
        public async Task DeleteSavedDestinationAsync_Should_Cascade_Delete_Related_Entities()
        {
            // Arrange
            var destinationId = Guid.NewGuid();

            // Act
            await _appService.DeleteSavedDestinationAsync(destinationId);

            // Assert
            await _favoritesRepository.Received(1).DeleteAsync(Arg.Any<System.Linq.Expressions.Expression<Func<FavoriteListItem, bool>>>());
            await _experienceRepository.Received(1).DeleteAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Experience, bool>>>());
            await _reviewRepository.Received(1).DeleteAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Review, bool>>>());
            await _destinationRepository.Received(1).DeleteAsync(destinationId);
        }

        [Fact]
        public void DeleteSavedDestinationAsync_Should_Require_Admin_Role()
        {
            var method = typeof(DestinationAppService).GetMethod(nameof(DestinationAppService.DeleteSavedDestinationAsync));
            method.ShouldNotBeNull();

            var authorizeAttr = method.GetCustomAttribute<AuthorizeAttribute>();
            authorizeAttr.ShouldNotBeNull();
            authorizeAttr.Roles.ShouldBe("admin");
        }

        [Fact]
        public async Task SaveDestinationToInternalDbAsync_Should_Serialize_Concurrent_Calls_Without_Duplicates()
        {
            var stored = new List<Destination>();

            _destinationRepository
                .FirstOrDefaultAsync(Arg.Any<Expression<Func<Destination, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(async callInfo =>
                {
                    await Task.Delay(15);
                    var predicate = callInfo.Arg<Expression<Func<Destination, bool>>>().Compile();
                    return stored.FirstOrDefault(predicate);
                });

            _destinationRepository
                .InsertAsync(Arg.Any<Destination>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
                .Returns(callInfo =>
                {
                    var entity = callInfo.Arg<Destination>();
                    stored.Add(entity);
                    return Task.FromResult(entity);
                });

            _destinationRepository
                .UpdateAsync(Arg.Any<Destination>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
                .Returns(callInfo => Task.FromResult(callInfo.Arg<Destination>()));

            var tasks = Enumerable.Range(0, 5).Select(_ => _appService.SaveDestinationToInternalDbAsync(new SaveDestinationInput
            {
                Name = "  Mendoza  ",
                Country = "  Argentina  ",
                Population = 120000,
                Latitude = -32.8894,
                Longitude = -68.8458,
                ImageUrl = "https://example.com/mendoza.jpg"
            }));

            var results = await Task.WhenAll(tasks);

            results.Length.ShouldBe(5);
            stored.Count.ShouldBe(1);
            stored[0].Name.ShouldBe("Mendoza");
            stored[0].Country.ShouldBe("Argentina");
            await _destinationRepository.Received(1).InsertAsync(Arg.Any<Destination>(), Arg.Any<bool>(), Arg.Any<CancellationToken>());
            await _destinationRepository.Received(4).UpdateAsync(Arg.Any<Destination>(), Arg.Any<bool>(), Arg.Any<CancellationToken>());
        }
    }
}
