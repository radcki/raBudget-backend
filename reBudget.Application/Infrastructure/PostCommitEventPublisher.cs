using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using raBudget.Domain.Interfaces;

namespace raBudget.Application.Infrastructure
{
    /// <summary>Dispatches notifications after a command commits and logs failures independently of its response.</summary>
    public sealed class PostCommitEventPublisher(
        IUserContext userContext,
        IServiceProvider services,
        ILogger<PostCommitEventPublisher> logger)
    {
        /// <summary>Captures the notification recipient and runs application handlers before client handlers.</summary>
        public async Task PublishAsync<TEvent>(TEvent notification, CancellationToken cancellationToken)
            where TEvent : UserNotification
        {
            notification.UserId = userContext.UserId;

            try
            {
                var eventBus = services.GetRequiredService<EventBus<TEvent>>();
                await eventBus.PublishFilteredAsync(notification, handler => !IsClientHandler(handler), cancellation: cancellationToken);
                await eventBus.PublishFilteredAsync(notification, IsClientHandler, cancellation: cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Post-commit notification {NotificationType} failed", typeof(TEvent).FullName);
            }
        }

        private static bool IsClientHandler(Type handler) => typeof(IClientNotificationHandler).IsAssignableFrom(handler);
    }
}
