using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using raBudget.Domain.Entities;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Models;

namespace raBudget.Infrastructure.Database.Configuration
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