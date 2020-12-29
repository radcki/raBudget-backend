using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using raBudget.Domain.Entities;
using raBudget.Domain.Models;
using raBudget.Domain.ValueObjects;

namespace raBudget.Infrastructure.Database.Configuration
{
    public class BudgetBalanceConfiguration : IEntityTypeConfiguration<BudgetBalance>
    {
        public void Configure(EntityTypeBuilder<BudgetBalance> builder)
        {
            builder.HasKey(x => x.BudgetId);
            builder.Property(x => x.BudgetId).HasColumnType("char(36)").HasConversion<Guid>(x => x.Value, i => new BudgetId(i));

            builder.OwnsOne(typeof(MoneyAmount), "TotalBalance");
            builder.OwnsOne(typeof(MoneyAmount), "UnassignedFunds");
            builder.OwnsOne(typeof(MoneyAmount), "SpendingTotal");
            builder.OwnsOne(typeof(MoneyAmount), "IncomeTotal");
            builder.OwnsOne(typeof(MoneyAmount), "SavingTotal");
        }
    }
}