using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Data;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;
using Xunit;
using Shouldly;

using Volo.Abp.Modularity;

namespace TourismTracking.Users
{
    public abstract class TourismUserAppService_Tests<TStartupModule> : TourismTrackingApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly ITourismUserAppService _tourismUserAppService;
        private readonly IdentityUserManager _userManager;
        private readonly ICurrentUser _currentUser;
        private readonly IProfileAppService _profileAppService;
        private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;

        protected TourismUserAppService_Tests()
        {
            _tourismUserAppService = GetRequiredService<ITourismUserAppService>();
            _userManager = GetRequiredService<IdentityUserManager>();
            _currentUser = GetRequiredService<ICurrentUser>();
            _profileAppService = GetRequiredService<IProfileAppService>();
            _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
        }

        [Fact]
        public async Task Should_Get_Public_Profile_With_Extra_Properties()
        {
            // Arrange
            var user = new IdentityUser(Guid.NewGuid(), "testuser", "testuser@tourismtracking.com");
            user.SetProperty("Photo", "my_avatar.png");
            user.SetProperty("Preferences", "culture,nature");
            
            var result = await _userManager.CreateAsync(user, "TestUserPass123!");
            result.Succeeded.ShouldBeTrue();

            // Act
            var profile = await _tourismUserAppService.GetPublicProfileAsync(user.Id);

            // Assert
            profile.ShouldNotBeNull();
            profile.Id.ShouldBe(user.Id);
            profile.UserName.ShouldBe("testuser");
            profile.Photo.ShouldBe("my_avatar.png");
            profile.Preferences.ShouldBe("culture,nature");
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Public_Profile_User_Not_Found()
        {
            // Act & Assert
            await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            {
                await _tourismUserAppService.GetPublicProfileAsync(Guid.NewGuid());
            });
        }

        [Fact]
        public async Task Should_Delete_My_Account_When_Logged_In()
        {
            // Arrange
            var user = new IdentityUser(Guid.NewGuid(), "deleteuser", "deleteuser@tourismtracking.com");
            var result = await _userManager.CreateAsync(user, "DeleteUserPass123!");
            result.Succeeded.ShouldBeTrue();

            // Authenticate user using the Change method of ICurrentPrincipalAccessor
            var claims = new[]
            {
                new Claim(AbpClaimTypes.UserId, user.Id.ToString()),
                new Claim(AbpClaimTypes.UserName, user.UserName),
                new Claim(AbpClaimTypes.Email, user.Email)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            using (_currentPrincipalAccessor.Change(principal))
            {
                // Act
                await _tourismUserAppService.DeleteMyAccountAsync();

                // Assert
                var deletedUser = await _userManager.FindByIdAsync(user.Id.ToString());
                deletedUser.ShouldBeNull();
            }
        }

        [Fact]
        public async Task Should_Throw_Unauthorized_When_Deleting_Account_And_Not_Logged_In()
        {
            // Authenticate as anonymous
            var principal = new ClaimsPrincipal(new ClaimsIdentity());

            using (_currentPrincipalAccessor.Change(principal))
            {
                // Act & Assert
                await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                {
                    await _tourismUserAppService.DeleteMyAccountAsync();
                });
            }
        }

        [Fact]
        public async Task Should_Allow_Updating_Profile_With_Extra_Properties()
        {
            // Arrange
            var user = new IdentityUser(Guid.NewGuid(), "edituser", "edituser@tourismtracking.com");
            var createResult = await _userManager.CreateAsync(user, "EditUserPass123!");
            createResult.Succeeded.ShouldBeTrue();

            var claims = new[]
            {
                new Claim(AbpClaimTypes.UserId, user.Id.ToString()),
                new Claim(AbpClaimTypes.UserName, user.UserName),
                new Claim(AbpClaimTypes.Email, user.Email)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            using (_currentPrincipalAccessor.Change(principal))
            {
                // Act
                var profile = await _profileAppService.GetAsync();
                profile.Name = "UpdatedName";
                profile.Surname = "UpdatedSurname";

                var updateDto = new UpdateProfileDto
                {
                    Name = profile.Name,
                    Surname = profile.Surname,
                    Email = profile.Email,
                    UserName = profile.UserName,
                    ConcurrencyStamp = profile.ConcurrencyStamp
                };
                updateDto.SetProperty("Photo", "new_avatar.png");
                updateDto.SetProperty("Preferences", "sports,music");

                var updatedProfile = await _profileAppService.UpdateAsync(updateDto);

                // Assert
                updatedProfile.Name.ShouldBe("UpdatedName");
                updatedProfile.GetProperty<string>("Photo").ShouldBe("new_avatar.png");
                updatedProfile.GetProperty<string>("Preferences").ShouldBe("sports,music");

                var freshUser = await _userManager.FindByIdAsync(user.Id.ToString());
                freshUser.Name.ShouldBe("UpdatedName");
                freshUser.GetProperty<string>("Photo").ShouldBe("new_avatar.png");
                freshUser.GetProperty<string>("Preferences").ShouldBe("sports,music");
            }
        }
    }
}
