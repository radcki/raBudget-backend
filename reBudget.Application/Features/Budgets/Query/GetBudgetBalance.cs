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
using raBudget.Domain.ReadModels;
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

        public class Mapper : IHaveCustomMapping
        {
            #region Implementation of IHaveCustomMapping

            /// <inheritdoc />
            public void CreateMappings(Profile configuration)
            {
                configuration.CreateMap<BudgetBalance, BudgetBalanceDto>();
            }

            #endregion
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly AccessControlService _accessControlService;
            private readonly BalanceService _balanceService;
            private readonly IMapper _mapper;

            public Handler(AccessControlService accessControlService, BalanceService balanceService, IMapper mapper)
            {
                _accessControlService = accessControlService;
                _balanceService = balanceService;
                _mapper = mapper;
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
                           Data = _mapper.Map<BudgetBalanceDto>(balance)
                       };
            }
        }
    }
}