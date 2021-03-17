using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using raBudget.Domain.Entities;
using raBudget.Domain.ValueObjects;

namespace raBudget.Infrastructure.Database.Configuration
{
    public class TransactionTemplateConfiguration : IEntityTypeConfiguration<TransactionTemplate>
    {
        public void Configure(EntityTypeBuilder<TransactionTemplate> builder)
        {
            builder.HasKey(x => x.TransactionTemplateId);
            builder.Property(x => x.TransactionTemplateId).HasColumnType("char(36)").HasConversion<Guid>(x => x.Value, i => new TransactionTemplateId(i));
            builder.Property(x => x.BudgetCategoryId).HasColumnType("char(36)").HasConversion<Guid>(x => x.Value, i => new BudgetCategoryId(i));

            builder.OwnsOne(typeof(MoneyAmount), "Amount");
            builder.HasQueryFilter(x => !x.Deleted);
        }
    }
}