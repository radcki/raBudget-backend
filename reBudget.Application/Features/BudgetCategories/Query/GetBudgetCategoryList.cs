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
using raBudget.Common.Response;
using raBudget.Domain.Enums;
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
            public List<eBudgetCategoryType> BudgetCategoryTypes { get; set; }
            public bool IncludeHidden { get; set; } = false;
        }

        public class Result : CollectionResponse<BudgetCategoryDto>
        {
        }

        public class BudgetCategoryDto
        {
            public BudgetCategoryId BudgetCategoryId { get; set; }
            public string BudgetCategoryIconKey { get; set; }
            public BudgetCategoryIconId BudgetCategoryIconId { get; set; }
            public BudgetId BudgetId { get; set; }
            public eBudgetCategoryType BudgetCategoryType { get; set; }
            public int Order { get; set; }
            public string Name { get; set; }
            public bool Hidden { get; set; }
            public MoneyAmount CurrentBudgetedAmount { get; set; }
            public List<BudgetedAmountDto> BudgetedAmounts { get; set; }
        }

        public class BudgetedAmountDto
        {
            public BudgetedAmountId BudgetedAmountId { get; set; }
            public DateTime ValidFrom { get; set; }
            public DateTime? ValidTo { get; set; }
            public MoneyAmount Amount { get; set; }
        }

        public class Mapper : IHaveCustomMapping
        {
            public void CreateMappings(Profile configuration)
            {
                configuration.CreateMap<Domain.ReadModels.BudgetCategory.BudgetedAmount, BudgetedAmountDto>();
                configuration.CreateMap<Domain.ReadModels.BudgetCategory, BudgetCategoryDto>()
                             .ForMember(dest => dest.BudgetedAmounts, opt => opt.MapFrom(src => src.BudgetedAmounts.OrderBy(x => x.ValidFrom)));
            }
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly IReadDbContext _readDb;
            private readonly IMapper _mapper;
            private readonly AccessControlService _accessControlService;

            public Handler(IReadDbContext readDb,AccessControlService accessControlService, IMapper mapper)
            {
                _readDb = readDb;
                _accessControlService = accessControlService;
                _mapper = mapper;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var budgetCategoryIds = _accessControlService.GetAccessibleBudgetCategoryIds(request.BudgetId);

                var query = _readDb.BudgetCategories
                                   .Where(x => budgetCategoryIds.Contains(x.BudgetCategoryId) && x.BudgetId == request.BudgetId);

                if (request.BudgetCategoryTypes != null && request.BudgetCategoryTypes.Any())
                {
                    query = query.Where(x => request.BudgetCategoryTypes.Contains(x.BudgetCategoryType));
                }

                if (!request.IncludeHidden)
                {
                    query = query.Where(x => x.Hidden != true);
                }

                var budgetCategories = await query.ToListAsync(cancellationToken: cancellationToken);

                var dtos = _mapper.Map<List<BudgetCategoryDto>>(budgetCategories);

                return new Result()
                       {
                           Data = dtos.Select(x =>
                                              {
                                                  x.BudgetedAmounts = x.BudgetedAmounts.OrderBy(s => s.ValidFrom).ToList();
                                                  return x;
                                              })
                                      .OrderBy(x => x.Order)
                                      .ToList(),
                           Total = budgetCategories.Count()
                       };
            }
        }
    }
}