using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TourismTracking.Destinations;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace TourismTracking.Notifications
{
    [Authorize]
    public class NotificationAppService : ApplicationService, INotificationAppService
    {
        private readonly IRepository<Notification, Guid> _notificationRepository;
        private readonly IRepository<Destination, Guid>? _destinationRepository;

        public NotificationAppService(
            IRepository<Notification, Guid> notificationRepository,
            IRepository<Destination, Guid>? destinationRepository = null)
        {
            _notificationRepository = notificationRepository;
            _destinationRepository = destinationRepository;
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

        public async Task<NotificationDto> SendTestNotificationAsync()
        {
            var userId = CurrentUser.GetId();
            string destName = "Bariloche";
            if (_destinationRepository != null)
            {
                var firstDest = await _destinationRepository.FirstOrDefaultAsync();
                if (firstDest != null)
                {
                    destName = firstDest.Name;
                }
            }

            var notification = new Notification(
                GuidGenerator.Create(),
                userId,
                $"Aviso especial: {destName}",
                $"Alerta en tiempo real: Se han actualizado las condiciones climáticas y la agenda de eventos para {destName}."
            );
            await _notificationRepository.InsertAsync(notification);
            return ObjectMapper.Map<Notification, NotificationDto>(notification);
        }
    }
}
