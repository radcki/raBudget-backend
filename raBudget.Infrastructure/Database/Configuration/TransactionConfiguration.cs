﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using raBudget.Domain.Entities;
using raBudget.Domain.ValueObjects;

namespace raBudget.Infrastructure.Database.Configuration
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(x => x.TransactionId);
            builder.Property(x => x.TransactionId).HasColumnType("VARCHAR(36)").HasConversion<string>(x => x.ToString(), i => new TransactionId(i));
            builder.Property(x => x.BudgetCategoryId).HasColumnType("VARCHAR(36)").HasConversion<string>(x => x.ToString(), i => new BudgetCategoryId(i));

            builder.OwnsOne(typeof(MoneyAmount), "Amount");
            builder.HasQueryFilter(x => !x.Deleted);
        }
    }
}