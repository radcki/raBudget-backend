using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
    public class CreateBudgetCategory
    {
        public class Command : IRequest<Result>
        {
            public BudgetId BudgetId { get; set; }
            public string Name { get; set; }
            public BudgetCategoryIconId BudgetCategoryIconId { get; set; }
            public eBudgetCategoryType BudgetCategoryType { get; set; }
            public List<BudgetedAmountDto> BudgetedAmounts { get; set; }
        }

        public class BudgetedAmountDto
        {
            public MoneyAmount Amount { get; set; }
            public DateTime ValidFrom { get; set; }
        }

        public class Result : IdResponse<BudgetCategoryId>
        {
        }

		public class Notification : INotification
		{
			public BudgetCategory ReferenceBudgetCategory { get; set; }
		}

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly IUserContext _userContext;
            private readonly IWriteDbContext _writeDbContext;
            private readonly AccessControlService _accessControlService;
			private readonly IMediator _mediator;

            public Handler(IUserContext userContext, IWriteDbContext writeDbContext, AccessControlService accessControlService, IMediator mediator)
            {
                _userContext = userContext;
                _writeDbContext = writeDbContext;
                _accessControlService = accessControlService;
				_mediator = mediator;
			}

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var budgetId = request.BudgetId;
                if (!await _accessControlService.HasBudgetAccessAsync(budgetId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetNotFound));
                }

                var budget = await _writeDbContext.Budgets
                                                  .FirstOrDefaultAsync(x => x.BudgetId == budgetId
                                                                            && x.OwnerUserId == _userContext.UserId, cancellationToken);

                var icon = await _writeDbContext.BudgetCategoryIcons
                                                .FirstOrDefaultAsync(x => x.BudgetCategoryIconId == request.BudgetCategoryIconId, cancellationToken);

                var budgetCategory = Domain.Entities.BudgetCategory.Create(budget, request.Name, icon, request.BudgetCategoryType);
                foreach (var requestBudgetedAmount in request.BudgetedAmounts)
                {
                    budgetCategory.AddBudgetedAmount(requestBudgetedAmount.Amount, requestBudgetedAmount.ValidFrom);
                }
                _writeDbContext.BudgetCategories.Add(budgetCategory);

                await _writeDbContext.SaveChangesAsync(cancellationToken);

				_ = _mediator.Publish(new Notification()
				{
					ReferenceBudgetCategory = budgetCategory,
				}, cancellationToken);

                return new Result()
                       {
                           Id = budgetCategory.BudgetCategoryId
                       };
            }
        }
    }
}