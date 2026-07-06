using System;
using Volo.Abp.ObjectMapping;
using TourismTracking.Destinations;
using TourismTracking.Experiences;
using System.Collections.Generic;

namespace TourismTracking.Application.Tests
{
    public class SimpleTestObjectMapper : IObjectMapper
    {
        public IAutoObjectMappingProvider AutoObjectMappingProvider => null;

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return (TDestination)Map(typeof(TSource), typeof(TDestination), source);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return (TDestination)Map(typeof(TSource), typeof(TDestination), source);
        }

        public object Map(Type sourceType, Type destinationType, object source)
        {
            if (source == null) return null;

            if (destinationType == typeof(List<DestinationDto>) && source is List<Destination> dests)
            {
                var dtos = new List<DestinationDto>();
                foreach (var d in dests)
                {
                    dtos.Add(new DestinationDto 
                    { 
                        Id = d.Id, 
                        Name = d.Name, 
                        Country = d.Country, 
                        Population = d.Population, 
                        Latitude = d.Latitude, 
                        Longitude = d.Longitude 
                    });
                }
                return dtos;
            }

            if (destinationType == typeof(ReviewDto) && source is Review r)
            {
                return new ReviewDto 
                { 
                    Id = r.Id, 
                    Rating = r.Rating, 
                    Comment = r.Comment, 
                    UserId = r.UserId 
                };
            }

            if (destinationType == typeof(ExperienceDto) && source is Experience e)
            {
                return new ExperienceDto 
                { 
                    Id = e.Id, 
                    DestinationId = e.DestinationId, 
                    UserId = e.UserId, 
                    Title = e.Title, 
                    Content = e.Content, 
                    Keywords = e.Keywords 
                };
            }

            return null;
        }

        public object Map(Type sourceType, Type destinationType, object source, object destination)
        {
            return Map(sourceType, destinationType, source);
        }
    }
}
