using System;
using Volo.Abp.Application.Dtos;

namespace TourismTracking.Notifications
{
    public class NotificationDto : EntityDto<Guid>
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
