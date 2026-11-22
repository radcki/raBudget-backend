using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using raBudget.Application.Infrastructure;
using raBudget.Common.Response;
using raBudget.Domain.Entities;
using raBudget.Domain.Enums;
using raBudget.Domain.Interfaces;
using raBudget.Domain.ValueObjects;

namespace raBudget.Application.Features.Budget.Command
{
    public class CreateBudget
    {
        public class Command : IApplicationRequest<Result>
        {
            public string Name { get; set; }
            public DateTime StartingDate { get; set; }
            public eCurrencyCode CurrencyCode { get; set; }
        }

        public class Result: IdResponse<BudgetId>
        {
        }

        public class Handler : ICommandHandler<Command, Result>
        {
            private readonly IUserContext _userContext;
            private readonly IWriteDbContext _writeDbContext;

            public Handler(IUserContext userContext, IWriteDbContext writeDbContext)
            {
                _userContext = userContext;
                _writeDbContext = writeDbContext;
            }

            public async Task<Result> ExecuteAsync(Command request, CancellationToken cancellationToken)
            {
                var budget = Domain.Entities.Budget.Create(request.Name, request.StartingDate, Currency.Get(request.CurrencyCode));
                budget.SetOwner(_userContext.UserId);
                _writeDbContext.Budgets.Add(budget);
                await _writeDbContext.SaveChangesAsync(cancellationToken);

                return new Result()
                       {
                           Id = budget.BudgetId
                       };
            }
        }
    }
}