using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Common.Interfaces;
using raBudget.Common.Response;
using raBudget.Domain.Enums;
using raBudget.Domain.Interfaces;
using raBudget.Domain.ReadModels;
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

        public class Mapper : IHaveCustomMapping
        {
            public void CreateMappings(Profile configuration)
            {
                configuration.CreateMap<BudgetCategoryIcon, BudgetCategoryIconDto>();
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
                var data = _readDb.BudgetCategoryIcons.ProjectTo<BudgetCategoryIconDto>(_mapperConfiguration);

                return new Result()
                       {
                           Data = await data.ToListAsync(cancellationToken),
                           Total = data.Count()
                       };
            }
        }
    }
}