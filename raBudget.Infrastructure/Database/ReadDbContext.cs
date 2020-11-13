using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using raBudget.Domain.Interfaces;
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
                                                                 BudgetId = x.BudgetId.Value,
                                                                 Currency = x.Currency,
                                                                 Name = x.Name,
                                                                 StartingDate = x.StartingDate,
                                                             });

        /// <inheritdoc />
        public IQueryable<BudgetCategory> BudgetCategories => _db.BudgetCategories
                                                                 .AsNoTracking()
                                                                 .Select(x=> new BudgetCategory()
                                                                             {
                                                                                 BudgetCategoryId = x.BudgetCategoryId.Value,
                                                                                 BudgetId = x.BudgetId.Value,
                                                                                 Name = x.Name,
                                                                                 BudgetCategoryType = x.BudgetCategoryType
                                                                             });


        public void Dispose()
        {
            _db.Dispose();
        }
    }
}