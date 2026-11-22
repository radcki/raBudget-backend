using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using raBudget.Application.Infrastructure;
using raBudget.Application.Features.Allocations.Command;
using raBudget.Application.Features.Budgets.Notification;
using raBudget.Domain.Entities;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;

namespace raBudget.Application.Features.BudgetCategories.Notification
{
    public class BudgetCategoryBalanceChanged
    {
        public class Listener : IEventHandler<CreateAllocation.Notification>,
                                IEventHandler<RemoveAllocation.Notification>,
                                IEventHandler<UpdateAllocationAmount.Notification>,
                                IEventHandler<UpdateAllocationDate.Notification>,
                                IEventHandler<UpdateAllocationSourceCategory.Notification>,
                                IEventHandler<UpdateAllocationTargetCategory.Notification>


        {
            private readonly IServiceScopeFactory _scopeFactory;

            public Listener(IServiceScopeFactory scopeFactory)
            {
                _scopeFactory = scopeFactory;
            }


            /// <inheritdoc />
            public async Task HandleAsync(CreateAllocation.Notification notification, CancellationToken cancellationToken)
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var writeDb = scope.ServiceProvider.GetRequiredService<IWriteDbContext>();
                var balanceService = scope.ServiceProvider.GetRequiredService<BalanceService>();

                var categories = new List<BudgetCategory>();
                categories.Add(writeDb.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.ReferenceAllocation.TargetBudgetCategoryId));
                if (notification.ReferenceAllocation.SourceBudgetCategoryId != null)
                {
                    categories.Add(writeDb.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.ReferenceAllocation.SourceBudgetCategoryId));
                }

                await RecalculateBalance(balanceService, categories, notification.UserId, notification.ReferenceAllocation.AllocationDate.Year, notification.ReferenceAllocation.AllocationDate.Month, cancellationToken);

