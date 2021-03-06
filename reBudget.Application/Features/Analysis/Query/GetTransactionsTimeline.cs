﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Common;
using raBudget.Common.Extensions;
using raBudget.Common.Response;
using raBudget.Domain.Enums;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;

namespace raBudget.Application.Features.Analysis.Query
{
    public class GetTransactionsTimeline
    {
        public class Query : IRequest<Result>
        {
            public BudgetId BudgetId { get; set; }

            public List<BudgetCategoryId> BudgetCategoryIds { get; set; }
            public eBudgetCategoryType? BudgetCategoryType { get; set; }

            public DateTime TransactionDateStart { get; set; }
            public DateTime TransactionDateEnd { get; set; }

            public eGroupingStep GroupingStep { get; set; }
        }

        public class Result : SingleResponse<ResponseDto>
        {
        }

        public enum eGroupingStep
        {
            Year,
            Month,
            Quarter,
            Week,
            Day
        }

        public class ResponseDto
        {
            public List<DateRangeData> DateRanges { get; set; } = new List<DateRangeData>();
            public List<BudgetCategoryDataPointDto> BudgetCategoryTotals { get; set; } = new List<BudgetCategoryDataPointDto>();
            public DataPointDto Total { get; set; }
        }

        public class DateRangeData
        {
            public string Key { get; set; }
            public DateRange DateRange { get; set; }
            public List<BudgetCategoryDataPointDto> BudgetCategories { get; set; } = new List<BudgetCategoryDataPointDto>();
            public DataPointDto Total { get; set; }
        }

        public class DataPointDto
        {
            public string Key { get; set; }
            public DateRange DateRange { get; set; }
            public MoneyAmount AmountTotal { get; set; }
            public MoneyAmount AmountPerDay => AmountTotal / DateRange.DayCount();
            public MoneyAmount AmountPerWeek => AmountTotal / DateRange.WeekCount(CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
            public MoneyAmount AmountPerMonth => AmountTotal / DateRange.MonthCount();

            public decimal? AmountTotalChange { get; set; }
            public decimal? AmountPerDayChange { get; set; }
            public decimal? AmountPerWeekChange { get; set; }
            public decimal? AmountPerMonthChange { get; set; }
        }

