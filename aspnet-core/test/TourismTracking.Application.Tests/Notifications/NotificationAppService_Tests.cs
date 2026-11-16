using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Guids;
using Volo.Abp.Linq;
using Xunit;
using TourismTracking.Notifications;

namespace TourismTracking.Application.Tests.Notifications
{
    public class NotificationAppService_Tests
    {
        private readonly IRepository<Notification, Guid> _notificationRepository;
        private readonly ICurrentUser _currentUser;
        private readonly NotificationAppService _service;
        private readonly Guid _currentUserId = Guid.NewGuid();

        public NotificationAppService_Tests()
        {
            _notificationRepository = Substitute.For<IRepository<Notification, Guid>>();
            _currentUser = Substitute.For<ICurrentUser>();
            _currentUser.Id.Returns(_currentUserId);
            _currentUser.IsAuthenticated.Returns(true);

            var serviceProvider = Substitute.For<IServiceProvider>();
            var objectMapper = new SimpleTestObjectMapper();
            var guidGenerator = Substitute.For<IGuidGenerator>();
            guidGenerator.Create().Returns(Guid.NewGuid());

            var asyncExecuter = Substitute.For<IAsyncQueryableExecuter>();
            asyncExecuter.ToListAsync(Arg.Any<IQueryable<Notification>>(), Arg.Any<CancellationToken>())
                .Returns(callInfo => Task.FromResult(callInfo.Arg<IQueryable<Notification>>().ToList()));

            serviceProvider.GetService(typeof(IObjectMapper)).Returns(objectMapper);
            serviceProvider.GetService(typeof(IGuidGenerator)).Returns(guidGenerator);
            serviceProvider.GetService(typeof(ICurrentUser)).Returns(_currentUser);
            serviceProvider.GetService(typeof(IAsyncQueryableExecuter)).Returns(asyncExecuter);

            var lazyServiceProvider = new AbpLazyServiceProvider(serviceProvider);

            _service = new NotificationAppService(_notificationRepository);
            _service.LazyServiceProvider = lazyServiceProvider;
        }

        [Fact]
        public async Task MarkAsRead_Should_Mark_Own_Notification_As_Read()
        {
            var notifId = Guid.NewGuid();
            var notification = new Notification(notifId, _currentUserId, "Aviso", "Nuevo evento en destino");

            _notificationRepository.GetAsync(notifId).Returns(Task.FromResult(notification));

            await _service.MarkAsReadAsync(notifId);

            notification.IsRead.ShouldBeTrue();
            await _notificationRepository.Received(1).UpdateAsync(notification);
        }

        [Fact]
        public async Task MarkAsRead_Should_Throw_When_Notification_Belongs_To_Other_User()
        {
            var notifId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var notification = new Notification(notifId, otherUserId, "Aviso", "Mensaje privado");

            _notificationRepository.GetAsync(notifId).Returns(Task.FromResult(notification));

            await Should.ThrowAsync<UnauthorizedAccessException>(async () =>
            {
                await _service.MarkAsReadAsync(notifId);
            });

            notification.IsRead.ShouldBeFalse();
            await _notificationRepository.DidNotReceive().UpdateAsync(notification);
        }

        [Fact]
        public async Task GetMyNotifications_Should_Return_Only_User_Notifications()
        {
            var myNotifs = new List<Notification>
            {
                new Notification(Guid.NewGuid(), _currentUserId, "Aviso 1", "Contenido 1"),
                new Notification(Guid.NewGuid(), _currentUserId, "Aviso 2", "Contenido 2"),
                new Notification(Guid.NewGuid(), Guid.NewGuid(), "Aviso Ajeno", "Contenido 3")
            };

            _notificationRepository.GetQueryableAsync().Returns(Task.FromResult(myNotifs.AsQueryable()));

            var result = await _service.GetMyNotificationsAsync();

            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result.All(n => n.Title.StartsWith("Aviso ")).ShouldBeTrue();
        }
    }
}
