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
            builder.Property(x => x.BudgetCategoryIconId).HasColumnType("VARCHAR(36)").HasConversion(x => x.ToString(), i => new BudgetCategoryIconId(i));
            builder.Property(x => x.BudgetCategoryId).HasColumnType("VARCHAR(36)").HasConversion(x => x.ToString(), i => new BudgetCategoryId(i));
            builder.Property(x => x.BudgetId).HasColumnType("VARCHAR(36)").HasConversion(x => x.ToString(), i => new BudgetId(i));
            builder.HasOne(x => x.BudgetCategoryIcon).WithMany().HasForeignKey(x=>x.BudgetCategoryIconId);

            builder.HasQueryFilter(x => !x.Deleted);
        }
    }
}