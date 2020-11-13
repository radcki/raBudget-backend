using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using raBudget.Common.Interfaces;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Models;

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

            public Handler(IReadDbContext readDb, MapperConfiguration mapperConfiguration)
            {
                _readDb = readDb;
                _mapperConfiguration = mapperConfiguration;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var data = _readDb.Budgets
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
