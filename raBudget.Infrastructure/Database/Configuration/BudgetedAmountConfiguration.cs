using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using raBudget.Domain.Entities;
using raBudget.Domain.ValueObjects;

namespace raBudget.Infrastructure.Database.Configuration
{
    public class BudgetedAmountConfiguration : IEntityTypeConfiguration<BudgetCategory.BudgetedAmount>
    {
        public void Configure(EntityTypeBuilder<BudgetCategory.BudgetedAmount> builder)
        {
            builder.HasKey(x => x.BudgetedAmountId);

            builder.Property(x => x.BudgetedAmountId).HasColumnType("char(36)").HasConversion<Guid>(x => x.Value, i => new BudgetedAmountId(i));
            builder.Property(x => x.BudgetCategoryId).HasColumnType("char(36)").HasConversion<Guid>(x => x.Value, i => new BudgetCategoryId(i));

            builder.OwnsOne(typeof(MoneyAmount), "Amount");

        }
    }
}