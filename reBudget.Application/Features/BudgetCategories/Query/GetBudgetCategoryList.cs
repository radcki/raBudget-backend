using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Common.Interfaces;
using raBudget.Common.Response;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;

namespace raBudget.Application.Features.BudgetCategories.Query
{
    public class GetBudgetCategoryList
    {
        public class Query : IRequest<Result>
        {
            public BudgetId BudgetId { get; set; }
        }

        public class Result : CollectionResponse<BudgetCategoryDto>
        {
        }

        public class BudgetCategoryDto
        {
            public BudgetId BudgetId { get; set; }
            public int Order { get; set; }
            public string Name { get; set; }
        }

        public class Mapper : IHaveCustomMapping
        {
            public void CreateMappings(Profile configuration)
            {
                configuration.CreateMap<Domain.ReadModels.BudgetCategory, BudgetCategoryDto>();
            }
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly IReadDbContext _readDb;
            private readonly MapperConfiguration _mapperConfiguration;
            private readonly AccessControlService _accessControlService;

            public Handler(IReadDbContext readDb, MapperConfiguration mapperConfiguration, AccessControlService accessControlService)
            {
                _readDb = readDb;
                _mapperConfiguration = mapperConfiguration;
                _accessControlService = accessControlService;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var budgetCategoryIds = _accessControlService.GetAccessibleBudgetCategoryIds();

                var data = _readDb.BudgetCategories
                                  .Where(x => budgetCategoryIds.Contains(x.BudgetCategoryId) && x.BudgetId == request.BudgetId)
                                  .ProjectTo<BudgetCategoryDto>(_mapperConfiguration)
                                  .OrderBy(x => x.Order);

                return new Result()
                       {
                           Data = await data.ToListAsync(cancellationToken),
                           Total = data.Count()
                       };
            }
        }
    }
}