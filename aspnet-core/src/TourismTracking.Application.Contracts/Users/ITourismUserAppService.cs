using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace TourismTracking.Users
{
    public interface ITourismUserAppService : IApplicationService
    {
        Task DeleteMyAccountAsync();
        Task<PublicUserProfileDto> GetPublicProfileAsync(Guid userId);
    }

    public class PublicUserProfileDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Photo { get; set; }
        public string Preferences { get; set; }
    }
}