                foreach (var budgetCategory in categories)
                {
                    await new Notification()
                                            {
                                                UserId = notification.UserId,
                                                BudgetCategoryId = budgetCategory.BudgetCategoryId
                                            }.PublishAsync(cancellation: cancellationToken);
                }
            }

            /// <inheritdoc />
            public async Task HandleAsync(RemoveAllocation.Notification notification, CancellationToken cancellationToken)
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var writeDb = scope.ServiceProvider.GetRequiredService<IWriteDbContext>();
                var balanceService = scope.ServiceProvider.GetRequiredService<BalanceService>();

                var categories = new List<BudgetCategory>();
                categories.Add(writeDb.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.ReferenceAllocation.TargetBudgetCategoryId));
                if (notification.ReferenceAllocation.SourceBudgetCategoryId != null)
                {
                    categories.Add(writeDb.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.ReferenceAllocation.SourceBudgetCategoryId));
                }

                await RecalculateBalance(balanceService, categories, notification.UserId, notification.ReferenceAllocation.AllocationDate.Year, notification.ReferenceAllocation.AllocationDate.Month, cancellationToken);

                foreach (var budgetCategory in categories)
                {
                    await new Notification()
                                            {
                                                UserId = notification.UserId,
                                                BudgetCategoryId = budgetCategory.BudgetCategoryId
                                            }.PublishAsync(cancellation: cancellationToken);
                }
            }

            /// <inheritdoc />
            public async Task HandleAsync(UpdateAllocationAmount.Notification notification, CancellationToken cancellationToken)
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var writeDb = scope.ServiceProvider.GetRequiredService<IWriteDbContext>();
                var balanceService = scope.ServiceProvider.GetRequiredService<BalanceService>();

                var categories = new List<BudgetCategory>();
                categories.Add(writeDb.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.ReferenceAllocation.TargetBudgetCategoryId));
                if (notification.ReferenceAllocation.SourceBudgetCategoryId != null)
                {
                    categories.Add(writeDb.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.ReferenceAllocation.SourceBudgetCategoryId));
                }

                await RecalculateBalance(balanceService, categories, notification.UserId, notification.ReferenceAllocation.AllocationDate.Year, notification.ReferenceAllocation.AllocationDate.Month, cancellationToken);
                foreach (var budgetCategory in categories)
                {
                    await new Notification()
                                            {
                                                UserId = notification.UserId,
                                                BudgetCategoryId = budgetCategory.BudgetCategoryId
                                            }.PublishAsync(cancellation: cancellationToken);
                }
            }

            public async Task HandleAsync(UpdateAllocationDate.Notification notification, CancellationToken cancellationToken)
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var writeDb = scope.ServiceProvider.GetRequiredService<IWriteDbContext>();
                var balanceService = scope.ServiceProvider.GetRequiredService<BalanceService>();

                var categories = new List<BudgetCategory>();
                categories.Add(writeDb.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.ReferenceAllocation.TargetBudgetCategoryId));
                if (notification.ReferenceAllocation.SourceBudgetCategoryId != null)
                {
                    categories.Add(writeDb.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.ReferenceAllocation.SourceBudgetCategoryId));
                }


                await RecalculateBalance(balanceService, categories, notification.UserId, notification.NewAllocationDate.Year, notification.NewAllocationDate.Month, cancellationToken);

                if (notification.NewAllocationDate.Year != notification.OldAllocationDate.Year || notification.NewAllocationDate.Month != notification.OldAllocationDate.Month)
                {
                    await RecalculateBalance(balanceService, categories, notification.UserId, notification.OldAllocationDate.Year, notification.OldAllocationDate.Month, cancellationToken);
                }

                foreach (var budgetCategory in categories)
                {
                    await new Notification()
                                            {
                                                UserId = notification.UserId,
                                                BudgetCategoryId = budgetCategory.BudgetCategoryId
                                            }.PublishAsync(cancellation: cancellationToken);
                }
            }


            /// <inheritdoc />
            public async Task HandleAsync(UpdateAllocationSourceCategory.Notification notification, CancellationToken cancellationToken)
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var writeDb = scope.ServiceProvider.GetRequiredService<IWriteDbContext>();
                var balanceService = scope.ServiceProvider.GetRequiredService<BalanceService>();

                var categories = new List<BudgetCategory>();
                if (notification.OldBudgetCategoryId != null)
                {
                    categories.Add(writeDb.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.OldBudgetCategoryId));
                }

                if (notification.NewBudgetCategoryId != null)
                {
                    categories.Add(writeDb.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.NewBudgetCategoryId));
                }

                await RecalculateBalance(balanceService, categories, notification.UserId, notification.ReferenceAllocation.AllocationDate.Year, notification.ReferenceAllocation.AllocationDate.Month, cancellationToken);
                foreach (var budgetCategory in categories)
                {
                    await new Notification()
                                            {
                                                UserId = notification.UserId,
                                                BudgetCategoryId = budgetCategory.BudgetCategoryId
                                            }.PublishAsync(cancellation: cancellationToken);
                }
            }


            /// <inheritdoc />
            public async Task HandleAsync(UpdateAllocationTargetCategory.Notification notification, CancellationToken cancellationToken)
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var writeDb = scope.ServiceProvider.GetRequiredService<IWriteDbContext>();
                var balanceService = scope.ServiceProvider.GetRequiredService<BalanceService>();

                var categories = new List<BudgetCategory>();
                if (notification.OldBudgetCategoryId != null)
                {
                    categories.Add(writeDb.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.OldBudgetCategoryId));
                }

                if (notification.NewBudgetCategoryId != null)
                {
                    categories.Add(writeDb.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.NewBudgetCategoryId));
                }

                await RecalculateBalance(balanceService, categories, notification.UserId, notification.ReferenceAllocation.AllocationDate.Year, notification.ReferenceAllocation.AllocationDate.Month, cancellationToken);
                foreach (var budgetCategory in categories)
                {
                    await new Notification()
                                            {
                                                UserId = notification.UserId,
                                                BudgetCategoryId = budgetCategory.BudgetCategoryId
                                            }.PublishAsync(cancellation: cancellationToken);
                }
            }

            private static async Task RecalculateBalance(BalanceService balanceService, IEnumerable<BudgetCategory> budgetCategories, string userId, int year, int month, CancellationToken cancellationToken)
            {
                foreach (var budgetCategory in budgetCategories)
                {
                    await balanceService.CalculateBudgetCategoryBalance(budgetCategory.BudgetCategoryId, year, month, cancellationToken);
                }

                foreach (var budgetId in budgetCategories.Select(x => x.BudgetId).Distinct())
                {
                    await balanceService.CalculateBudgetBalance(budgetId, cancellationToken);
                    await new BudgetBalanceChanged()
                                            {
                                                UserId = userId,
                                                BudgetId = budgetId
                                            }.PublishAsync(cancellation: cancellationToken);
                }
            }
        }

        public class Notification : UserNotification
        {
            public BudgetCategoryId BudgetCategoryId { get; set; }
        }
    }
}