using AutoMapper;
using TourismTracking.Destinations;
using TourismTracking.Experiences;

namespace TourismTracking;

public class TourismTrackingApplicationAutoMapperProfile : Profile
{
    public TourismTrackingApplicationAutoMapperProfile()
    {
        CreateMap<Destination, DestinationDto>();
        CreateMap<Review, ReviewDto>();
        CreateMap<Experience, ExperienceDto>();
        CreateMap<Notifications.Notification, Notifications.NotificationDto>();
        CreateMap<Metrics.ApiMetric, Metrics.ApiMetricDto>();
    }
}
