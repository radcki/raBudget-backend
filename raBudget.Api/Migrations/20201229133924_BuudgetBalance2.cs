using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace raBudget.Api.Migrations
{
    public partial class BuudgetBalance2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateTime",
                table: "BudgetBalances",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                                        name: "UpdateTime",
                                        table: "BudgetBalances");
        }
    }
}
