using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Application.Infrastructure;
using raBudget.Common.Response;
using raBudget.Domain.Enums;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;

namespace raBudget.Application.Features.TransactionTemplates.Query
{
    public class GetTransactionTemplateList
    {
        public class Query : IRequest<Result>
        {
            public BudgetId BudgetId { get; set; }
            public eBudgetCategoryType? BudgetCategoryType { get; set; }

        }

        public class Result : CollectionResponse<TransactionTemplateDto>
        {
        }

        
        public class TransactionTemplateDto
        {
            public TransactionTemplateId TransactionTemplateId { get; set; }
            public BudgetCategoryId BudgetCategoryId { get; set; }
            public MoneyAmount Amount { get; set; }
            public string Description { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly IReadDbContext _readDb;
            private readonly AccessControlService _accessControlService;

            public Handler(IReadDbContext readDb, AccessControlService accessControlService)
            {
                _readDb = readDb;
                _accessControlService = accessControlService;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var budgetCategoryIdsQuery = _accessControlService.GetAccessibleBudgetCategoryIds(request.BudgetId, request.BudgetCategoryType);
               
                //var budgetCategoryIds = budgetCategoryIdsQuery.ToList();

                var query = _readDb.TransactionTemplates.Where(x => budgetCategoryIdsQuery.Contains(x.BudgetCategoryId));

                var data = await query.ToTransactionTemplateDtos()
                                      .OrderBy(x=>x.Description)
                                      .ToListAsync(cancellationToken);
                return new Result()
                       {
                           Data = data,
                           Total = query.Count()
                       };
            }
        }
    }
}