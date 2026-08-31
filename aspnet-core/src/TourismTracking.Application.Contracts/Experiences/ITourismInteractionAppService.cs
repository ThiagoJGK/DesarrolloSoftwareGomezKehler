using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace TourismTracking.Experiences
{
    public interface ITourismInteractionAppService : IApplicationService
    {
        // CRUD Reseñas (Reviews)
        Task<ReviewDto> AddReviewAsync(Guid destinationId, int rating, string comment);
        Task<ReviewDto> EditReviewAsync(Guid reviewId, int rating, string comment);
        Task DeleteReviewAsync(Guid reviewId);
        Task<DestinationMetricsDto> GetDestinationAverageRatingAsync(Guid destinationId);
        Task<List<ReviewDto>> GetDestinationReviewsAsync(Guid destinationId);

        // CRUD Experiencias
        Task<ExperienceDto> CreateExperienceAsync(Guid destinationId, string title, string content, string keywords);
        Task<ExperienceDto> EditExperienceAsync(Guid experienceId, string title, string content, string keywords);
        Task DeleteExperienceAsync(Guid experienceId);
        Task<List<ExperienceDto>> GetExperiencesByDestinationAsync(Guid destinationId, string keywordFilter = null);
        Task<List<ExperienceDto>> GetMyExperiencesAsync();
        
        // Operaciones de Favoritos
        Task AddToFavoritesAsync(Guid destinationId);
        Task RemoveFromFavoritesAsync(Guid destinationId);
        Task<List<Guid>> GetMyFavoriteDestinationsAsync();
    }
}
