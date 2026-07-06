using System.Collections.Generic;

namespace TourismTracking.Metrics
{
    public class SystemStatisticsDto
    {
        public int TotalUsers { get; set; }
        public int TotalExperiences { get; set; }
        public int TotalSavedDestinations { get; set; }
        public List<MostReviewedDestinationDto> MostReviewedDestinations { get; set; }
    }

    public class MostReviewedDestinationDto
    {
        public string DestinationName { get; set; }
        public string Country { get; set; }
        public int ReviewsCount { get; set; }
        public double AverageRating { get; set; }
    }
}
