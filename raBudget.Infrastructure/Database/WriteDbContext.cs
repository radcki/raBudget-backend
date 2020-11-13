using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using raBudget.Domain.Entities;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Models;

namespace raBudget.Infrastructure.Database
{
    public class WriteDbContext : DbContext, IWriteDbContext
    {
        public WriteDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetCategory> BudgetCategories { get; }
        public DbSet<Transaction> Transactions { get; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WriteDbContext).Assembly, type => type.FullName != null && type.FullName.Contains("Configuration"));
        }
    }
}