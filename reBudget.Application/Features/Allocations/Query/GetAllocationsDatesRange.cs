using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Common.Response;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;

namespace raBudget.Application.Features.Allocations.Query
{
    public class GetAllocationsDatesRange
    {
        public class Query : IRequest<Result>
        {
            public BudgetId BudgetId { get; set; }
        }

        public class Result : SingleResponse<DatesRangeDto>
        {
        }

        public class DatesRangeDto
        {
            public DateTime? MinDate { get; set; }
            public DateTime? MaxDate { get; set; }
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
                var budgetCategoryIds = _accessControlService.GetAccessibleBudgetCategoryIds(request.BudgetId).ToList();
                var query = _readDb.Allocations.Where(x => budgetCategoryIds.Any(s => s == x.TargetBudgetCategoryId));
                DateTime? min = null;
                DateTime? max = null;
                if (query.Any())
                {
                    min = (await query.MinAsync(x => x.AllocationDate, cancellationToken)).Date;
                    max = (await query.MaxAsync(x => x.AllocationDate, cancellationToken)).Date;
                }

                return new Result()
                       {
                           Data = new DatesRangeDto()
                                  {
                                      MaxDate = max,
                                      MinDate = min
                                  }
                       };
            }
        }
    }
}