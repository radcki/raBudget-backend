﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using raBudget.Domain.Entities;
using raBudget.Domain.ValueObjects;

namespace raBudget.Infrastructure.Database.Configuration
{
    public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
    {
        public void Configure(EntityTypeBuilder<Budget> builder)
        {
            builder.HasKey(x => x.BudgetId);
            builder.Property(x => x.BudgetId).HasColumnType("VARCHAR(36)").HasConversion(x => x.ToString(), i => new BudgetId(i));
            builder.HasOne<Currency>().WithMany().HasForeignKey(x => x.CurrencyCode);
        }
    }
}