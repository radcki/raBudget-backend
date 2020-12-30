using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using raBudget.Domain.Entities;
using raBudget.Domain.Models;
using raBudget.Domain.ValueObjects;

namespace raBudget.Infrastructure.Database.Configuration
{
    public class BudgetCategoryBalanceConfiguration : IEntityTypeConfiguration<BudgetCategoryBalance>
    {
        public void Configure(EntityTypeBuilder<BudgetCategoryBalance> builder)
        {
            builder.HasKey(x => new {x.Year, x.Month, x.BudgetCategoryId});
            builder.Property(x => x.BudgetCategoryId).HasColumnType("char(36)").HasConversion<Guid>(x => x.Value, i => new BudgetCategoryId(i));

            builder.OwnsOne(typeof(MoneyAmount), "BudgetedAmount");
            builder.OwnsOne(typeof(MoneyAmount), "TransactionsTotal");
            builder.OwnsOne(typeof(MoneyAmount), "AllocationsTotal");
        }
    }
}