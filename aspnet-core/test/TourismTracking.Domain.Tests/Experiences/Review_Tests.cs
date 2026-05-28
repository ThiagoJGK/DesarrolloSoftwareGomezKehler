using System;
using Xunit;
using Shouldly;
using TourismTracking.Experiences;

namespace TourismTracking.Domain.Tests.Experiences
{
    public class Review_Tests
    {
        [Fact]
        public void Should_Create_Valid_Review()
        {
            var review = new Review(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 4, "Muy bueno");
            review.Rating.ShouldBe(4);
            review.Comment.ShouldBe("Muy bueno");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void Should_Accept_Valid_Ratings(int rating)
        {
            var review = new Review(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), rating, "Test");
            review.Rating.ShouldBe(rating);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(6)]
        [InlineData(100)]
        public void Should_Throw_For_Invalid_Ratings(int rating)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Review(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), rating, "Test")
            );
        }

        [Fact]
        public void UpdateReview_Should_Update_Rating_And_Comment()
        {
            var review = new Review(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 3, "Regular");
            review.UpdateReview(5, "Excelente después de todo");
            review.Rating.ShouldBe(5);
            review.Comment.ShouldBe("Excelente después de todo");
        }

        [Fact]
        public void UpdateReview_Should_Throw_For_Invalid_Rating()
        {
            var review = new Review(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 3, "Ok");
            Assert.Throws<ArgumentOutOfRangeException>(() => review.UpdateReview(0, "Invalid"));
        }

        [Fact]
        public void Should_Store_DestinationId_And_UserId()
        {
            var destId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var review = new Review(Guid.NewGuid(), destId, userId, 5, "Perfect");
            review.DestinationId.ShouldBe(destId);
            review.UserId.ShouldBe(userId);
        }
    }
}
