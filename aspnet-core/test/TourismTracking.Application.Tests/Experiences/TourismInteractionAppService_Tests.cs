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
    }
}
