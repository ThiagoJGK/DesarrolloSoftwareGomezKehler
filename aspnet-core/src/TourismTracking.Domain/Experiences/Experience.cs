using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace TourismTracking.Experiences
{
    public class Experience : FullAuditedAggregateRoot<Guid>
    {
        public const int MaxTitleLength = 200;
        public const int MaxContentLength = 10000;
        public const int MaxKeywordsLength = 500;

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
            SetContent(content);
            SetKeywords(keywords);
        }

        public void UpdateDetails(string title, string content, string keywords)
        {
            SetTitle(title);
            SetContent(content);
            SetKeywords(keywords);
        }

        private void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) 
                throw new ArgumentException("Title cannot be empty");
            if (title.Length > MaxTitleLength)
                throw new ArgumentOutOfRangeException(nameof(title));
            Title = title;
        }

        private void SetContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content cannot be empty", nameof(content));
            if (content.Length > MaxContentLength)
                throw new ArgumentOutOfRangeException(nameof(content));
            Content = content;
        }

        private void SetKeywords(string keywords)
        {
            if (keywords != null && keywords.Length > MaxKeywordsLength)
                throw new ArgumentOutOfRangeException(nameof(keywords));
            Keywords = keywords;
        }
    }
}
