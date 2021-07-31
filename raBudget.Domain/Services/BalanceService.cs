using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using raBudget.Common.Extensions;
using raBudget.Common.Resources;
using raBudget.Domain.Enums;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Interfaces;
using raBudget.Domain.ReadModels;
using raBudget.Domain.ValueObjects;
using RLib.Localization;
using BudgetBalance = raBudget.Domain.Entities.BudgetBalance;
using BudgetCategoryBalance = raBudget.Domain.Entities.BudgetCategoryBalance;

namespace raBudget.Domain.Services
{
    public class BalanceService
    {
        private readonly IReadDbContext _readDb;
        private readonly IWriteDbContext _writeDb;
        private readonly IServiceProvider _serviceProvider;

        public BalanceService(IReadDbContext readDb, IWriteDbContext writeDb, IServiceProvider serviceProvider)
        {
            _readDb = readDb;
            _writeDb = writeDb;
            _serviceProvider = serviceProvider;
        }

        public async Task CalculateBudgetBalance(BudgetId budgetId, CancellationToken cancellationToken)
        {
            var currency = (await _writeDb.Budgets.FirstOrDefaultAsync(x => x.BudgetId == budgetId, cancellationToken: cancellationToken)).Currency;

            var incomeCategoryIds = _writeDb.BudgetCategories
                                            .Where(x => x.BudgetId == budgetId && x.BudgetCategoryType == eBudgetCategoryType.Income)
                                            .Select(x => x.BudgetCategoryId);

            var incomeSum = _writeDb.Transactions
                                    .Include(x => x.SubTransactions)
                                    .Where(x => incomeCategoryIds.Any(s => s == x.BudgetCategoryId))
                                    .Sum(x => x.Amount.Amount + x.SubTransactions.Sum(s => s.Amount.Amount));

            var spendingCategoryIds = _writeDb.BudgetCategories
                                              .Where(x => x.BudgetId == budgetId && x.BudgetCategoryType == eBudgetCategoryType.Spending)
                                              .Select(x => x.BudgetCategoryId);

            var spendingSum = _writeDb.Transactions
                                      .Include(x => x.SubTransactions)
                                      .Where(x => spendingCategoryIds.Any(s => s == x.BudgetCategoryId))
                                      .Sum(x => x.Amount.Amount + x.SubTransactions.Sum(s => s.Amount.Amount));

            var savingCategoryIds = _writeDb.BudgetCategories
                                            .Where(x => x.BudgetId == budgetId && x.BudgetCategoryType == eBudgetCategoryType.Saving)
                                            .Select(x => x.BudgetCategoryId);

            var savingSum = _writeDb.Transactions
                                    .Include(x => x.SubTransactions)
                                    .Where(x => savingCategoryIds.Any(s => s == x.BudgetCategoryId))
                                    .Sum(x => x.Amount.Amount + x.SubTransactions.Sum(s => s.Amount.Amount));

            var storedBalance = await _writeDb.BudgetBalances.FirstOrDefaultAsync(x => x.BudgetId == budgetId, cancellationToken);
            if (storedBalance == null)
            {
                storedBalance = BudgetBalance.Create(budgetId);
                _writeDb.BudgetBalances.Add(storedBalance);
            }

            var totalBalance = new MoneyAmount(currency.CurrencyCode, incomeSum - spendingSum - savingSum);
            var incomeTotal = new MoneyAmount(currency.CurrencyCode, incomeSum);
            var spendingTotal = new MoneyAmount(currency.CurrencyCode, spendingSum);
            var savingTotal = new MoneyAmount(currency.CurrencyCode, savingSum);

            var categoriesBudgetedLeftoverTotal = 0m;
            var budgetCategoryIds = _writeDb.BudgetCategories
                                            .Where(x => x.BudgetId == budgetId && x.BudgetCategoryType == eBudgetCategoryType.Spending)
                                            .Select(x => x.BudgetCategoryId)
                                            .ToList();
            foreach (var budgetCategoryId in budgetCategoryIds)
            {
                categoriesBudgetedLeftoverTotal += Math.Max(0, (await GetCategoryBalance(budgetCategoryId, null, null, cancellationToken)).Amount);
            }

            var unassignedFunds = new MoneyAmount(currency.CurrencyCode, totalBalance.Amount - categoriesBudgetedLeftoverTotal);

            storedBalance.Update(totalBalance, unassignedFunds, spendingTotal, incomeTotal, savingTotal);

            await _writeDb.SaveChangesAsync(cancellationToken);
        }

