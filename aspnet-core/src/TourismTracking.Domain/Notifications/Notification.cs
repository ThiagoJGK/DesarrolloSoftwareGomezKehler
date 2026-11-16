using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace TourismTracking.Notifications
{
    public class Notification : CreationAuditedEntity<Guid>
    {
        public Guid UserId { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public bool IsRead { get; private set; }

        protected Notification() { }

        public Notification(Guid id, Guid userId, string title, string message) : base(id)
        {
            UserId = userId;
            Title = title;
            Message = message;
            IsRead = false;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}
