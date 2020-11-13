﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using raBudget.Domain.Entities;

namespace raBudget.Infrastructure.Database.Configuration
{
    public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
    {
        public void Configure(EntityTypeBuilder<Budget> builder)
        {
            builder.HasKey(x => x.BudgetId);
            builder.Property(x => x.BudgetId).HasConversion<Guid>(x => x.Value, i => new Budget.Id(i));
        }
    }
}