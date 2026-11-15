using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace TourismTracking.Experiences
{
    public class Review : FullAuditedEntity<Guid>
    {
        public const int MaxCommentLength = 2000;

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
            SetComment(comment);
        }

        public void UpdateReview(int rating, string comment)
        {
            SetRating(rating);
            SetComment(comment);
        }

        private void SetRating(int rating)
        {
            if (rating < 1 || rating > 5) 
                throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");
            Rating = rating;
        }

        private void SetComment(string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
                throw new ArgumentException("Comment cannot be empty.", nameof(comment));
            if (comment.Length > MaxCommentLength)
                throw new ArgumentOutOfRangeException(nameof(comment), $"Comment cannot exceed {MaxCommentLength} characters.");
            Comment = comment.Trim();
        }
    }
}
