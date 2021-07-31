using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Application.Features.BudgetCategories.Query;
using raBudget.Common.Extensions;
using raBudget.Common.Interfaces;
using raBudget.Common.Resources;
using raBudget.Common.Response;
using raBudget.Domain.Entities;
using raBudget.Domain.Enums;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;
using RLib.Localization;

namespace raBudget.Application.Features.BudgetCategories.Command
{
    public class AddBudgetedAmount
    {
        public class Command : IRequest<Result>
        {
            public BudgetCategoryId BudgetCategoryId { get; set; }
            public MoneyAmount Amount { get; set; }
            public DateTime ValidFrom { get; set; }
        }

        public class Result : SingleResponse<BudgetCategoryDto>
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

        public class Notification : INotification
		{
			public BudgetCategory ReferenceBudgetCategory { get; set; }
            public BudgetCategory.BudgetedAmount ReferenceBudgetedAmount { get; set; }
		}

        public class Mapper : IHaveCustomMapping
        {
            public void CreateMappings(Profile configuration)
            {
                configuration.CreateMap<BudgetCategory.BudgetedAmount, BudgetedAmountDto>();
                configuration.CreateMap<BudgetCategory, BudgetCategoryDto>()
                             .ForMember(dest => dest.BudgetedAmounts, opt => opt.MapFrom(src => src.BudgetedAmounts.OrderBy(x => x.ValidFrom)));
            }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly IWriteDbContext _writeDbContext;
            private readonly AccessControlService _accessControlService;
			private readonly IMediator _mediator;
            private readonly IMapper _mapper;

            public Handler(IWriteDbContext writeDbContext, AccessControlService accessControlService, IMediator mediator, IMapper mapper)
            {
                _writeDbContext = writeDbContext;
                _accessControlService = accessControlService;
				_mediator = mediator;
                _mapper = mapper;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                if (!await _accessControlService.HasBudgetCategoryAccessAsync(request.BudgetCategoryId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetCategoryNotFound));
                }

                var budgetCategory = await _writeDbContext.BudgetCategories
                                                          .Include(x=>x.BudgetedAmounts)
                                                          .FirstOrDefaultAsync(x => x.BudgetCategoryId == request.BudgetCategoryId, cancellationToken);

                var budgetedAmount = budgetCategory.AddBudgetedAmount(request.Amount, request.ValidFrom.StartOfMonth());

                await _writeDbContext.SaveChangesAsync(cancellationToken);

				_=_mediator.Publish(new Notification()
				{
					ReferenceBudgetCategory = budgetCategory,
					ReferenceBudgetedAmount = budgetedAmount
				}, cancellationToken);

                return new Result()
                       {
                           Data = _mapper.Map<BudgetCategoryDto>(budgetCategory)
                       };
            }
        }
    }
}