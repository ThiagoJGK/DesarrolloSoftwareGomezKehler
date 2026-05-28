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
    }
}
