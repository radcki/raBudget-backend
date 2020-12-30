using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace raBudget.Api.Migrations
{
    public partial class BuudgetBalance3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.CreateTable(
                name: "Allocations",
                columns: table => new
                {
                    AllocationId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    TargetBudgetCategoryId = table.Column<Guid>(type: "char(36)", nullable: true),
                    SourceBudgetCategoryId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Amount_CurrencyCode = table.Column<int>(type: "int", nullable: true),
                    Amount_Amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    AllocationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allocations", x => x.AllocationId);
                });

            migrationBuilder.CreateTable(
                name: "BudgetCategoryBalances",
                columns: table => new
                {
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    BudgetCategoryId = table.Column<Guid>(type: "char(36)", nullable: false),
                    BudgetedAmount_CurrencyCode = table.Column<int>(type: "int", nullable: true),
                    BudgetedAmount_Amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    TransactionsTotal_CurrencyCode = table.Column<int>(type: "int", nullable: true),
                    TransactionsTotal_Amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    AllocationsTotal_CurrencyCode = table.Column<int>(type: "int", nullable: true),
                    AllocationsTotal_Amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetCategoryBalances", x => new { x.Year, x.Month, x.BudgetCategoryId });
                });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Allocations");

            migrationBuilder.DropTable(
                name: "BudgetCategoryBalances");

           
        }
    }
}
