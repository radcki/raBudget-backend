using System.Threading;
using System.Threading.Tasks;
using MediatR;
using raBudget.Common.Resources;
using raBudget.Common.Response;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;
using RLib.Localization;

namespace raBudget.Application.Features.Budgets.Query
{
    public class GetBudgetBalance
    {
        public class Query : IRequest<Response>
        {
            public BudgetId BudgetId { get; set; }
        }

        public class Response : SingleResponse<BudgetBalanceDto>
        {
        }

        public class BudgetBalanceDto
        {
            public MoneyAmount TotalBalance { get; set; }
            public MoneyAmount UnassignedFunds { get; set; }
            public MoneyAmount SpendingTotal { get; set; }
            public MoneyAmount IncomeTotal { get; set; }
            public MoneyAmount SavingTotal { get; set; }
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly AccessControlService _accessControlService;
            private readonly BalanceService _balanceService;

            public Handler(AccessControlService accessControlService, BalanceService balanceService)
            {
                _accessControlService = accessControlService;
                _balanceService = balanceService;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                if (!await _accessControlService.HasBudgetAccessAsync(request.BudgetId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetNotFound));
                }
                
                var balance = await _balanceService.GetBudgetBalance(request.BudgetId, cancellationToken);

                return new Response()
                       {
                           Data = balance == null ? null : new BudgetBalanceDto
                           {
                               TotalBalance = balance.TotalBalance,
                               UnassignedFunds = balance.UnassignedFunds,
                               SpendingTotal = balance.SpendingTotal,
                               IncomeTotal = balance.IncomeTotal,
                               SavingTotal = balance.SavingTotal
                           }
                       };
            }
        }
    }
}