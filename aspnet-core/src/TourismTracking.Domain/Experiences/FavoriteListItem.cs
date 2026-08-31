using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace TourismTracking.Experiences
{
    public class FavoriteListItem : CreationAuditedEntity<Guid>
    {
        public Guid DestinationId { get; private set; }
        public Guid UserId { get; private set; }

        protected FavoriteListItem() { }

        public FavoriteListItem(Guid id, Guid destinationId, Guid userId)
            : base(id)
        {
            DestinationId = destinationId;
            UserId = userId;
        }
    }
}
