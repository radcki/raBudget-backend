using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Models;
using raBudget.Domain.ReadModels;

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

        public IQueryable<Budget> Budgets => _db.Budgets.AsNoTracking()
                                                .Select(x => new Budget()
                                                             {
                                                                 OwnerUserId = x.OwnerUserId,
                                                                 BudgetId = x.BudgetId,
                                                                 Currency = x.Currency,
                                                                 Name = x.Name,
                                                                 StartingDate = x.StartingDate,
                                                             });

        /// <inheritdoc />
        public IQueryable<BudgetCategory> BudgetCategories => _db.BudgetCategories
                                                                 .AsNoTracking()
                                                                 .Select(x => new BudgetCategory()
                                                                              {
                                                                                  BudgetCategoryId = x.BudgetCategoryId,
                                                                                  BudgetId = x.BudgetId,
                                                                                  Name = x.Name,
                                                                                  BudgetCategoryType = x.BudgetCategoryType
                                                                              });

        /// <inheritdoc />
        public IQueryable<Transaction> Transactions => _db.Transactions
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
        public IQueryable<Currency> Currencies => _db.Currencies
                                                     .OrderBy(x => x.NativeName)
                                                     .Select(x => new Currency(x.CurrencyCode));

        public IQueryable<BudgetCategoryIcon> BudgetCategoryIcons => _db.BudgetCategoryIcons
                                                                        .OrderBy(x => x.IconKey)
                                                                        .Select(x => new BudgetCategoryIcon()
                                                                                     {
                                                                                         IconKey = x.IconKey,
                                                                                         BudgetCategoryIconId = x.BudgetCategoryIconId
                                                                                     });

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}