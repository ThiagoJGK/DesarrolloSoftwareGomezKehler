using System;
using Xunit;
using Shouldly;
using TourismTracking.Destinations;

namespace TourismTracking.Domain.Tests.Destinations
{
    public class Destination_Tests
    {
        [Fact]
        public void Should_Create_Valid_Destination()
        {
            // Act
            var dest = new Destination(
                Guid.NewGuid(), "Buenos Aires", "Argentina",
                15000000, -34.6037, -58.3816, "https://example.com/ba.jpg", DateTime.UtcNow
            );

            // Assert
            dest.Name.ShouldBe("Buenos Aires");
            dest.Country.ShouldBe("Argentina");
            dest.Population.ShouldBe(15000000);
            dest.Latitude.ShouldBe(-34.6037);
            dest.Longitude.ShouldBe(-58.3816);
        }

        [Fact]
        public void Should_Throw_For_Empty_Name()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Destination(Guid.NewGuid(), "", "Argentina", 1000, 0, 0, null, DateTime.UtcNow)
            );
            ex.ParamName.ShouldBe("name");
        }

        [Fact]
        public void Should_Throw_For_Null_Name()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Destination(Guid.NewGuid(), null, "Argentina", 1000, 0, 0, null, DateTime.UtcNow)
            );
            ex.ParamName.ShouldBe("name");
        }

        [Fact]
        public void Should_Throw_For_Whitespace_Name()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Destination(Guid.NewGuid(), "   ", "Argentina", 1000, 0, 0, null, DateTime.UtcNow)
            );
            ex.ParamName.ShouldBe("name");
        }

        [Fact]
        public void Should_Throw_For_Empty_Country()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Destination(Guid.NewGuid(), "Paris", "", 1000, 0, 0, null, DateTime.UtcNow)
            );
            ex.ParamName.ShouldBe("country");
        }

        [Fact]
        public void Should_Throw_For_Null_Country()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Destination(Guid.NewGuid(), "Paris", null, 1000, 0, 0, null, DateTime.UtcNow)
            );
            ex.ParamName.ShouldBe("country");
        }

        [Fact]
        public void UpdateDetails_Should_Update_Population_And_ImageUrl()
        {
            var dest = new Destination(Guid.NewGuid(), "Tokyo", "Japan", 1000, 35.6762, 139.6503, "old.jpg", DateTime.UtcNow);
            dest.UpdateDetails(14000000, "new-image.jpg");
            dest.Population.ShouldBe(14000000);
            dest.ImageUrl.ShouldBe("new-image.jpg");
        }

        [Fact]
        public void UpdateDetails_Should_Not_Override_ImageUrl_With_Empty()
        {
            var dest = new Destination(Guid.NewGuid(), "Tokyo", "Japan", 1000, 35.6762, 139.6503, "original.jpg", DateTime.UtcNow);
            dest.UpdateDetails(2000, "");
            dest.ImageUrl.ShouldBe("original.jpg");
            dest.Population.ShouldBe(2000);
        }
    }
}
