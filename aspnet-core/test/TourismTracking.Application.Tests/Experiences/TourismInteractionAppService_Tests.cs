using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Xunit;
using TourismTracking.Experiences;
using System.Linq;

namespace TourismTracking.Application.Tests.Experiences
{
    public class TourismInteractionAppService_Tests
    {
        private readonly IRepository<Review, Guid> _reviewRepo;
        private readonly IRepository<Experience, Guid> _experienceRepo;
        private readonly IRepository<FavoriteListItem, Guid> _favoritesRepo;
        private readonly ICurrentUser _currentUser;
        private readonly TourismInteractionAppService _appService;

        public TourismInteractionAppService_Tests()
        {
            _reviewRepo = Substitute.For<IRepository<Review, Guid>>();
            _experienceRepo = Substitute.For<IRepository<Experience, Guid>>();
            _favoritesRepo = Substitute.For<IRepository<FavoriteListItem, Guid>>();
            _currentUser = Substitute.For<ICurrentUser>();

            _appService = new TourismInteractionAppService(
                _reviewRepo,
                _experienceRepo,
                _favoritesRepo,
                _currentUser
            );
        }

        [Fact]
        public async Task GetDestinationAverageRatingAsync_Should_Calculate_Correct_Average()
        {
            // Arrange
            var destinationId = Guid.NewGuid();
            var reviews = new List<Review>
            {
                new Review(Guid.NewGuid(), destinationId, Guid.NewGuid(), 4, "Bueno"),
                new Review(Guid.NewGuid(), destinationId, Guid.NewGuid(), 5, "Excelente"),
                new Review(Guid.NewGuid(), destinationId, Guid.NewGuid(), 2, "Malo")
            };

            // NSubstitute configuration for GetListAsync
            _reviewRepo.GetListAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Review, bool>>>())
                .Returns(Task.FromResult(reviews));

            // Act
            var result = await _appService.GetDestinationAverageRatingAsync(destinationId);

            // Assert
            result.ShouldNotBeNull();
            result.TotalReviews.ShouldBe(3);
            result.AverageRating.ShouldBe(Math.Round((4d + 5d + 2d) / 3, 1));
        }

        [Fact]
        public async Task AddReviewAsync_Should_Throw_Unauthorized_If_Not_Logged_In()
        {
            // Arrange
            _currentUser.Id.Returns((Guid?)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await _appService.AddReviewAsync(Guid.NewGuid(), 5, "Test");
            });
        }
        
        [Fact]
        public async Task DeleteReviewAsync_Should_Throw_If_User_Is_Not_Author()
        {
            // Arrange
            var currentUserId = Guid.NewGuid();
            var authorUserId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            
            _currentUser.Id.Returns(currentUserId);
            var review = new Review(reviewId, Guid.NewGuid(), authorUserId, 5, "Test");
            _reviewRepo.GetAsync(reviewId).Returns(Task.FromResult(review));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await _appService.DeleteReviewAsync(reviewId);
            });
            
            ex.Message.ShouldContain("Cannot delete someone else's review");
        }

        [Fact]
        public async Task EditReviewAsync_Should_Throw_If_Not_Author()
        {
            // Arrange
            var currentUserId = Guid.NewGuid();
            var authorUserId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            
            _currentUser.Id.Returns(currentUserId);
            var review = new Review(reviewId, Guid.NewGuid(), authorUserId, 4, "Original");
            _reviewRepo.GetAsync(reviewId).Returns(Task.FromResult(review));

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await _appService.EditReviewAsync(reviewId, 5, "Hacked");
            });
        }

        [Fact]
        public async Task GetDestinationAverageRatingAsync_Should_Return_Zero_For_No_Reviews()
        {
            // Arrange
            var destinationId = Guid.NewGuid();
            _reviewRepo.GetListAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Review, bool>>>())
                .Returns(Task.FromResult(new List<Review>()));

            // Act
            var result = await _appService.GetDestinationAverageRatingAsync(destinationId);

            // Assert
            result.AverageRating.ShouldBe(0);
            result.TotalReviews.ShouldBe(0);
        }

        [Fact]
        public async Task AddToFavoritesAsync_Should_Throw_If_Not_Logged_In()
        {
            // Arrange
            _currentUser.Id.Returns((Guid?)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await _appService.AddToFavoritesAsync(Guid.NewGuid());
            });
        }

        [Fact]
        public async Task RemoveFromFavoritesAsync_Should_Throw_If_Not_Logged_In()
        {
            // Arrange
            _currentUser.Id.Returns((Guid?)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await _appService.RemoveFromFavoritesAsync(Guid.NewGuid());
            });
        }

        [Fact]
        public async Task GetMyFavoriteDestinationsAsync_Should_Return_Empty_If_Not_Logged_In()
        {
            // Arrange
            _currentUser.Id.Returns((Guid?)null);

            // Act
            var result = await _appService.GetMyFavoriteDestinationsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task DeleteExperienceAsync_Should_Throw_If_Not_Author()
        {
            // Arrange
            var currentUserId = Guid.NewGuid();
            var authorUserId = Guid.NewGuid();
            var experienceId = Guid.NewGuid();
            
            _currentUser.Id.Returns(currentUserId);
            var experience = new Experience(experienceId, Guid.NewGuid(), authorUserId, "Title", "Content", "tags");
            _experienceRepo.GetAsync(experienceId).Returns(Task.FromResult(experience));

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await _appService.DeleteExperienceAsync(experienceId);
            });
        }

        [Fact]
        public async Task CreateExperienceAsync_Should_Throw_If_Not_Logged_In()
        {
            // Arrange
            _currentUser.Id.Returns((Guid?)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await _appService.CreateExperienceAsync(Guid.NewGuid(), "Title", "Content", "tags");
            });
        }
    }
}
