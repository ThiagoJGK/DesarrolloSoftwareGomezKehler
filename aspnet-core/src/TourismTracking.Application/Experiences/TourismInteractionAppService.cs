using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Microsoft.AspNetCore.Authorization;

namespace TourismTracking.Experiences
{
    public class TourismInteractionAppService : ApplicationService, ITourismInteractionAppService
    {
        private readonly IRepository<Review, Guid> _reviewRepo;
        private readonly IRepository<Experience, Guid> _experienceRepo;
        private readonly IRepository<FavoriteListItem, Guid> _favoritesRepo;
        private readonly ICurrentUser _currentUser;

        public TourismInteractionAppService(
            IRepository<Review, Guid> reviewRepo,
            IRepository<Experience, Guid> experienceRepo,
            IRepository<FavoriteListItem, Guid> favoritesRepo,
            ICurrentUser currentUser)
        {
            _reviewRepo = reviewRepo;
            _experienceRepo = experienceRepo;
            _favoritesRepo = favoritesRepo;
            _currentUser = currentUser;
        }

        // --- Reviews ---
        [Authorize]
        public async Task<ReviewDto> AddReviewAsync(Guid destinationId, int rating, string comment)
        {
            var userId = _currentUser.Id ?? throw new UnauthorizedAccessException("Must be logged in.");
            var review = new Review(GuidGenerator.Create(), destinationId, userId, rating, comment);
            await _reviewRepo.InsertAsync(review);
            return ObjectMapper.Map<Review, ReviewDto>(review);
        }

        [Authorize]
        public async Task<ReviewDto> EditReviewAsync(Guid reviewId, int rating, string comment)
        {
            var review = await _reviewRepo.GetAsync(reviewId);
            if (review.UserId != _currentUser.Id) 
                throw new UnauthorizedAccessException("Not your review.");

            review.UpdateReview(rating, comment);
            await _reviewRepo.UpdateAsync(review);
            return ObjectMapper.Map<Review, ReviewDto>(review);
        }

        [Authorize]
        public async Task DeleteReviewAsync(Guid reviewId)
        {
            var review = await _reviewRepo.GetAsync(reviewId);
            if (review.UserId != _currentUser.Id) 
                throw new UnauthorizedAccessException("Cannot delete someone else's review");
            await _reviewRepo.DeleteAsync(reviewId);
        }

        [AllowAnonymous]
        public async Task<DestinationMetricsDto> GetDestinationAverageRatingAsync(Guid destinationId)
        {
            var reviews = await _reviewRepo.GetListAsync(r => r.DestinationId == destinationId);
            if (!reviews.Any()) return new DestinationMetricsDto { AverageRating = 0, TotalReviews = 0 };

            var avg = reviews.Average(r => r.Rating);
            return new DestinationMetricsDto { AverageRating = Math.Round(avg, 1), TotalReviews = reviews.Count };
        }

        [AllowAnonymous]
        public async Task<List<ReviewDto>> GetDestinationReviewsAsync(Guid destinationId)
        {
            var reviews = await _reviewRepo.GetListAsync(r => r.DestinationId == destinationId);
            return ObjectMapper.Map<List<Review>, List<ReviewDto>>(reviews);
        }

        // --- Experiences ---
        [Authorize]
        public async Task<ExperienceDto> CreateExperienceAsync(Guid destinationId, string title, string content, string keywords)
        {
            var userId = _currentUser.Id ?? throw new UnauthorizedAccessException("Must be logged in.");
            var exp = new Experience(GuidGenerator.Create(), destinationId, userId, title, content, keywords);
            await _experienceRepo.InsertAsync(exp);
            return ObjectMapper.Map<Experience, ExperienceDto>(exp);
        }

        [Authorize]
        public async Task<ExperienceDto> EditExperienceAsync(Guid experienceId, string title, string content, string keywords)
        {
            var exp = await _experienceRepo.GetAsync(experienceId);
            if (exp.UserId != _currentUser.Id) 
                throw new UnauthorizedAccessException("Not your experience.");
            exp.UpdateDetails(title, content, keywords);
            await _experienceRepo.UpdateAsync(exp);
            return ObjectMapper.Map<Experience, ExperienceDto>(exp);
        }

        [Authorize]
        public async Task DeleteExperienceAsync(Guid experienceId)
        {
            var exp = await _experienceRepo.GetAsync(experienceId);
            if (exp.UserId != _currentUser.Id) 
                throw new UnauthorizedAccessException("Cannot delete someone else's experience.");
            await _experienceRepo.DeleteAsync(exp);
        }

        [AllowAnonymous]
        public async Task<List<ExperienceDto>> GetExperiencesByDestinationAsync(Guid destinationId, string keywordFilter = null)
        {
            var query = await _experienceRepo.GetQueryableAsync();
            var filtered = query.Where(e => e.DestinationId == destinationId);
            
            if (!string.IsNullOrEmpty(keywordFilter))
            {
                filtered = filtered.Where(e => e.Keywords.Contains(keywordFilter));
            }
            return ObjectMapper.Map<List<Experience>, List<ExperienceDto>>(await AsyncExecuter.ToListAsync(filtered));
        }

        [Authorize]
        public async Task<List<ExperienceDto>> GetMyExperiencesAsync()
        {
            var userId = _currentUser.Id ?? throw new UnauthorizedAccessException("Must be logged in.");
            var query = await _experienceRepo.GetQueryableAsync();
            var filtered = query.Where(e => e.UserId == userId);
            return ObjectMapper.Map<List<Experience>, List<ExperienceDto>>(await AsyncExecuter.ToListAsync(filtered));
        }

        // --- Favorites ---
        [Authorize]
        public async Task AddToFavoritesAsync(Guid destinationId)
        {
            var userId = _currentUser.Id ?? throw new UnauthorizedAccessException("Must be logged in.");
            var exists = await _favoritesRepo.AnyAsync(f => f.DestinationId == destinationId && f.UserId == userId);
            if (!exists)
            {
                await _favoritesRepo.InsertAsync(new FavoriteListItem(GuidGenerator.Create(), destinationId, userId));
            }
        }

        [Authorize]
        public async Task RemoveFromFavoritesAsync(Guid destinationId)
        {
            var userId = _currentUser.Id ?? throw new UnauthorizedAccessException("Must be logged in.");
            var fav = await _favoritesRepo.FirstOrDefaultAsync(f => f.DestinationId == destinationId && f.UserId == userId);
            if (fav != null)
            {
                await _favoritesRepo.DeleteAsync(fav);
            }
        }

        [AllowAnonymous]
        public async Task<List<Guid>> GetMyFavoriteDestinationsAsync()
        {
            if (_currentUser.Id == null) return new List<Guid>();
            var userId = _currentUser.Id.Value;
            var myFavs = await _favoritesRepo.GetListAsync(f => f.UserId == userId);
            return myFavs.Select(f => f.DestinationId).ToList();
        }
    }
}
