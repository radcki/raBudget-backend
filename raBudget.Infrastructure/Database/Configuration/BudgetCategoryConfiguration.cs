using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using raBudget.Domain.Entities;
using raBudget.Domain.ValueObjects;

namespace raBudget.Infrastructure.Database.Configuration
{
    public class BudgetCategoryConfiguration : IEntityTypeConfiguration<BudgetCategory>
    {
        public void Configure(EntityTypeBuilder<BudgetCategory> builder)
        {
            builder.HasKey(x => x.BudgetCategoryId);
            builder.Property(x => x.BudgetCategoryIconId).HasColumnType("char(36)").HasConversion<Guid>(x => x.Value, i => new BudgetCategoryIconId(i));
            builder.Property(x => x.BudgetCategoryId).HasColumnType("char(36)").HasConversion<Guid>(x => x.Value, i => new BudgetCategoryId(i));
            builder.Property(x => x.BudgetId).HasColumnType("char(36)").HasConversion<Guid>(x => x.Value, i => new BudgetId(i));
        }
    }
}