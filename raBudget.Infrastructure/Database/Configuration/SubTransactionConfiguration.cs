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
            builder.Property(x => x.SubTransactionId).HasColumnType("char(36)").HasConversion<Guid>(x => x.Value, i => new SubTransactionId(i));
            builder.Property(x => x.ParentTransactionId).HasColumnType("char(36)").HasConversion<Guid>(x => x.Value, i => new TransactionId(i));

            builder.OwnsOne(typeof(MoneyAmount), "Amount");

        }
    }
}