using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Common.Resources;
using raBudget.Common.Response;
using raBudget.Domain.Entities;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;
using RLib.Localization;

namespace raBudget.Application.Features.BudgetCategories.Command
{
    public class RemoveBudgetedAmount
    {
        public class Command : IRequest<Result>
        {
            public BudgetedAmountId BudgetedAmountId { get; set; }
        }

        public class Result: BaseResponse
        {
        }

		public class Notification : INotification
		{
			public BudgetCategory ReferenceBudgetCategory { get; set; }
			public BudgetCategory.BudgetedAmount ReferenceBudgetedAmount { get; set; }
		}

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly IWriteDbContext _writeDbContext;
            private readonly AccessControlService _accessControlService;
			private readonly IMediator _mediator;

            public Handler(IWriteDbContext writeDbContext, AccessControlService accessControlService, IMediator mediator)
            {
                _writeDbContext = writeDbContext;
                _accessControlService = accessControlService;
				_mediator = mediator;
			}

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var budgetCategory = await _writeDbContext.BudgetCategories
                                                          .Include(x => x.BudgetedAmounts)
                                                          .FirstOrDefaultAsync(x => x.BudgetedAmounts.Any(s => s.BudgetedAmountId == request.BudgetedAmountId), cancellationToken);

                if (!await _accessControlService.HasBudgetCategoryAccessAsync(budgetCategory.BudgetCategoryId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetCategoryNotFound));
                }

                var budgetedAmount = budgetCategory.BudgetedAmounts.FirstOrDefault(x => x.BudgetedAmountId == request.BudgetedAmountId)
                                     ?? throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetedAmountNotFound));

                budgetCategory.BudgetedAmounts.Remove(budgetedAmount);

                await _writeDbContext.SaveChangesAsync(cancellationToken);

				_ = _mediator.Publish(new Notification()
				{
					ReferenceBudgetCategory = budgetCategory,
                    ReferenceBudgetedAmount = budgetedAmount
				}, cancellationToken);

                return new Result() { };
            }
        }
    }
}