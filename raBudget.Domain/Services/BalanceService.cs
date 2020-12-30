using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using raBudget.Common.Extensions;
using raBudget.Common.Resources;
using raBudget.Domain.Entities;
using raBudget.Domain.Enums;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Interfaces;
using raBudget.Domain.ValueObjects;
using RLib.Localization;

namespace raBudget.Domain.Services
{
    public class BalanceService
    {
        private readonly IReadDbContext _readDb;
        private readonly IWriteDbContext _writeDb;

        public BalanceService(IReadDbContext readDb, IWriteDbContext writeDb)
        {
            _readDb = readDb;
            _writeDb = writeDb;
        }

        public async Task CalculateBudgetBalance(BudgetId budgetId, CancellationToken cancellationToken)
        {
            var currency = (await _readDb.Budgets.FirstOrDefaultAsync(x => x.BudgetId == budgetId, cancellationToken: cancellationToken)).Currency;

            var incomeCategoryIds = _readDb.BudgetCategories
                                           .Where(x => x.BudgetId == budgetId && x.BudgetCategoryType == eBudgetCategoryType.Income)
                                           .Select(x => x.BudgetCategoryId);

            var incomeSum = _readDb.Transactions
                                   .Where(x => incomeCategoryIds.Any(s => s == x.BudgetCategoryId))
                                   .Sum(x => x.Amount.Amount + x.SubTransactions.Sum(s => s.Amount.Amount));

            var spendingCategoryIds = _readDb.BudgetCategories
                                             .Where(x => x.BudgetId == budgetId && x.BudgetCategoryType == eBudgetCategoryType.Spending)
                                             .Select(x => x.BudgetCategoryId);

            var spendingSum = _readDb.Transactions
                                     .Where(x => spendingCategoryIds.Any(s => s == x.BudgetCategoryId))
                                     .Sum(x => x.Amount.Amount + x.SubTransactions.Sum(s => s.Amount.Amount));

            var savingCategoryIds = _readDb.BudgetCategories
                                           .Where(x => x.BudgetId == budgetId && x.BudgetCategoryType == eBudgetCategoryType.Saving)
                                           .Select(x => x.BudgetCategoryId);

            var savingSum = _readDb.Transactions
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

            var categoriesBudgetedTotal = 0m;
            var budgetCategoryIds = _readDb.BudgetCategories
                                           .Where(x => x.BudgetId == budgetId && x.BudgetCategoryType == eBudgetCategoryType.Spending)
                                           .Select(x => x.BudgetCategoryId)
                                           .ToList();
            foreach (var budgetCategoryId in budgetCategoryIds)
            {
                categoriesBudgetedTotal += (await GetCategoryBudgetedAmount(budgetCategoryId, null, null, cancellationToken)).Amount;
            }

            var unassignedFunds = new MoneyAmount(currency.CurrencyCode, totalBalance.Amount - categoriesBudgetedTotal);

            storedBalance.Update(totalBalance, unassignedFunds, spendingTotal, incomeTotal, savingTotal);

            await _writeDb.SaveChangesAsync(cancellationToken);
        }

        public async Task<Domain.ReadModels.BudgetBalance> GetBudgetBalance(BudgetId budgetId, CancellationToken cancellationToken)
        {
            var storedBalance = await _readDb.BudgetBalances.FirstOrDefaultAsync(x => x.BudgetId == budgetId, cancellationToken);
            if (storedBalance == null)
            {
                await CalculateBudgetBalance(budgetId, cancellationToken);
                storedBalance = await _readDb.BudgetBalances.FirstOrDefaultAsync(x => x.BudgetId == budgetId, cancellationToken);
            }

            return storedBalance;
        }

        public async Task CalculateBudgetedCategoryBalance(BudgetCategoryId budgetCategoryId, int year, int month, CancellationToken cancellationToken)
        {
            var budgetCategory = _readDb.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == budgetCategoryId)
                                 ?? throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetCategoryNotFound));
            var currency = (await _readDb.Budgets.FirstOrDefaultAsync(x => x.BudgetId == budgetCategory.BudgetId, cancellationToken: cancellationToken)).Currency;

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
                                                       .FirstOrDefault(x => x.ValidFrom <= date && (x.ValidTo == null || x.ValidTo >= endOfMonth));
            if (categoryBudgetedAmount != null)
            {
                budgetedAmount += categoryBudgetedAmount.Amount.Amount;
            }

            var transactionsSum = _readDb.Transactions
                                         .Where(x => x.BudgetCategoryId == budgetCategoryId && x.TransactionDate >= date && x.TransactionDate <= endOfMonth)
                                         .Sum(x => x.Amount.Amount);

            var targetAllocationsSum = _readDb.Allocations
                                              .Where(x => x.TargetBudgetCategoryId == budgetCategoryId
                                                          && x.AllocationDate >= date
                                                          && x.AllocationDate <= endOfMonth)
                                              .Sum(x => x.Amount.Amount);

            var sourceAllocationsSum = _readDb.Allocations
                                              .Where(x => x.SourceBudgetCategoryId == budgetCategoryId
                                                          && x.AllocationDate >= date
                                                          && x.AllocationDate <= endOfMonth)
                                              .Sum(x => x.Amount.Amount);

            storedBalance.Update(new MoneyAmount(currency.CurrencyCode, budgetedAmount),
                                 new MoneyAmount(currency.CurrencyCode, transactionsSum),
                                 new MoneyAmount(currency.CurrencyCode, targetAllocationsSum - sourceAllocationsSum));

            await _writeDb.SaveChangesAsync(cancellationToken);
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
                    CalculateBudgetedCategoryBalance(budgetCategoryId, month.Year, month.Month, CancellationToken.None).GetAwaiter().GetResult();
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