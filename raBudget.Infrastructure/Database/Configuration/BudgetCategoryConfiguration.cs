using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using raBudget.Domain.Entities;

namespace raBudget.Infrastructure.Database.Configuration
{
    public class BudgetCategoryConfiguration : IEntityTypeConfiguration<BudgetCategory>
    {
        public void Configure(EntityTypeBuilder<BudgetCategory> builder)
        {
            builder.HasKey(x => x.BudgetCategoryId);
            builder.Property(x => x.BudgetCategoryId).HasConversion<Guid>(x => x.Value, i => new BudgetCategory.Id(i));
            builder.Property(x => x.BudgetId).HasConversion<Guid>(x => x.Value, i => new Budget.Id(i));
        }
    }
}