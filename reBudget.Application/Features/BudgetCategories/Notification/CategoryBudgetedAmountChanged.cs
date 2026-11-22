using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using raBudget.Application.Infrastructure;
using raBudget.Application.Features.BudgetCategories.Command;
using raBudget.Application.Features.Budgets.Notification;
using raBudget.Domain.Entities;
using raBudget.Domain.Services;

namespace raBudget.Application.Features.BudgetCategories.Notification
{
    public class CategoryBudgetedAmountChanged
    {
        public class Listener : IEventHandler<CreateBudgetCategory.Notification>,
                                IEventHandler<AddBudgetedAmount.Notification>,
                                IEventHandler<RemoveBudgetCategory.Notification>,
                                IEventHandler<RemoveBudgetedAmount.Notification>,
                                IEventHandler<UpdateBudgetedAmountAmount.Notification>,
                                IEventHandler<UpdateBudgetedAmountValidFrom.Notification>


        {
            public async Task HandleAsync(CreateBudgetCategory.Notification notification, CancellationToken cancellationToken)
            {
                await new Notification()
                                        {
                                            UserId = notification.UserId,
                                            BudgetCategories = new List<BudgetCategory>() {notification.ReferenceBudgetCategory}
                                        }.PublishAsync(cancellation: cancellationToken);
            }

            public async Task HandleAsync(AddBudgetedAmount.Notification notification, CancellationToken cancellationToken)
            {
                await new Notification()
                                        {
                                            UserId = notification.UserId,
                                            BudgetCategories = new List<BudgetCategory>() {notification.ReferenceBudgetCategory}
                                        }.PublishAsync(cancellation: cancellationToken);
            }

            public async Task HandleAsync(RemoveBudgetCategory.Notification notification, CancellationToken cancellationToken)
            {
                await new Notification()
                                        {
                                            UserId = notification.UserId,
                                            BudgetCategories = new List<BudgetCategory>() {notification.ReferenceBudgetCategory}
                                        }.PublishAsync(cancellation: cancellationToken);
            }

            public async Task HandleAsync(RemoveBudgetedAmount.Notification notification, CancellationToken cancellationToken)
            {
                await new Notification()
                                        {
                                            UserId = notification.UserId,
                                            BudgetCategories = new List<BudgetCategory>() {notification.ReferenceBudgetCategory}
                                        }.PublishAsync(cancellation: cancellationToken);
            }

            public async Task HandleAsync(UpdateBudgetedAmountAmount.Notification notification, CancellationToken cancellationToken)
            {
                await new Notification()
                                        {
                                            UserId = notification.UserId,
                                            BudgetCategories = new List<BudgetCategory>() {notification.ReferenceBudgetCategory}
                                        }.PublishAsync(cancellation: cancellationToken);
            }

            public async Task HandleAsync(UpdateBudgetedAmountValidFrom.Notification notification, CancellationToken cancellationToken)
            {
                await new Notification()
                                        {
                                            UserId = notification.UserId,
                                            BudgetCategories = new List<BudgetCategory>() {notification.ReferenceBudgetCategory}
                                        }.PublishAsync(cancellation: cancellationToken);
            }
            }

        public class Notification : UserNotification
        {
            public List<BudgetCategory> BudgetCategories { get; set; }
        }

        public class Handler : IEventHandler<Notification>
        {
            private readonly IServiceScopeFactory _scopeFactory;

            public Handler(IServiceScopeFactory scopeFactory)
            {
                _scopeFactory = scopeFactory;
            }


            /// <inheritdoc />
            public async Task HandleAsync(Notification notification, CancellationToken cancellationToken)
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var balanceService = scope.ServiceProvider.GetRequiredService<BalanceService>();

                foreach (var budgetCategory in notification.BudgetCategories)
                {
                    await balanceService.CalculateBudgetCategoryBalance(budgetCategory.BudgetCategoryId, cancellationToken);
                    await new BudgetCategoryBalanceChanged.Notification()
                                            {
                                                UserId = notification.UserId,
                                                BudgetCategoryId = budgetCategory.BudgetCategoryId
                                            }.PublishAsync(cancellation: cancellationToken);
                }


                foreach (var budgetId in notification.BudgetCategories.Select(x => x.BudgetId).Distinct())
                {
                    await balanceService.CalculateBudgetBalance(budgetId, cancellationToken);
                    await new BudgetBalanceChanged()
                                            {
                                                UserId = notification.UserId,
                                                BudgetId = budgetId
                                            }.PublishAsync(cancellation: cancellationToken);
                }
            }
        }
    }
}