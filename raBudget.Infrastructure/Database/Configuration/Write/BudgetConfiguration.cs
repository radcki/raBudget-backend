using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using raBudget.Domain.Models;

namespace raBudget.Infrastructure.Database.Configuration.Write
{
    public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
    {
        public void Configure(EntityTypeBuilder<Budget> builder)
        {
            builder.HasKey(x => x.BudgetId);
            builder.Property(x => x.BudgetId).ValueGeneratedOnAdd();
            builder.Property(x => x.BudgetId).HasConversion<int>(x => x.Value, i => new Budget.Id(i));
        }
    }
}