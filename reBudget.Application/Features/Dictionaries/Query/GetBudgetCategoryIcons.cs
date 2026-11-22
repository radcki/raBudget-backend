using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Application.Infrastructure;
using raBudget.Common.Response;
using raBudget.Domain.Interfaces;
using raBudget.Domain.ValueObjects;

namespace raBudget.Application.Features.Dictionaries.Query
{
    public class GetBudgetCategoryIcons
    {
        public class Query : IRequest<Result>
        {
        }

        public class Result : CollectionResponse<BudgetCategoryIconDto>
        {
        }

        public class BudgetCategoryIconDto
        {
            public BudgetCategoryIconId BudgetCategoryIconId { get; set; }
            public string IconKey { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly IReadDbContext _readDb;

            public Handler(IReadDbContext readDb)
            {
                _readDb = readDb;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var data = _readDb.BudgetCategoryIcons.ToBudgetCategoryIconDtos();

                return new Result()
                       {
                           Data = await data.ToListAsync(cancellationToken),
                           Total = data.Count()
                       };
            }
        }
    }
}