        public async Task<Domain.ReadModels.BudgetBalance> GetBudgetBalance(BudgetId budgetId, CancellationToken cancellationToken)
        {
            var storedBalance = await _readDb.BudgetBalances.FirstOrDefaultAsync(x => x.BudgetId == budgetId, cancellationToken);
            if (storedBalance == null || storedBalance.UpdateTime.Date.StartOfMonth() != DateTime.Today.StartOfMonth())
            {
                await CalculateBudgetBalance(budgetId, cancellationToken);
                storedBalance = await _readDb.BudgetBalances.FirstOrDefaultAsync(x => x.BudgetId == budgetId, cancellationToken);
            }

            return storedBalance;
        }

        private IWriteDbContext GetNewWriteDbContext()
        {
            return (IWriteDbContext) _serviceProvider.GetService(typeof(IWriteDbContext));
        }

        public async Task CalculateBudgetCategoryBalance(BudgetCategoryId budgetCategoryId, CancellationToken cancellationToken)
        {
            if (_writeDb.BudgetCategoryBalances.Any(x => x.BudgetCategoryId == budgetCategoryId))
            {
                _writeDb.BudgetCategoryBalances.RemoveRange(_writeDb.BudgetCategoryBalances.Where(x => x.BudgetCategoryId == budgetCategoryId));
                await _writeDb.SaveChangesAsync(cancellationToken);
            }

            var budgetCategory = _readDb.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == budgetCategoryId);

            if (budgetCategory == null || !budgetCategory.BudgetedAmounts.Any())
            {
                return;
            }

            var from = budgetCategory.BudgetedAmounts.Min(x => x.ValidFrom);
            var to = new[] {budgetCategory.BudgetedAmounts.Max(x => x.ValidFrom), new DateTime(DateTime.Today.Year, 12, 1)}.Max();

            var getTransactionsTask = GetNewWriteDbContext().Transactions.Include(x => x.SubTransactions).Where(x => x.BudgetCategoryId == budgetCategoryId).ToListAsync(cancellationToken);
            var getAllocationsTask = GetNewWriteDbContext().Allocations.Where(x => (x.TargetBudgetCategoryId == budgetCategoryId || x.SourceBudgetCategoryId == budgetCategoryId)).ToListAsync(cancellationToken);
            Task.WaitAll(getTransactionsTask, getAllocationsTask);

            var transactions = getTransactionsTask.Result;
            var allocations = getAllocationsTask.Result;

            foreach (var month in DateTimeExtensions.MonthRange(from, to))
            {
                var currency = budgetCategory.Currency;
                decimal budgetedAmount = 0;
                var categoryBudgetedAmount = budgetCategory.BudgetedAmounts
                                                           .FirstOrDefault(x => x.ValidFrom.Date <= month && (x.ValidTo == null || x.ValidTo.Value.Date >= month));
                if (categoryBudgetedAmount != null)
                {
                    budgetedAmount += categoryBudgetedAmount.Amount.Amount;
                }

                var date = new DateTime(month.Year, month.Month, 1);
                var endOfMonth = date.EndOfMonth();

                var balance = BudgetCategoryBalance.Create(budgetCategoryId, month.Year, month.Month);
                _writeDb.BudgetCategoryBalances.Add(balance);
                

                var transactionsSum = transactions.Where(x => x.TransactionDate >= date && x.TransactionDate <= endOfMonth)
                                                  .Sum(x => x.Amount.Amount + x.SubTransactions.Sum(s => s.Amount.Amount));

                var monthAllocations = allocations
                                      .Where(x => x.AllocationDate >= date && x.AllocationDate <= endOfMonth)
                                      .ToList();

                var targetAllocationsSum = monthAllocations.Where(x => x.TargetBudgetCategoryId == budgetCategoryId)
                                                           .Sum(x => x.Amount.Amount);

                var sourceAllocationsSum = monthAllocations.Where(x => x.SourceBudgetCategoryId == budgetCategoryId)
                                                           .Sum(x => x.Amount.Amount);


                balance.Update(new MoneyAmount(currency.CurrencyCode, budgetedAmount),
                               new MoneyAmount(currency.CurrencyCode, transactionsSum),
                               new MoneyAmount(currency.CurrencyCode, targetAllocationsSum - sourceAllocationsSum));

                //await CalculateBudgetCategoryBalance(budgetCategory, month.Year, month.Month, CancellationToken.None);
            }

            await _writeDb.SaveChangesAsync(cancellationToken);
        }

