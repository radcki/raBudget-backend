using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using raBudget.Domain.Entities;
using raBudget.Domain.ValueObjects;

namespace raBudget.Infrastructure.Database.Configuration
{
    public class BudgetCategoryIconConfiguration : IEntityTypeConfiguration<BudgetCategoryIcon>
    {
        public void Configure(EntityTypeBuilder<BudgetCategoryIcon> builder)
        {
            builder.HasKey(x => x.BudgetCategoryIconId);
            builder.Property(x => x.BudgetCategoryIconId).HasColumnType("VARCHAR(36)").HasConversion(x => x.ToString(), i => new BudgetCategoryIconId(i));

            builder.HasData(new List<BudgetCategoryIcon>()
                            {
                                BudgetCategoryIcon.Create("mdi-car"),
                                BudgetCategoryIcon.Create("mdi-cart"),
                                BudgetCategoryIcon.Create("mdi-train-car"),
                                BudgetCategoryIcon.Create("mdi-wallet-travel"),
                                BudgetCategoryIcon.Create("mdi-wrench"),
                                BudgetCategoryIcon.Create("mdi-basket"),
                                BudgetCategoryIcon.Create("mdi-gamepad"),
                                BudgetCategoryIcon.Create("mdi-phone"),
                                BudgetCategoryIcon.Create("mdi-airplane"),
                                BudgetCategoryIcon.Create("mdi-currency-usd-circle-outline"),
                                BudgetCategoryIcon.Create("mdi-format-paint"),
                                BudgetCategoryIcon.Create("mdi-gamepad-square"),
                                BudgetCategoryIcon.Create("mdi-laptop"),
                                BudgetCategoryIcon.Create("mdi-camera"),
                                BudgetCategoryIcon.Create("mdi-city"),
                                BudgetCategoryIcon.Create("mdi-fire"),
                                BudgetCategoryIcon.Create("mdi-dumbbell"),
                                BudgetCategoryIcon.Create("mdi-coffee"),
                                BudgetCategoryIcon.Create("mdi-dice-5"),
                                BudgetCategoryIcon.Create("mdi-beach"),
                                BudgetCategoryIcon.Create("mdi-bus-articulated-front"),
                                BudgetCategoryIcon.Create("mdi-smoking"),
                                BudgetCategoryIcon.Create("mdi-fridge"),
                                BudgetCategoryIcon.Create("mdi-baby-face"),
                                BudgetCategoryIcon.Create("mdi-paw"),
                                BudgetCategoryIcon.Create("mdi-bandage"),
                                BudgetCategoryIcon.Create("mdi-human"),
                                BudgetCategoryIcon.Create("mdi-sofa"),
                                BudgetCategoryIcon.Create("mdi-memory"),
                                BudgetCategoryIcon.Create("mdi-cellphone-android"),
                                BudgetCategoryIcon.Create("mdi-speaker"),
                                BudgetCategoryIcon.Create("mdi-sim"),
                                BudgetCategoryIcon.Create("mdi-silverware-fork-knife"),
                                BudgetCategoryIcon.Create("mdi-food"),
                                BudgetCategoryIcon.Create("mdi-gas-station"),
                                BudgetCategoryIcon.Create("mdi-hospital-building"),
                                BudgetCategoryIcon.Create("mdi-shopping"),
                                BudgetCategoryIcon.Create("mdi-glass-cocktail"),
                                BudgetCategoryIcon.Create("mdi-filmstrip"),
                                BudgetCategoryIcon.Create("mdi-bike"),
                                BudgetCategoryIcon.Create("mdi-motorbike"),
                                BudgetCategoryIcon.Create("mdi-wallet-giftcard"),
                            });
        }
    }
}