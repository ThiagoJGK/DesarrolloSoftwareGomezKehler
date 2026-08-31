using System;
using Volo.Abp.ObjectMapping;
using TourismTracking.Destinations;
using TourismTracking.Experiences;
using TourismTracking.Notifications;
using TourismTracking.Metrics;
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
                        Longitude = d.Longitude,
                        ImageUrl = d.ImageUrl
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
                    UserId = r.UserId,
                    DestinationId = r.DestinationId
                };
            }

            if (destinationType == typeof(List<ReviewDto>) && source is List<Review> reviews)
            {
                var list = new List<ReviewDto>();
                foreach (var rev in reviews)
                {
                    list.Add(new ReviewDto
                    {
                        Id = rev.Id,
                        Rating = rev.Rating,
                        Comment = rev.Comment,
                        UserId = rev.UserId,
                        DestinationId = rev.DestinationId
                    });
                }
                return list;
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

            if (destinationType == typeof(List<ExperienceDto>) && source is List<Experience> exps)
            {
                var list = new List<ExperienceDto>();
                foreach (var exp in exps)
                {
                    list.Add(new ExperienceDto
                    {
                        Id = exp.Id,
                        DestinationId = exp.DestinationId,
                        UserId = exp.UserId,
                        Title = exp.Title,
                        Content = exp.Content,
                        Keywords = exp.Keywords
                    });
                }
                return list;
            }

            if (destinationType == typeof(List<NotificationDto>) && source is List<Notification> notifs)
            {
                var list = new List<NotificationDto>();
                foreach (var n in notifs)
                {
                    list.Add(new NotificationDto
                    {
                        Id = n.Id,
                        Title = n.Title,
                        Message = n.Message,
                        IsRead = n.IsRead,
                        CreationTime = n.CreationTime
                    });
                }
                return list;
            }

            if (destinationType == typeof(List<ApiMetricDto>) && source is List<ApiMetric> metrics)
            {
                var list = new List<ApiMetricDto>();
                foreach (var m in metrics)
                {
                    list.Add(new ApiMetricDto
                    {
                        Id = m.Id,
                        ApiName = m.ApiName,
                        Endpoint = m.Endpoint,
                        IsSuccess = m.IsSuccess,
                        ResponseTimeMs = m.ResponseTimeMs,
                        ErrorMessage = m.ErrorMessage,
                        CreationTime = m.CreationTime
                    });
                }
                return list;
            }

            return null;
        }

        public object Map(Type sourceType, Type destinationType, object source, object destination)
        {
            return Map(sourceType, destinationType, source);
        }
    }
}
