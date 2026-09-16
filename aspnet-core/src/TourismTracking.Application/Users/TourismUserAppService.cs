using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace TourismTracking.Users
{
    [Authorize]
    public class TourismUserAppService : ApplicationService, ITourismUserAppService
    {
        private readonly IdentityUserManager _userManager;
        private readonly ICurrentUser _currentUser;

        public TourismUserAppService(
            IdentityUserManager userManager,
            ICurrentUser currentUser)
        {
            _userManager = userManager;
            _currentUser = currentUser;
        }

        public async Task DeleteMyAccountAsync()
        {
            if (!_currentUser.IsAuthenticated || _currentUser.Id == null || _currentUser.Id == Guid.Empty)
            {
                throw new UnauthorizedAccessException("Must be logged in to delete your account.");
            }

            var user = await _userManager.FindByIdAsync(_currentUser.Id.ToString());
            if (user == null)
            {
                throw new UserFriendlyException("User not found.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new UserFriendlyException("Failed to delete user account: " + string.Join(", ", result.Errors));
            }
        }

        [AllowAnonymous]
        public async Task<PublicUserProfileDto> GetPublicProfileAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new UserFriendlyException("User not found.");
            }

            return new PublicUserProfileDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Photo = user.GetProperty<string>("Photo"),
                Preferences = user.GetProperty<string>("Preferences")
            };
        }
    }
}
