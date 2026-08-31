using System;
using Volo.Abp.Application.Dtos;

namespace TourismTracking.Experiences
{
    public class ExperienceDto : EntityDto<Guid>
    {
        public Guid DestinationId { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Keywords { get; set; }
    }
}
