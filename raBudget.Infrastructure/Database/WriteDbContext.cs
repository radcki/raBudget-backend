using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using raBudget.Domain.Entities;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Models;
using raBudget.Domain.ValueObjects;
using raBudget.Infrastructure.Database.Configuration;

namespace raBudget.Infrastructure.Database
{
    public class WriteDbContext : DbContext, IWriteDbContext
    {
        public WriteDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Budget> Budgets { get; private set; }
        public DbSet<BudgetCategory> BudgetCategories { get; private set; }
        public DbSet<Transaction> Transactions { get; private set; }
        public DbSet<BudgetCategoryIcon> BudgetCategoryIcons { get; private set; }
        public DbSet<Currency> Currencies { get; private set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BudgetCategoryIconConfiguration).Assembly);
        }
    }
}