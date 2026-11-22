using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using raBudget.Common.Extensions;
using raBudget.Common.Query;
using raBudget.Application.Infrastructure;
using raBudget.Common.Response;
using raBudget.Domain.Enums;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;

namespace raBudget.Application.Features.Transactions.Query
{
    public class GetTransactionList
    {
        public class Query : GridQuery, IApplicationRequest<Result>
        {
            public BudgetId BudgetId { get; set; }

            public List<BudgetCategoryId> BudgetCategoryIds { get; set; }
            public eBudgetCategoryType? BudgetCategoryType { get; set; }

            public DateTime? TransactionDateStart { get; set; }
            public DateTime? TransactionDateEnd { get; set; }

            public decimal? MinAmount { get; set; }
            public decimal? MaxAmount { get; set; }
        }

        public class Result : CollectionResponse<TransactionDto>
        {
            public MoneyAmount AmountTotal => Data.Count > 0 ? Data.Select(x => x.TotalAmount).Aggregate((amount, moneyAmount) => amount + moneyAmount) : null;
        }

        public class TransactionDto
        {
            public TransactionId TransactionId { get; set; }
            public BudgetCategoryId BudgetCategoryId { get; set; }
            public MoneyAmount Amount { get; set; }
            public string Description { get; set; }
            public DateTime TransactionDate { get; set; }
            public List<SubTransactionDto> SubTransactions { get; set; }
            public MoneyAmount TotalAmount => Amount + SubTransactions.Sum(x => x.Amount.Amount);
            public DateTime CreationDateTime { get; set; }
        }

        public class SubTransactionDto
        {
            public SubTransactionId SubTransactionId { get; set; }
            public MoneyAmount Amount { get; set; }
            public string Description { get; set; }
            public DateTime TransactionDate { get; set; }
            public DateTime CreationDateTime { get; set; }
        }

        public class Handler : ICommandHandler<Query, Result>
        {
            private readonly IReadDbContext _readDb;
            private readonly AccessControlService _accessControlService;

            public Handler(IReadDbContext readDb, AccessControlService accessControlService)
            {
                _readDb = readDb;
                _accessControlService = accessControlService;
            }

            public async Task<Result> ExecuteAsync(Query request, CancellationToken cancellationToken)
            {
                var budgetCategoryIdsQuery = _accessControlService.GetAccessibleBudgetCategoryIds(request.BudgetId, request.BudgetCategoryType);
                if (request.BudgetCategoryIds != null && request.BudgetCategoryIds.Any())
                {
                    budgetCategoryIdsQuery = budgetCategoryIdsQuery.Where(x => request.BudgetCategoryIds.Any(s => s == x));
                }

                request.DataOrder.AddDescending<TransactionDto>(x => x.CreationDateTime);

                var query = _readDb.Transactions.Where(x => budgetCategoryIdsQuery.Contains(x.BudgetCategoryId));

                if (!string.IsNullOrEmpty(request.Search))
                {
                    query = query.Where(x => x.Description.ToLower().Contains(request.Search.ToLower()));
                }

                if (request.TransactionDateStart != null)
                {
                    query = query.Where(x => x.TransactionDate >= request.TransactionDateStart.Value.Date);
                }

                if (request.TransactionDateEnd != null)
                {
                    query = query.Where(x => x.TransactionDate <= request.TransactionDateEnd.Value.Date);
                }

                if (request.MinAmount != null)
                {
                    query = query.Where(x => x.Amount.Amount >= request.MinAmount);
                }

                if (request.MaxAmount != null)
                {
                    query = query.Where(x => x.Amount.Amount <= request.MaxAmount);
                }

                var data = await query.ToTransactionDtos()
                                      .ApplyGridQueryOptions(request)
                                      .ToListAsync(cancellationToken);

                return new Result()
                       {
                           Data = data,
                           Total = query.Count(),
                           PageSize = request.PageSize
                       };
            }
        }
    }
}