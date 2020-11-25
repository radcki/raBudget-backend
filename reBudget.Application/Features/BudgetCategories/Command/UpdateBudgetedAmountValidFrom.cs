using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Common.Resources;
using raBudget.Common.Response;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;
using RLib.Localization;

namespace raBudget.Application.Features.BudgetCategories.Command
{
    public class UpdateBudgetedAmountValidFrom
    {
        public class Command : IRequest<Result>
        {
            public BudgetedAmountId BudgetedAmountId { get; set; }
            public DateTime ValidFrom { get; set; }
        }

        public class Result: SingleResponse<DateTime>
        {
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly IWriteDbContext _writeDbContext;
            private readonly AccessControlService _accessControlService;

            public Handler(IWriteDbContext writeDbContext, AccessControlService accessControlService)
            {
                _writeDbContext = writeDbContext;
                _accessControlService = accessControlService;
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

                budgetedAmount.SetValidFromDate(request.ValidFrom);
                await _writeDbContext.SaveChangesAsync(cancellationToken);

                return new Result()
                       {
                           Data = request.ValidFrom
                       };
            }
        }
    }
}