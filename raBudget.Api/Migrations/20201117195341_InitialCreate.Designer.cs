﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using raBudget.Infrastructure.Database;

namespace raBudget.Api.Migrations
{
    [DbContext(typeof(WriteDbContext))]
    [Migration("20201117195341_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("raBudget.Domain.Entities.Budget", b =>
                {
                    b.Property<Guid>("BudgetId")
                        .HasColumnType("char(36)");

                    b.Property<int>("CurrencyCode")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("OwnerUserId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("StartingDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("BudgetId");

                    b.ToTable("Budgets");
                });

            modelBuilder.Entity("raBudget.Domain.Entities.BudgetCategory", b =>
                {
                    b.Property<Guid>("BudgetCategoryId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("BudgetCategoryIconId")
                        .HasColumnType("char(36)");

                    b.Property<int>("BudgetCategoryType")
                        .HasColumnType("int");

                    b.Property<Guid?>("BudgetId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.HasKey("BudgetCategoryId");

                    b.HasIndex("BudgetId");

                    b.ToTable("BudgetCategories");
                });

            modelBuilder.Entity("raBudget.Domain.Entities.BudgetCategory+BudgetedAmount", b =>
                {
                    b.Property<Guid>("BudgetedAmountId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("BudgetCategoryId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("ValidFrom")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("ValidTo")
                        .HasColumnType("datetime(6)");

                    b.HasKey("BudgetedAmountId");

                    b.HasIndex("BudgetCategoryId");

                    b.ToTable("BudgetedAmount");
                });

            modelBuilder.Entity("raBudget.Domain.Entities.BudgetCategoryIcon", b =>
                {
                    b.Property<Guid>("BudgetCategoryIconId")
                        .HasColumnType("char(36)");

                    b.HasKey("BudgetCategoryIconId");

                    b.ToTable("BudgetCategoryIcons");
                });

            modelBuilder.Entity("raBudget.Domain.Entities.SubTransaction", b =>
                {
                    b.Property<Guid>("SubTransactionId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreationDateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<Guid?>("ParentTransactionId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("TransactionDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("TransactionId")
                        .HasColumnType("char(36)");

                    b.HasKey("SubTransactionId");

                    b.HasIndex("TransactionId");

                    b.ToTable("SubTransaction");
                });

            modelBuilder.Entity("raBudget.Domain.Entities.Transaction", b =>
                {
                    b.Property<Guid>("TransactionId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("BudgetCategoryId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreationDateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("TransactionDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("TransactionId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("raBudget.Domain.Models.Currency", b =>
                {
                    b.Property<int>("CurrencyCode")
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("EnglishName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("NativeName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Symbol")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("CurrencyCode");

                    b.ToTable("Currency");
                });

            modelBuilder.Entity("raBudget.Domain.Entities.BudgetCategory", b =>
                {
                    b.HasOne("raBudget.Domain.Entities.Budget", null)
                        .WithMany("BudgetCategories")
                        .HasForeignKey("BudgetId");
                });

            modelBuilder.Entity("raBudget.Domain.Entities.BudgetCategory+BudgetedAmount", b =>
                {
                    b.HasOne("raBudget.Domain.Entities.BudgetCategory", null)
                        .WithMany("BudgetedAmounts")
                        .HasForeignKey("BudgetCategoryId");

                    b.OwnsOne("raBudget.Domain.ValueObjects.MoneyAmount", "Amount", b1 =>
                        {
                            b1.Property<Guid>("BudgetedAmountId")
                                .HasColumnType("char(36)");

                            b1.Property<decimal>("Amount")
                                .HasColumnType("decimal(65,30)");

                            b1.Property<int>("CurrencyCode")
                                .HasColumnType("int");

                            b1.HasKey("BudgetedAmountId");

                            b1.ToTable("BudgetedAmount");

                            b1.WithOwner()
                                .HasForeignKey("BudgetedAmountId");
                        });

                    b.Navigation("Amount");
                });

            modelBuilder.Entity("raBudget.Domain.Entities.SubTransaction", b =>
                {
                    b.HasOne("raBudget.Domain.Entities.Transaction", null)
                        .WithMany("SubTransactions")
                        .HasForeignKey("TransactionId");

                    b.OwnsOne("raBudget.Domain.ValueObjects.MoneyAmount", "Amount", b1 =>
                        {
                            b1.Property<Guid>("SubTransactionId")
                                .HasColumnType("char(36)");

                            b1.Property<decimal>("Amount")
                                .HasColumnType("decimal(65,30)");

                            b1.Property<int>("CurrencyCode")
                                .HasColumnType("int");

                            b1.HasKey("SubTransactionId");

                            b1.ToTable("SubTransaction");

                            b1.WithOwner()
                                .HasForeignKey("SubTransactionId");
                        });

                    b.Navigation("Amount");
                });

            modelBuilder.Entity("raBudget.Domain.Entities.Transaction", b =>
                {
                    b.OwnsOne("raBudget.Domain.ValueObjects.MoneyAmount", "Amount", b1 =>
                        {
                            b1.Property<Guid>("TransactionId")
                                .HasColumnType("char(36)");

                            b1.Property<decimal>("Amount")
                                .HasColumnType("decimal(65,30)");

                            b1.Property<int>("CurrencyCode")
                                .HasColumnType("int");

                            b1.HasKey("TransactionId");

                            b1.ToTable("Transactions");

                            b1.WithOwner()
                                .HasForeignKey("TransactionId");
                        });

                    b.Navigation("Amount");
                });

            modelBuilder.Entity("raBudget.Domain.Entities.Budget", b =>
                {
                    b.Navigation("BudgetCategories");
                });

            modelBuilder.Entity("raBudget.Domain.Entities.BudgetCategory", b =>
                {
                    b.Navigation("BudgetedAmounts");
                });

            modelBuilder.Entity("raBudget.Domain.Entities.Transaction", b =>
                {
                    b.Navigation("SubTransactions");
                });
#pragma warning restore 612, 618
        }
    }
}
