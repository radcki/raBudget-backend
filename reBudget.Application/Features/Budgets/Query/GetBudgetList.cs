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
using raBudget.Domain.Entities;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;

namespace raBudget.Application.Features.Budgets.Query
{
    public class GetBudgetList
    {
        public class Query : IRequest<Response>
        {
        }

        public class Response : CollectionResponse<BudgetDto>
        {
        }

        public class BudgetDto
        {
            public BudgetId BudgetId { get; set; }
            public DateTime StartingDate { get; set; }
            public Currency Currency { get; set; }
            public string Name { get; set; }
        }

        public class Mapper : IHaveCustomMapping
        {
            public void CreateMappings(Profile configuration)
            {
                configuration.CreateMap<Domain.ReadModels.Budget, BudgetDto>();
            }
        }

        public class Handler : IRequestHandler<Query, Response>
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

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                var budgetIds = _accessControlService.GetAccessibleBudgetIds();

                var data = _readDb.Budgets
                                  .Where(x => budgetIds.Contains(x.BudgetId))
                                  .ProjectTo<BudgetDto>(_mapperConfiguration)
                                  .OrderBy(x => x.Name);

                return new Response()
                       {
                           Data = await data.ToListAsync(cancellationToken),
                           Total = data.Count()
                       };
            }
        }
    }
}