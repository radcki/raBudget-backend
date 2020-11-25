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
    public class MoveBudgetCategoryUp
    {
        public class Command : IRequest<Result>
        {
            public BudgetCategoryId BudgetCategoryId { get; set; }
        }

        public class Result : BaseResponse
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
                if (!await _accessControlService.HasBudgetCategoryAccessAsync(request.BudgetCategoryId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetCategoryNotFound));
                }

                var budget = await _writeDbContext.Budgets
                                                  .Include(x => x.BudgetCategories)
                                                  .FirstAsync(x => x.BudgetCategories.Any(s => s.BudgetCategoryId == request.BudgetCategoryId), cancellationToken);

                budget.BudgetCategories.MoveUp(budget.BudgetCategories.First(x => x.BudgetCategoryId == request.BudgetCategoryId));

                await _writeDbContext.SaveChangesAsync(cancellationToken);

                return new Result() { };
            }
        }
    }
}