        public class BudgetCategoryDataPointDto
        {
            public BudgetCategoryId BudgetCategoryId { get; set; }
            public string Key { get; set; }
            public DateRange DateRange { get; set; }
            public MoneyAmount AmountTotal { get; set; }
            public MoneyAmount AmountPerDay => AmountTotal / DateRange.DayCount();
            public MoneyAmount AmountPerWeek => AmountTotal / DateRange.WeekCount(CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
            public MoneyAmount AmountPerMonth => AmountTotal / DateRange.MonthCount();

            public decimal? AmountTotalChange { get; set; }
            public decimal? AmountPerDayChange { get; set; }
            public decimal? AmountPerWeekChange { get; set; }
            public decimal? AmountPerMonthChange { get; set; }
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
                var budgetCategoryIdsQuery = _accessControlService.GetAccessibleBudgetCategoryIds(request.BudgetId, request.BudgetCategoryType);
                if (request.BudgetCategoryIds != null && request.BudgetCategoryIds.Any())
                    budgetCategoryIdsQuery = budgetCategoryIdsQuery.Where(x => request.BudgetCategoryIds.Any(s => s == x));

                var budgetCurrency = _readDb.Budgets.First(x => x.BudgetId == request.BudgetId).Currency;

                var budgetCategoryIds = budgetCategoryIdsQuery.ToList();
                var budgetCategories = _readDb.BudgetCategories.Where(x => budgetCategoryIds.Contains(x.BudgetCategoryId)).ToList();

                var query = _readDb.Transactions.Where(x => budgetCategoryIds.Contains(x.BudgetCategoryId));

                query = query.Where(x => x.TransactionDate >= request.TransactionDateStart.Date && x.TransactionDate <= request.TransactionDateEnd.Date);


                var transactions = await query.ToListAsync(cancellationToken);

                if (!transactions.Any())
                    return new Result();

                var timelinePeriod = new DateRange(request.TransactionDateStart, request.TransactionDateEnd);

                var periods = request.GroupingStep switch
                {
                    eGroupingStep.Day => timelinePeriod.Start.EachDayTo(timelinePeriod.End),
                    eGroupingStep.Year => timelinePeriod.Start.EachYearTo(timelinePeriod.End),
                    eGroupingStep.Month => timelinePeriod.Start.EachMonthTo(timelinePeriod.End),
                    eGroupingStep.Quarter => timelinePeriod.Start.EachQuarterTo(timelinePeriod.End),
                    eGroupingStep.Week => timelinePeriod.Start.EachWeekTo(timelinePeriod.End, CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek),
                    _ => throw new ArgumentOutOfRangeException()
                };

                var dateRanges = transactions.GroupBy(x => periods.FirstOrDefault(s => s.Contains(x.TransactionDate)))
                                             .Select(periodGroup =>
                                                     {
                                                         var period = periodGroup.Key;

                                                         var categoryDataPoints = budgetCategories.Select(category =>
                                                                                                          {
                                                                                                              var categoryId = category.BudgetCategoryId;
                                                                                                              var categoryAmount = periodGroup.Where(s => s.BudgetCategoryId == categoryId)
                                                                                                                                              .Select(s => s.Amount + s.SubTransactions.Select(t => t.Amount).Aggregate(new MoneyAmount(budgetCurrency.CurrencyCode, 0), (a, b) => a + b))
                                                                                                                                              .Aggregate(new MoneyAmount(budgetCurrency.CurrencyCode, 0), (a, b) => a + b);

                                                                                                              return new BudgetCategoryDataPointDto()
                                                                                                                     {
                                                                                                                         DateRange = period,
                                                                                                                         BudgetCategoryId = categoryId,
                                                                                                                         Key = period.ToString(),
                                                                                                                         AmountTotal = categoryAmount
                                                                                                                     };
                                                                                                          })
                                                                                                  .ToList();

                                                         var totalAmount = categoryDataPoints.Select(x => x.AmountTotal)
                                                                                             .Aggregate(new MoneyAmount(budgetCurrency.CurrencyCode, 0), (a, b) => a + b);

                                                         return new DateRangeData()
                                                                {
                                                                    DateRange = period,
                                                                    Key = period.ToString(),
                                                                    BudgetCategories = categoryDataPoints,
                                                                    Total = new DataPointDto()
                                                                            {
                                                                                DateRange = period,
                                                                                Key = period.ToString(),
                                                                                AmountTotal = totalAmount
                                                                            }
                                                                };
                                                     })
                                             .OrderByDescending(x => x.DateRange.Start)
                                             .ToList();

                if (dateRanges.Count > 1)
                {
                    for (var i = 0; i < dateRanges.Count-1; i++)
                    {
                        var current = dateRanges[i];
                        var previous = dateRanges[i + 1];

                        foreach (var budgetCategoryDataPoint in current.BudgetCategories)
                        {
                            var previousBudgetCategoryDataPoint = previous.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == budgetCategoryDataPoint.BudgetCategoryId);
                            if (previousBudgetCategoryDataPoint == null)
                            {
                                continue;
                                ;
                            }

                            budgetCategoryDataPoint.AmountTotalChange = previousBudgetCategoryDataPoint.AmountTotal.Amount > 0
                                                                            ? (budgetCategoryDataPoint.AmountTotal.Amount - previousBudgetCategoryDataPoint.AmountTotal.Amount) / previousBudgetCategoryDataPoint.AmountTotal.Amount
                                                                            : (decimal?) null;

                            budgetCategoryDataPoint.AmountPerDayChange = previousBudgetCategoryDataPoint.AmountPerDay.Amount > 0
                                                                             ? (budgetCategoryDataPoint.AmountPerDay.Amount - previousBudgetCategoryDataPoint.AmountPerDay.Amount) / previousBudgetCategoryDataPoint.AmountPerDay.Amount
                                                                             : (decimal?) null;

                            budgetCategoryDataPoint.AmountPerWeekChange = previousBudgetCategoryDataPoint.AmountPerWeek.Amount > 0
                                                                              ? (budgetCategoryDataPoint.AmountPerWeek.Amount - previousBudgetCategoryDataPoint.AmountPerWeek.Amount) / previousBudgetCategoryDataPoint.AmountPerWeek.Amount
                                                                              : (decimal?) null;

                            budgetCategoryDataPoint.AmountPerMonthChange = previousBudgetCategoryDataPoint.AmountPerMonth.Amount > 0
                                                                               ? (budgetCategoryDataPoint.AmountPerMonth.Amount - previousBudgetCategoryDataPoint.AmountPerMonth.Amount) / previousBudgetCategoryDataPoint.AmountPerMonth.Amount
                                                                               : (decimal?) null;
                        }

                        current.Total.AmountTotalChange = previous.Total.AmountTotal.Amount > 0
                                                              ? (current.Total.AmountTotal.Amount - previous.Total.AmountTotal.Amount) / previous.Total.AmountTotal.Amount
                                                              : (decimal?) null;
                        current.Total.AmountPerDayChange = previous.Total.AmountTotal.Amount > 0
                                                               ? (current.Total.AmountPerDay.Amount - previous.Total.AmountPerDay.Amount) / previous.Total.AmountPerDay.Amount
                                                               : (decimal?) null;
                        current.Total.AmountPerWeekChange = previous.Total.AmountTotal.Amount > 0
                                                                ? (current.Total.AmountPerWeek.Amount - previous.Total.AmountPerWeek.Amount) / previous.Total.AmountPerWeek.Amount
                                                                : (decimal?) null;
                        current.Total.AmountPerMonthChange = previous.Total.AmountTotal.Amount > 0
                                                                 ? (current.Total.AmountPerMonth.Amount - previous.Total.AmountPerMonth.Amount) / previous.Total.AmountPerMonth.Amount
                                                                 : (decimal?) null;
                    }
                }

                var budgetCategoryTotals = dateRanges.SelectMany(x => x.BudgetCategories)
                                                     .GroupBy(x => x.BudgetCategoryId)
                                                     .Select(x => new BudgetCategoryDataPointDto()
                                                                  {
                                                                      Key = timelinePeriod.ToString(),
                                                                      DateRange = timelinePeriod,
                                                                      BudgetCategoryId = x.Key,
                                                                      AmountTotal = x.Select(s => s.AmountTotal)
                                                                                     .Aggregate(new MoneyAmount(budgetCurrency.CurrencyCode, 0), (a, b) => a + b)
                                                                  })
                                                     .ToList();
                var total = new DataPointDto()
                            {
                                DateRange = timelinePeriod,
                                Key = timelinePeriod.ToString(),
                                AmountTotal = budgetCategoryTotals.Select(x => x.AmountTotal)
                                                                  .Aggregate(new MoneyAmount(budgetCurrency.CurrencyCode, 0), (a, b) => a + b)
                            };

                return new Result()
                       {
                           Data = new ResponseDto()
                                  {
                                      DateRanges = dateRanges,
                                      BudgetCategoryTotals = budgetCategoryTotals,
                                      Total = total
                                  }
                       };
            }
        }
    }
}