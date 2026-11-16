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

        [Fact]
        public void Should_Throw_When_Title_Exceeds_MaxLength()
        {
            var tooLongTitle = new string('a', Experience.MaxTitleLength + 1);
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Experience(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), tooLongTitle, "Content", "tags")
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Should_Throw_For_Empty_Or_Null_Content(string invalidContent)
        {
            Assert.Throws<ArgumentException>(() =>
                new Experience(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Title", invalidContent, "tags")
            );
        }

        [Fact]
        public void Should_Throw_When_Content_Exceeds_MaxLength()
        {
            var tooLongContent = new string('a', Experience.MaxContentLength + 1);
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Experience(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Title", tooLongContent, "tags")
            );
        }

        [Fact]
        public void Should_Throw_When_Keywords_Exceeds_MaxLength()
        {
            var tooLongKeywords = new string('a', Experience.MaxKeywordsLength + 1);
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Experience(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Title", "Content", tooLongKeywords)
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void UpdateDetails_Should_Throw_For_Empty_Or_Null_Content(string invalidContent)
        {
            var exp = new Experience(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Title", "Content", "tags");
            Assert.Throws<ArgumentException>(() => exp.UpdateDetails("Title", invalidContent, "tags"));
        }

        [Fact]
        public void UpdateDetails_Should_Throw_When_Limits_Exceeded()
        {
            var exp = new Experience(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Title", "Content", "tags");
            Assert.Throws<ArgumentOutOfRangeException>(() => exp.UpdateDetails(new string('a', Experience.MaxTitleLength + 1), "Content", "tags"));
            Assert.Throws<ArgumentOutOfRangeException>(() => exp.UpdateDetails("Title", new string('a', Experience.MaxContentLength + 1), "tags"));
            Assert.Throws<ArgumentOutOfRangeException>(() => exp.UpdateDetails("Title", "Content", new string('a', Experience.MaxKeywordsLength + 1)));
        }
    }
}
