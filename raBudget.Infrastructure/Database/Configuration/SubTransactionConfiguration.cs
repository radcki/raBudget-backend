using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using raBudget.Domain.Entities;
using raBudget.Domain.ValueObjects;

namespace raBudget.Infrastructure.Database.Configuration
{
    public class SubTransactionConfiguration : IEntityTypeConfiguration<SubTransaction>
    {
        public void Configure(EntityTypeBuilder<SubTransaction> builder)
        {
            builder.HasKey(x => x.SubTransactionId);
            builder.Property(x => x.SubTransactionId).HasColumnType("VARCHAR(36)").HasConversion<string>(x => x.ToString(), i => new SubTransactionId(i));
            builder.Property(x => x.ParentTransactionId).HasColumnType("VARCHAR(36)").HasConversion<string>(x => x.ToString(), i => new TransactionId(i));

            builder.OwnsOne(typeof(MoneyAmount), "Amount");
            builder.HasQueryFilter(x => !x.Deleted);

        }
    }
}