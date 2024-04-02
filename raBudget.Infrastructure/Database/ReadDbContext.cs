using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using raBudget.Domain.Entities;
using raBudget.Domain.Interfaces;
using raBudget.Domain.ValueObjects;
using Allocation = raBudget.Domain.ReadModels.Allocation;
using Budget = raBudget.Domain.ReadModels.Budget;
using BudgetBalance = raBudget.Domain.ReadModels.BudgetBalance;
using BudgetCategory = raBudget.Domain.ReadModels.BudgetCategory;
using BudgetCategoryBalance = raBudget.Domain.ReadModels.BudgetCategoryBalance;
using BudgetCategoryIcon = raBudget.Domain.ReadModels.BudgetCategoryIcon;
using SubTransaction = raBudget.Domain.ReadModels.SubTransaction;
using Transaction = raBudget.Domain.ReadModels.Transaction;
using TransactionTemplate = raBudget.Domain.ReadModels.TransactionTemplate;

namespace raBudget.Infrastructure.Database
{
    public sealed class ReadDbContext : IReadDbContext, IDisposable
    {
        private readonly IWriteDbContext _db;
        private readonly IUserContext _userContext;

        public ReadDbContext(IWriteDbContext db, IUserContext userContext)
        {
            _db = db;
            _userContext = userContext;
        }

        public IQueryable<Budget> Budgets =>
            _db.Budgets.AsNoTracking()
               .Select(x => new Budget()
                            {
                                OwnerUserId = x.OwnerUserId,
                                BudgetId = x.BudgetId,
                                Currency = x.Currency,
                                Name = x.Name,
                                StartingDate = x.StartingDate,
                            });

        /// <inheritdoc />
        public IQueryable<BudgetCategory> BudgetCategories =>
            _db.BudgetCategories
               .Include(x => x.BudgetedAmounts)
               .Include(x => x.BudgetCategoryIcon)
               .AsNoTracking()
               .Select(x => new BudgetCategory()
                            {
                                BudgetCategoryId = x.BudgetCategoryId,
                                BudgetId = x.BudgetId,
                                Name = x.Name,
                                Order = x.Order,
                                BudgetCategoryIconId = x.BudgetCategoryIconId,
                                BudgetCategoryIconKey = x.BudgetCategoryIcon != null
                                                            ? x.BudgetCategoryIcon.IconKey
                                                            : null,
                                BudgetCategoryType = x.BudgetCategoryType,
                                BudgetedAmounts = x.BudgetedAmounts
                                                   .Select(s => new BudgetCategory.BudgetedAmount()
                                                                {
                                                                    Amount = s.Amount,
                                                                    BudgetedAmountId = s.BudgetedAmountId,
                                                                    ValidFrom = s.ValidFrom,
                                                                    ValidTo = s.ValidTo
                                                                })
                                                   .ToList(),
                                Hidden = x.Hidden,
                                Currency = _db.Budgets.FirstOrDefault(s => x.BudgetId == s.BudgetId).Currency
                            });

        /// <inheritdoc />
        public IQueryable<Transaction> Transactions =>
            _db.Transactions
               .AsNoTracking()
               .Include(x => x.SubTransactions)
               .Select(x => new Transaction()
                            {
                                TransactionId = x.TransactionId,
                                BudgetCategoryId = x.BudgetCategoryId,
                                Description = x.Description,
                                Amount = x.Amount,
                                CreationDateTime = x.CreationDateTime,
                                TransactionDate = x.TransactionDate,
                                SubTransactions = x.SubTransactions
                                                   .Select(s => new SubTransaction()
                                                                {
                                                                    Description = s.Description,
                                                                    Amount = s.Amount,
                                                                    CreationDateTime = s.CreationDateTime,
                                                                    ParentTransactionTransactionId = s.ParentTransactionId,
                                                                    SubTransactionId = s.SubTransactionId,
                                                                    TransactionDate = s.TransactionDate
                                                                })
                                                   .ToList()
                            });

        /// <inheritdoc />
        public IQueryable<TransactionTemplate> TransactionTemplates =>
            _db.TransactionTemplates
               .AsNoTracking()
               .Select(x => new TransactionTemplate()
                            {
                                TransactionTemplateId = x.TransactionTemplateId,
                                BudgetCategoryId = x.BudgetCategoryId,
                                Description = x.Description,
                                Amount = x.Amount,
                                CreationDateTime = x.CreationDateTime,
                            });

        /// <inheritdoc />
        public IQueryable<Allocation> Allocations =>
            _db.Allocations
               .AsNoTracking()
               .Select(x => new Allocation()
                            {
                                AllocationId = x.AllocationId,
                                TargetBudgetCategoryId = x.TargetBudgetCategoryId,
                                SourceBudgetCategoryId = x.SourceBudgetCategoryId,
                                Description = x.Description,
                                Amount = x.Amount,
                                CreationDateTime = x.CreationDateTime,
                                AllocationDate = x.AllocationDate
                            });

        /// <inheritdoc />
        public IQueryable<Currency> Currencies =>
            _db.Currencies
               .AsNoTracking()
               .OrderBy(x => x.NativeName)
               .Select(x => new Currency(x.CurrencyCode));

        public IQueryable<BudgetCategoryIcon> BudgetCategoryIcons =>
            _db.BudgetCategoryIcons
               .AsNoTracking()
               .OrderBy(x => x.IconKey)
               .Select(x => new BudgetCategoryIcon()
                            {
                                IconKey = x.IconKey,
                                BudgetCategoryIconId = x.BudgetCategoryIconId
                            });

        /// <inheritdoc />
        public IQueryable<BudgetBalance> BudgetBalances =>
            _db.BudgetBalances
               .AsNoTracking()
               .Select(x => new BudgetBalance()
                            {
                                IncomeTotal = x.IncomeTotal,
                                SavingTotal = x.SavingTotal,
                                SpendingTotal = x.SpendingTotal,
                                TotalBalance = x.TotalBalance,
                                UnassignedFunds = x.UnassignedFunds,
                                BudgetId = x.BudgetId,
                                UpdateTime = x.UpdateTime
                            });

        public IQueryable<BudgetCategoryBalance> BudgetCategoryBalances =>
            _db.BudgetCategoryBalances
               .AsNoTracking()
               .Select(x => new BudgetCategoryBalance()
                            {
                                Month = x.Month,
                                Year = x.Year,
                                BudgetCategoryId = x.BudgetCategoryId,
                                BudgetedAmount = x.BudgetedAmount,
                                TransactionsTotal = x.TransactionsTotal,
                                AllocationsTotal = x.AllocationsTotal
                            });

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}