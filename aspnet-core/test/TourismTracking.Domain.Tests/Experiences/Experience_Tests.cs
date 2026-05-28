using System;
using Xunit;
using Shouldly;
using TourismTracking.Experiences;

namespace TourismTracking.Domain.Tests.Experiences
{
    public class Experience_Tests
    {
        [Fact]
        public void Should_Create_Valid_Experience()
        {
            var exp = new Experience(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Mi viaje a París", "Fue increíble...", "gastronomía, cultura");
            exp.Title.ShouldBe("Mi viaje a París");
            exp.Content.ShouldBe("Fue increíble...");
            exp.Keywords.ShouldBe("gastronomía, cultura");
        }

        [Fact]
        public void Should_Throw_For_Empty_Title()
        {
            Assert.Throws<ArgumentException>(() =>
                new Experience(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "", "Content", "tags")
            );
        }

        [Fact]
        public void Should_Throw_For_Null_Title()
        {
            Assert.Throws<ArgumentException>(() =>
                new Experience(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, "Content", "tags")
            );
        }

        [Fact]
        public void Should_Throw_For_Whitespace_Title()
        {
            Assert.Throws<ArgumentException>(() =>
                new Experience(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "   ", "Content", "tags")
            );
        }

        [Fact]
        public void UpdateDetails_Should_Update_All_Fields()
        {
            var exp = new Experience(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Original", "Old content", "old");
            exp.UpdateDetails("Updated Title", "New content here", "new, updated");
            exp.Title.ShouldBe("Updated Title");
            exp.Content.ShouldBe("New content here");
            exp.Keywords.ShouldBe("new, updated");
        }

        [Fact]
        public void UpdateDetails_Should_Throw_For_Empty_Title()
        {
            var exp = new Experience(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Valid", "Content", "tags");
            Assert.Throws<ArgumentException>(() => exp.UpdateDetails("", "Content", "tags"));
        }

        [Fact]
        public void Should_Store_DestinationId_And_UserId()
        {
            var destId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var exp = new Experience(Guid.NewGuid(), destId, userId, "Title", "Content", "keywords");
            exp.DestinationId.ShouldBe(destId);
            exp.UserId.ShouldBe(userId);
        }
    }
}
