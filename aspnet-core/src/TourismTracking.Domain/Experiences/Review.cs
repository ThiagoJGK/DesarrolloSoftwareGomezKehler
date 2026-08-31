using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace TourismTracking.Experiences
{
    public class Review : FullAuditedEntity<Guid>
    {
        public Guid DestinationId { get; private set; }
        public Guid UserId { get; private set; }
        public int Rating { get; private set; } // 1 a 5 estrellas
        public string Comment { get; private set; }

        protected Review() { }

        public Review(Guid id, Guid destinationId, Guid userId, int rating, string comment)
            : base(id)
        {
            DestinationId = destinationId;
            UserId = userId;
            SetRating(rating);
            Comment = comment;
        }

        public void UpdateReview(int rating, string comment)
        {
            SetRating(rating);
            Comment = comment;
        }

        private void SetRating(int rating)
        {
            if (rating < 1 || rating > 5) 
                throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");
            Rating = rating;
        }
    }
}
