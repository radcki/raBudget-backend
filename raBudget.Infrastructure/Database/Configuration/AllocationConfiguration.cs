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
            builder.Property(x => x.AllocationId).HasColumnType("char(36)").HasConversion<Guid>(x => x.Value, i => new AllocationId(i));
            builder.Property(x => x.TargetBudgetCategoryId).HasColumnType("char(36)").HasConversion<Guid>(x => x.Value, i => new BudgetCategoryId(i));
            builder.Property(x => x.SourceBudgetCategoryId)
				   .HasColumnType("char(36)")
				   .IsRequired(false)
				   .HasConversion<Guid?>(x => x.Value, i => i != null ? new BudgetCategoryId(i.Value) : null);

            builder.OwnsOne(typeof(MoneyAmount), "Amount");
        }
    }
}