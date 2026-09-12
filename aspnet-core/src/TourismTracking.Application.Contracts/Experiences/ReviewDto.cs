using System;
using Volo.Abp.Application.Dtos;

namespace TourismTracking.Experiences
{
    public class ReviewDto : EntityDto<Guid>
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
        public Guid UserId { get; set; }
        public Guid DestinationId { get; set; }

        public string? AuthorName { get; set; }
        public string? AuthorUsername { get; set; }
        public string? AuthorAvatar { get; set; }
    }
}
