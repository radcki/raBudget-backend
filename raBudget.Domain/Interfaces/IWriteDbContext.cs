using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using raBudget.Domain.Entities;
using raBudget.Domain.Models;

namespace raBudget.Domain.Interfaces
{
    public interface IWriteDbContext : IDisposable
    {
        DbSet<Budget> Budgets { get; }
        DbSet<BudgetCategory> BudgetCategories { get; }
        DbSet<Transaction> Transactions { get; }
        DbSet<Allocation> Allocations { get; }
        DbSet<BudgetCategoryIcon> BudgetCategoryIcons { get; }
        DbSet<Currency> Currencies { get; }

        DbSet<BudgetBalance> BudgetBalances { get; }
        DbSet<BudgetCategoryBalance> BudgetCategoryBalances { get; }

        DatabaseFacade Database { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}