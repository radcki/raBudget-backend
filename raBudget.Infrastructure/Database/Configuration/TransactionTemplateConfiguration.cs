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
            builder.Property(x => x.TransactionTemplateId).HasColumnType("VARCHAR(36)").HasConversion<string>(x => x.ToString(), i => new TransactionTemplateId(i));
            builder.Property(x => x.BudgetCategoryId).HasColumnType("VARCHAR(36)").HasConversion<string>(x => x.ToString(), i => new BudgetCategoryId(i));

            builder.OwnsOne(typeof(MoneyAmount), "Amount");
            builder.HasQueryFilter(x => !x.Deleted);
        }
    }
}