        public async Task<BudgetCategoryBalance> CalculateBudgetCategoryBalance(BudgetCategoryId budgetCategoryId, int year, int month, CancellationToken cancellationToken)
        {
            return await CalculateBudgetCategoryBalance(_readDb.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == budgetCategoryId), year, month, cancellationToken);
        }

        public async Task<BudgetCategoryBalance> CalculateBudgetCategoryBalance(BudgetCategory budgetCategory, int year, int month, CancellationToken cancellationToken)
        {
            var budgetCategoryId = budgetCategory.BudgetCategoryId;
            var currency = budgetCategory.Currency;
            // var currency = (await _writeDb.Budgets.FirstOrDefaultAsync(x => x.BudgetId == budgetCategory.BudgetId, cancellationToken: cancellationToken)).Currency;

            var storedBalance = _writeDb.BudgetCategoryBalances.FirstOrDefault(x => x.Month == month && x.Year == year && x.BudgetCategoryId == budgetCategoryId);
            if (storedBalance == null)
            {
                storedBalance = BudgetCategoryBalance.Create(budgetCategoryId, year, month);
                _writeDb.BudgetCategoryBalances.Add(storedBalance);
            }

            decimal budgetedAmount = 0;
            var date = new DateTime(year, month, 1);
            var endOfMonth = date.EndOfMonth();
            var categoryBudgetedAmount = budgetCategory.BudgetedAmounts
                                                       .FirstOrDefault(x => x.ValidFrom.Date <= date && (x.ValidTo == null || x.ValidTo.Value.Date >= date));
            if (categoryBudgetedAmount != null)
            {
                budgetedAmount += categoryBudgetedAmount.Amount.Amount;
            }

            var transactionsSum = _writeDb.Transactions
                                          .Include(x => x.SubTransactions)
                                          .Where(x => x.BudgetCategoryId == budgetCategoryId && x.TransactionDate >= date && x.TransactionDate <= endOfMonth)
                                          .Sum(x => x.Amount.Amount + x.SubTransactions.Sum(s => s.Amount.Amount));

            var allocations = _writeDb.Allocations
                                      .Where(x => (x.TargetBudgetCategoryId == budgetCategoryId || x.SourceBudgetCategoryId == budgetCategoryId)
                                                  && x.AllocationDate >= date
                                                  && x.AllocationDate <= endOfMonth)
                                      .ToList();

            var targetAllocationsSum = allocations.Where(x => x.TargetBudgetCategoryId == budgetCategoryId)
                                                  .Sum(x => x.Amount.Amount);

            var sourceAllocationsSum = allocations.Where(x => x.SourceBudgetCategoryId == budgetCategoryId)
                                                  .Sum(x => x.Amount.Amount);

            storedBalance.Update(new MoneyAmount(currency.CurrencyCode, budgetedAmount),
                                 new MoneyAmount(currency.CurrencyCode, transactionsSum),
                                 new MoneyAmount(currency.CurrencyCode, targetAllocationsSum - sourceAllocationsSum));

            await _writeDb.SaveChangesAsync(cancellationToken);
            return storedBalance;
        }

        public async Task<MoneyAmount> GetCategoryBudgetedAmount(BudgetCategoryId budgetCategoryId, DateTime? from, DateTime? to, CancellationToken cancellationToken)
        {
            var budgetCategory = _readDb.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == budgetCategoryId)
                                 ?? throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetCategoryNotFound));
            var currency = (await _readDb.Budgets.FirstOrDefaultAsync(x => x.BudgetId == budgetCategory.BudgetId, cancellationToken: cancellationToken)).Currency;

            var amount = 0m;
            foreach (var balance in GetCategoryBalances(budgetCategoryId, from, to))
            {
                amount += balance.BudgetedAmount.Amount;
            }

            return new MoneyAmount(currency.CurrencyCode, amount);
        }

        public async Task<MoneyAmount> GetCategoryBalance(BudgetCategoryId budgetCategoryId, DateTime? from, DateTime? to, CancellationToken cancellationToken)
        {
            var budgetCategory = _writeDb.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == budgetCategoryId)
                                 ?? throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetCategoryNotFound));
            var currency = (await _writeDb.Budgets.FirstOrDefaultAsync(x => x.BudgetId == budgetCategory.BudgetId, cancellationToken: cancellationToken)).Currency;

            var amount = 0m;
            foreach (var balance in GetCategoryBalances(budgetCategoryId, from, to))
            {
                amount += balance.BudgetedAmount.Amount - balance.TransactionsTotal.Amount + balance.AllocationsTotal.Amount;
            }

            return new MoneyAmount(currency.CurrencyCode, amount);
        }

        public ReadModels.TotalBudgetCategoryBalance GetTotalCategoryBalance(BudgetCategoryId budgetCategoryId)
        {
            var endDate = new DateTime(DateTime.Today.Year, 12, 1);
            var categoryBalance = new TotalBudgetCategoryBalance()
                                  {
                                      BudgetCategoryId = budgetCategoryId
                                  };

            foreach (var budgetCategoryBalance in GetCategoryBalances(budgetCategoryId, null, endDate))
            {
                if (categoryBalance.TotalCategoryBalance == null)
                {
                    categoryBalance.TotalCategoryBalance = (budgetCategoryBalance.BudgetedAmount + budgetCategoryBalance.AllocationsTotal) - budgetCategoryBalance.TransactionsTotal;
                }
                else if (budgetCategoryBalance.Year < DateTime.Today.Year
                         || (budgetCategoryBalance.Year == DateTime.Today.Year && budgetCategoryBalance.Month <= DateTime.Today.Month))
                {
                    categoryBalance.TotalCategoryBalance += (budgetCategoryBalance.BudgetedAmount + budgetCategoryBalance.AllocationsTotal) - budgetCategoryBalance.TransactionsTotal;
                }

                if (categoryBalance.BudgetLeftToEndOfYear == null)
                {
                    categoryBalance.BudgetLeftToEndOfYear = (budgetCategoryBalance.BudgetedAmount + budgetCategoryBalance.AllocationsTotal) - budgetCategoryBalance.TransactionsTotal;
                }
                else
                {
                    categoryBalance.BudgetLeftToEndOfYear += (budgetCategoryBalance.BudgetedAmount + budgetCategoryBalance.AllocationsTotal) - budgetCategoryBalance.TransactionsTotal;
                }

                if (budgetCategoryBalance.Month == DateTime.Today.Month && budgetCategoryBalance.Year == DateTime.Today.Year)
                {
                    categoryBalance.ThisMonthTransactionsTotal = budgetCategoryBalance.TransactionsTotal;
                    categoryBalance.ThisMonthBudgetedAmount = budgetCategoryBalance.BudgetedAmount;
                }

                if (budgetCategoryBalance.Year == DateTime.Today.Year)
                {
                    if (categoryBalance.ThisYearBudgetedAmount == null)
                    {
                        categoryBalance.ThisYearBudgetedAmount = budgetCategoryBalance.BudgetedAmount;
                    }
                    else
                    {
                        categoryBalance.ThisYearBudgetedAmount += budgetCategoryBalance.BudgetedAmount;
                    }
                }

                if (!(budgetCategoryBalance.Year == DateTime.Today.Year && budgetCategoryBalance.Month > DateTime.Today.Month))
                {
                    if (categoryBalance.TotalBudgetedAmount == null)
                    {
                        categoryBalance.TotalBudgetedAmount = budgetCategoryBalance.BudgetedAmount;
                    }
                    else
                    {
                        categoryBalance.TotalBudgetedAmount += budgetCategoryBalance.BudgetedAmount;
                    }
                }
            }

            return categoryBalance;
        }

        public IEnumerable<ReadModels.BudgetCategoryBalance> GetCategoryBalances(BudgetCategoryId budgetCategoryId, DateTime? from, DateTime? to)
        {
            var budgetCategory = _readDb.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == budgetCategoryId)
                                 ?? throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetCategoryNotFound));

            if (!budgetCategory.BudgetedAmounts.Any())
            {
                yield break;
            }

            from ??= budgetCategory.BudgetedAmounts.Min(x => x.ValidFrom);
            to ??= DateTime.Today;

            var fromYear = from.Value.Year;
            var fromMonth = from.Value.Month;
            var toYear = to.Value.Year;
            var toMonth = to.Value.Month;
            var balances = _readDb.BudgetCategoryBalances
                                  .Where(x => x.BudgetCategoryId == budgetCategoryId
                                              && ((x.Year == fromYear && x.Month >= fromMonth) || x.Year > fromYear)
                                              && ((x.Year == toYear && x.Month <= toMonth) || x.Year < toYear))
                                  .ToList();


            foreach (var month in DateTimeExtensions.MonthRange(from.Value, to.Value))
            {
                var categoryBalance = balances.FirstOrDefault(x => x.Year == month.Year && x.Month == month.Month);

                if (categoryBalance == null)
                {
                    CalculateBudgetCategoryBalance(budgetCategory, month.Year, month.Month, CancellationToken.None).GetAwaiter().GetResult();
                    categoryBalance = _readDb.BudgetCategoryBalances.FirstOrDefault(x => x.Year == month.Year && x.Month == month.Month);
                }

                if (categoryBalance != null)
                {
                    yield return categoryBalance;
                }
            }
        }
    }
}