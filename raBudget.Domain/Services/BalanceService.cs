using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using raBudget.Domain.Entities;
using raBudget.Domain.Enums;
using raBudget.Domain.Interfaces;
using raBudget.Domain.ValueObjects;

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
                                           .Select(x => x.BudgetCategoryId).ToList();
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

           storedBalance.Update(totalBalance, new MoneyAmount(currency.CurrencyCode, 0), spendingTotal, incomeTotal, savingTotal);

            await _writeDb.SaveChangesAsync(cancellationToken);
        }

        public Task<MoneyAmount> GetBudgetBalance(BudgetId budgetId)
        {
            return GetBudgetBalance(budgetId, CancellationToken.None);
        }
        public async Task<MoneyAmount> GetBudgetBalance(BudgetId budgetId, CancellationToken cancellationToken)
        {
            var storedBalance = await _readDb.BudgetBalances.FirstOrDefaultAsync(x => x.BudgetId == budgetId, cancellationToken);
            if (storedBalance == null)
            {
                await CalculateBudgetBalance(budgetId, cancellationToken);
                storedBalance = await _readDb.BudgetBalances.FirstOrDefaultAsync(x => x.BudgetId == budgetId, cancellationToken);
            }

            return storedBalance.TotalBalance;
        }
    }
}
