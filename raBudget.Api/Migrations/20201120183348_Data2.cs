using Microsoft.EntityFrameworkCore.Migrations;

namespace raBudget.Api.Migrations
{
    public partial class Data2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IconKey",
                table: "BudgetCategoryIcons",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_CurrencyCode",
                table: "Budgets",
                column: "CurrencyCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Budgets_Currencies_CurrencyCode",
                table: "Budgets",
                column: "CurrencyCode",
                principalTable: "Currencies",
                principalColumn: "CurrencyCode",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budgets_Currencies_CurrencyCode",
                table: "Budgets");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_CurrencyCode",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "IconKey",
                table: "BudgetCategoryIcons");
        }
    }
}
