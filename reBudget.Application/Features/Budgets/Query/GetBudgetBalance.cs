using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Common.Interfaces;
using raBudget.Common.Resources;
using raBudget.Common.Response;
using raBudget.Domain.Enums;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Models;
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

        public class Response : SingleResponse<MoneyAmount>
        {
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
                           Data = balance
                       };
            }
        }
    }
}