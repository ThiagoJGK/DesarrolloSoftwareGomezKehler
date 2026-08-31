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
            if (string.IsNullOrWhiteSpace(title)) 
                throw new ArgumentException("Title cannot be empty");
            Title = title;
        }
    }
}
