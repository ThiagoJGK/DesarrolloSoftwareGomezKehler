using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace TourismTracking.Experiences
{
    public class Experience : FullAuditedAggregateRoot<Guid>
    {
        public Guid DestinationId { get; private set; }
        public Guid UserId { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }
        public string Keywords { get; private set; } // Ej: "gastronomía, seguridad"

        protected Experience() { }

        public Experience(Guid id, Guid destinationId, Guid userId, string title, string content, string keywords) 
            : base(id)
        {
            DestinationId = destinationId;
            UserId = userId;
            SetTitle(title);
            Content = content;
            Keywords = keywords;
        }

        public void UpdateDetails(string title, string content, string keywords)
        {
            SetTitle(title);
            Content = content;
            Keywords = keywords;
        }

        private void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title cannot be empty");
            Title = title;
        }
    }

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
            if (rating < 1 || rating > 5) throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");
            Rating = rating;
        }
    }

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
