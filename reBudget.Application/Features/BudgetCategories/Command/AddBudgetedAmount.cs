using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using raBudget.Application.Infrastructure;
using raBudget.Common.Extensions;
using raBudget.Common.Resources;
using raBudget.Common.Response;
using raBudget.Domain.Entities;
using raBudget.Domain.Enums;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;
using RLib.Localization;

namespace raBudget.Application.Features.BudgetCategories.Command
{
    public class AddBudgetedAmount
    {
        public class Command : IApplicationRequest<Result>
        {
            public BudgetCategoryId BudgetCategoryId { get; set; }
            public MoneyAmount Amount { get; set; }
            public DateTime ValidFrom { get; set; }
        }

        public class Result : SingleResponse<BudgetCategoryDto>
        {
        }

        public class BudgetCategoryDto
        {
            public BudgetCategoryId BudgetCategoryId { get; set; }
            public string BudgetCategoryIconKey { get; set; }
            public BudgetCategoryIconId BudgetCategoryIconId { get; set; }
            public BudgetId BudgetId { get; set; }
            public eBudgetCategoryType BudgetCategoryType { get; set; }
            public int Order { get; set; }
            public string Name { get; set; }
            public MoneyAmount CurrentBudgetedAmount { get; set; }
            public List<BudgetedAmountDto> BudgetedAmounts { get; set; }

            /// <summary>Creates the category returned by this command.</summary>
            internal static BudgetCategoryDto CreateFrom(BudgetCategory category)
            {
                return new BudgetCategoryDto
                {
                    BudgetCategoryId = category.BudgetCategoryId,
                    BudgetCategoryIconKey = category.BudgetCategoryIcon?.IconKey,
                    BudgetCategoryIconId = category.BudgetCategoryIconId,
                    BudgetId = category.BudgetId,
                    BudgetCategoryType = category.BudgetCategoryType,
                    Order = category.Order,
                    Name = category.Name,
                    CurrentBudgetedAmount = category.CurrentBudgetedAmount,
                    BudgetedAmounts = category.BudgetedAmounts
                        .OrderBy(amount => amount.ValidFrom)
                        .Select(BudgetedAmountDto.CreateFrom)
                        .ToList()
                };
            }
        }

        public class BudgetedAmountDto
        {
            public BudgetedAmountId BudgetedAmountId { get; set; }
            public DateTime ValidFrom { get; set; }
            public DateTime? ValidTo { get; set; }
            public MoneyAmount Amount { get; set; }

            /// <summary>Creates a DTO for an amount in the category response.</summary>
            internal static BudgetedAmountDto CreateFrom(BudgetCategory.BudgetedAmount amount)
            {
                return new BudgetedAmountDto
                {
                    BudgetedAmountId = amount.BudgetedAmountId,
                    ValidFrom = amount.ValidFrom,
                    ValidTo = amount.ValidTo,
                    Amount = amount.Amount
                };
            }
        }

        public class Notification : UserNotification
		{
			public BudgetCategory ReferenceBudgetCategory { get; set; }
            public BudgetCategory.BudgetedAmount ReferenceBudgetedAmount { get; set; }
		}

        public class Handler : ICommandHandler<Command, Result>
        {
            private readonly IWriteDbContext _writeDbContext;
            private readonly AccessControlService _accessControlService;
			private readonly PostCommitEventPublisher _eventPublisher;

            public Handler(IWriteDbContext writeDbContext, AccessControlService accessControlService, PostCommitEventPublisher eventPublisher)
            {
                _writeDbContext = writeDbContext;
                _accessControlService = accessControlService;
				_eventPublisher = eventPublisher;
            }

            public async Task<Result> ExecuteAsync(Command request, CancellationToken cancellationToken)
            {
                if (!await _accessControlService.HasBudgetCategoryAccessAsync(request.BudgetCategoryId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetCategoryNotFound));
                }

                var budgetCategory = await _writeDbContext.BudgetCategories
                                                          .Include(x=>x.BudgetedAmounts)
                                                          .FirstOrDefaultAsync(x => x.BudgetCategoryId == request.BudgetCategoryId, cancellationToken);

                var budgetedAmount = budgetCategory.AddBudgetedAmount(request.Amount, request.ValidFrom.StartOfMonth());

                await _writeDbContext.SaveChangesAsync(cancellationToken);

				_=_eventPublisher.PublishAsync(new Notification()
				{
					ReferenceBudgetCategory = budgetCategory,
					ReferenceBudgetedAmount = budgetedAmount
				}, cancellationToken);

                return new Result()
                       {
                           Data = BudgetCategoryDto.CreateFrom(budgetCategory)
                       };
            }
        }
    }
}
