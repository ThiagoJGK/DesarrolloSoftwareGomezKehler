using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace TourismTracking.Notifications
{
    public interface INotificationAppService : IApplicationService
    {
        Task<List<NotificationDto>> GetMyNotificationsAsync();
        Task MarkAsReadAsync(Guid id);
        Task MarkAllAsReadAsync();
        Task<NotificationDto> SendTestNotificationAsync(Guid? destinationId = null);
    }
}
