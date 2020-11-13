using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using raBudget.Domain.Entities;
using raBudget.Domain.Models;

namespace raBudget.Domain.Interfaces
{
    public interface IWriteDbContext: IDisposable
    {
        DbSet<Budget> Budgets { get; }
        DbSet<BudgetCategory> BudgetCategories { get; }
        DbSet<Transaction> Transactions { get; }
        DbSet<BudgetCategoryIcon> BudgetCategoryIcons { get; }

        DatabaseFacade Database { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
