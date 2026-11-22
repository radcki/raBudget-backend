using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.SignalR;
using raBudget.Application.Features.BudgetCategories.Command;
using raBudget.Application.Features.Transactions.Command;
using raBudget.Application.Infrastructure;

namespace raBudget.Api.Hubs
{
    public static class TransactionNotificationEvents
    {
        public const string TransactionListChanged = "TRANSACTION_LIST_CHANGED";
        public const string TransactionUpdated = "TRANSACTION_UPDATED";
    }


    public class TransactionNotificationsHub : Hub
    {
        public class Listener : IClientNotificationHandler,
                                IEventHandler<CreateTransaction.Notification>,
                                IEventHandler<RemoveTransaction.Notification>,
                                IEventHandler<AddSubTransaction.Notification>,
                                IEventHandler<UpdateSubTransactionAmount.Notification>,
                                IEventHandler<UpdateSubTransactionDate.Notification>,
                                IEventHandler<UpdateSubTransactionDescription.Notification>,
                                IEventHandler<RemoveSubTransaction.Notification>,
                                IEventHandler<UpdateTransactionDate.Notification>,
                                IEventHandler<UpdateTransactionCategory.Notification>,
                                IEventHandler<UpdateTransactionDescription.Notification>,
                                IEventHandler<RemoveBudgetCategory.Notification>
        {
            private readonly IHubContext<TransactionNotificationsHub> _hubContext;

            public Listener(IHubContext<TransactionNotificationsHub> hubContext)
            {
                _hubContext = hubContext;
            }

            public async Task HandleAsync(CreateTransaction.Notification notification, CancellationToken cancellationToken)
            {
                await SendAsync(TransactionNotificationEvents.TransactionListChanged, notification, cancellationToken);
            }

            /// <inheritdoc />
            public async Task HandleAsync(RemoveTransaction.Notification notification, CancellationToken cancellationToken)
            {
                await SendAsync(TransactionNotificationEvents.TransactionListChanged, notification, cancellationToken);
            }
            /// <inheritdoc />
            public async Task HandleAsync(RemoveBudgetCategory.Notification notification, CancellationToken cancellationToken)
            {
                await SendAsync(TransactionNotificationEvents.TransactionListChanged, notification, cancellationToken);
            }

            public async Task HandleAsync(UpdateTransactionDescription.Notification notification, CancellationToken cancellationToken)
            {
                await SendAsync(TransactionNotificationEvents.TransactionUpdated, notification, cancellationToken);
            }

            public async Task HandleAsync(AddSubTransaction.Notification notification, CancellationToken cancellationToken)
            {
                await SendAsync(TransactionNotificationEvents.TransactionUpdated, notification, cancellationToken);
            }

            public async Task HandleAsync(UpdateSubTransactionAmount.Notification notification, CancellationToken cancellationToken)
            {
                await SendAsync(TransactionNotificationEvents.TransactionUpdated, notification, cancellationToken);
            }

            public async Task HandleAsync(UpdateSubTransactionDate.Notification notification, CancellationToken cancellationToken)
            {
                await SendAsync(TransactionNotificationEvents.TransactionUpdated, notification, cancellationToken);
            }

            public async Task HandleAsync(UpdateSubTransactionDescription.Notification notification, CancellationToken cancellationToken)
            {
                await SendAsync(TransactionNotificationEvents.TransactionUpdated, notification, cancellationToken);
            }

            public async Task HandleAsync(UpdateTransactionDate.Notification notification, CancellationToken cancellationToken)
            {
                await SendAsync(TransactionNotificationEvents.TransactionUpdated, notification, cancellationToken);
            }

            public async Task HandleAsync(UpdateTransactionCategory.Notification notification, CancellationToken cancellationToken)
            {
                await SendAsync(TransactionNotificationEvents.TransactionUpdated, notification, cancellationToken);
            }

            public async Task HandleAsync(RemoveSubTransaction.Notification notification, CancellationToken cancellationToken)
            {
                await SendAsync(TransactionNotificationEvents.TransactionUpdated, notification, cancellationToken);
            }


            private Task SendAsync(string eventKey, UserNotification notification, CancellationToken cancellationToken)
            {
                return _hubContext.Clients.User(notification.UserId).SendAsync(eventKey, notification, cancellationToken);
            }
        }
    }
}
