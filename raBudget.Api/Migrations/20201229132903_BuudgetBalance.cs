using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace raBudget.Api.Migrations
{
    public partial class BuudgetBalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.CreateTable(
                name: "BudgetBalances",
                columns: table => new
                {
                    BudgetId = table.Column<Guid>(type: "char(36)", nullable: false),
                    TotalBalance_CurrencyCode = table.Column<int>(type: "int", nullable: true),
                    TotalBalance_Amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    UnassignedFunds_CurrencyCode = table.Column<int>(type: "int", nullable: true),
                    UnassignedFunds_Amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    SpendingTotal_CurrencyCode = table.Column<int>(type: "int", nullable: true),
                    SpendingTotal_Amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    IncomeTotal_CurrencyCode = table.Column<int>(type: "int", nullable: true),
                    IncomeTotal_Amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    SavingTotal_CurrencyCode = table.Column<int>(type: "int", nullable: true),
                    SavingTotal_Amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetBalances", x => x.BudgetId);
                });
            }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetBalances");
            
        }
    }
}
