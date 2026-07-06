using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace TourismTracking.Notifications
{
    [Authorize]
    public class NotificationAppService : ApplicationService, INotificationAppService
    {
        private readonly IRepository<Notification, Guid> _notificationRepository;

        public NotificationAppService(IRepository<Notification, Guid> notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<List<NotificationDto>> GetMyNotificationsAsync()
        {
            var userId = CurrentUser.GetId();
            var queryable = await _notificationRepository.GetQueryableAsync();
            var notifications = await AsyncExecuter.ToListAsync(
                queryable.Where(n => n.UserId == userId).OrderByDescending(n => n.CreationTime)
            );

            return ObjectMapper.Map<List<Notification>, List<NotificationDto>>(notifications);
        }

        public async Task MarkAsReadAsync(Guid id)
        {
            var userId = CurrentUser.GetId();
            var notification = await _notificationRepository.GetAsync(id);

            if (notification.UserId != userId)
            {
                throw new UnauthorizedAccessException("No puedes marcar como leída una notificación de otro usuario.");
            }

            notification.MarkAsRead();
            await _notificationRepository.UpdateAsync(notification);
        }

        public async Task MarkAllAsReadAsync()
        {
            var userId = CurrentUser.GetId();
            var queryable = await _notificationRepository.GetQueryableAsync();
            var unreadNotifications = await AsyncExecuter.ToListAsync(
                queryable.Where(n => n.UserId == userId && !n.IsRead)
            );

            foreach (var notification in unreadNotifications)
            {
                notification.MarkAsRead();
                await _notificationRepository.UpdateAsync(notification);
            }
        }
    }
}
