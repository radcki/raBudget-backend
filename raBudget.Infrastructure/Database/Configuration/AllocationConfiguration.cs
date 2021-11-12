using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using raBudget.Domain.Entities;
using raBudget.Domain.ValueObjects;

namespace raBudget.Infrastructure.Database.Configuration
{
    public class AllocationConfiguration : IEntityTypeConfiguration<Allocation>
    {
        public void Configure(EntityTypeBuilder<Allocation> builder)
        {
            builder.HasKey(x => x.AllocationId);
            builder.Property(x => x.AllocationId).HasColumnType("VARCHAR(36)").HasConversion(x => x.ToString(), i => new AllocationId(i));
            builder.Property(x => x.TargetBudgetCategoryId).HasColumnType("VARCHAR(36)").HasConversion(x => x.ToString(), i => new BudgetCategoryId(i));
            builder.Property(x => x.SourceBudgetCategoryId)
				   .HasColumnType("VARCHAR(36)")
				   .IsRequired(false)
				   .HasConversion(x => x.ToString(), i => i != null ? new BudgetCategoryId(i) : null);

            builder.OwnsOne(typeof(MoneyAmount), "Amount");
            builder.HasQueryFilter(x => !x.Deleted);
        }
    }
}