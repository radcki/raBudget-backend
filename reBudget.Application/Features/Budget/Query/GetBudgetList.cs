using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Common.Interfaces;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Models;
using raBudget.Domain.Services;

namespace raBudget.Application.Features.Budget.Query
{
    public class GetBudgetList
    {
        public class Query : IRequest<Result>
        {
        }

        public class Result
        {
            public List<BudgetDto> Data { get; set; }
            public int Total { get; set; }
        }

        public class BudgetDto
        {
            public int BudgetId { get; set; }
            public string Name { get; set; }
        }

        public class Mapper : IHaveCustomMapping
        {
            public void CreateMappings(Profile configuration)
            {
                configuration.CreateMap<Domain.ReadModels.Budget, BudgetDto>();
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
                var budgetIds = _accessControlService.GetAccessibleBudgetIds();

                var data = _readDb.Budgets
                                  .Where(x=>budgetIds.Contains(x.BudgetId))
                                  .ProjectTo<BudgetDto>(_mapperConfiguration)
                                  .OrderBy(x=>x.Name);

                return new Result()
                       {
                           Data = await data.ToListAsync(cancellationToken), 
                           Total = data.Count()
                       };
            }

        }
    }
}
