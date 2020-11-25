using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using raBudget.Domain.Models;

namespace raBudget.Infrastructure.Database.Configuration
{
    public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.HasKey(x => x.CurrencyCode);
            builder.Ignore(x => x.NumberFormat);
            builder.HasData(Currency.CurrencyDictionary.Select(x => x.Value));
        }
    }
}