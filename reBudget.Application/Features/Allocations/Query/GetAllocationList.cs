using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Common.Extensions;
using raBudget.Common.Interfaces;
using raBudget.Common.Query;
using raBudget.Common.Response;
using raBudget.Domain.Enums;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;

namespace raBudget.Application.Features.Allocations.Query
{
	public class GetAllocationList
	{
		public class Query : GridQuery, IRequest<Result>
		{
			public BudgetId BudgetId { get; set; }

			public List<BudgetCategoryId> TargetBudgetCategoryIds { get; set; }
			public eBudgetCategoryType? TargetBudgetCategoryType { get; set; }

			public List<BudgetCategoryId> SourceBudgetCategoryIds { get; set; }
			public eBudgetCategoryType? SourceBudgetCategoryType { get; set; }

			public DateTime? AllocationDateStart { get; set; }
			public DateTime? AllocationDateEnd { get; set; }

			public decimal? MinAmount { get; set; }
			public decimal? MaxAmount { get; set; }
		}

		public class Result : CollectionResponse<AllocationDto>
		{
            public MoneyAmount AmountTotal => Data.Count > 0 ? Data.Select(x => x.Amount).Aggregate((amount, moneyAmount) => amount + moneyAmount) : null;
		}

		public class AllocationDto
		{
			public AllocationId AllocationId { get; set; }
			public BudgetCategoryId TargetBudgetCategoryId { get; set; }
			public BudgetCategoryId SourceBudgetCategoryId { get; set; }
			public MoneyAmount Amount { get; set; }
			public string Description { get; set; }
			public DateTime AllocationDate { get; set; }
		}

		public class Mapper : IHaveCustomMapping
		{
			public void CreateMappings(Profile configuration)
			{
				configuration.CreateMap<Domain.ReadModels.Allocation, AllocationDto>();
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
				var targetBudgetCategoryIdsQuery = _accessControlService.GetAccessibleBudgetCategoryIds(request.BudgetId, request.TargetBudgetCategoryType);
				if (request.TargetBudgetCategoryIds != null && request.TargetBudgetCategoryIds.Any())
				{
					targetBudgetCategoryIdsQuery = targetBudgetCategoryIdsQuery.Where(x => request.TargetBudgetCategoryIds.Any(s => s == x));
				}

				var targetBudgetCategoryIds = targetBudgetCategoryIdsQuery.ToList();

				var sourceBudgetCategoryIdsQuery = _accessControlService.GetAccessibleBudgetCategoryIds(request.BudgetId, request.SourceBudgetCategoryType);
				if (request.SourceBudgetCategoryIds != null && request.SourceBudgetCategoryIds.Any())
				{
					sourceBudgetCategoryIdsQuery = sourceBudgetCategoryIdsQuery.Where(x => request.SourceBudgetCategoryIds.Any(s => s == x));
				}

				var sourceBudgetCategoryIds = sourceBudgetCategoryIdsQuery.ToList();

				var query = _readDb.Allocations.Where(x => targetBudgetCategoryIds.Contains(x.TargetBudgetCategoryId)
														   && (x.SourceBudgetCategoryId == null || sourceBudgetCategoryIds.Contains(x.SourceBudgetCategoryId)));

                if (request.SourceBudgetCategoryIds != null && request.SourceBudgetCategoryIds.Any())
                {
                    query = query.Where(x => x.SourceBudgetCategoryId != null);
                }

				if (!string.IsNullOrEmpty(request.Search))
				{
					query = query.Where(x => x.Description.ToLower().Contains(request.Search.ToLower()));
				}

				if (request.AllocationDateStart != null)
				{
					query = query.Where(x => x.AllocationDate >= request.AllocationDateStart.Value.Date);
				}

				if (request.AllocationDateEnd != null)
				{
					query = query.Where(x => x.AllocationDate <= request.AllocationDateEnd.Value.Date);
				}

				if (request.MinAmount != null)
				{
					query = query.Where(x => x.Amount.Amount >= request.MinAmount);
				}

				if (request.MaxAmount != null)
				{
					query = query.Where(x => x.Amount.Amount <= request.MaxAmount);
				}

				var data = await query.ProjectTo<AllocationDto>(_mapperConfiguration)
									  .ApplyGridQueryOptions(request)
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