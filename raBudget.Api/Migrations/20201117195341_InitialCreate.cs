using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace raBudget.Api.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BudgetCategoryIcons",
                columns: table => new
                {
                    BudgetCategoryIconId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetCategoryIcons", x => x.BudgetCategoryIconId);
                });

            migrationBuilder.CreateTable(
                name: "Budgets",
                columns: table => new
                {
                    BudgetId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    OwnerUserId = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    StartingDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CurrencyCode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budgets", x => x.BudgetId);
                });

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    CurrencyCode = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Symbol = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    EnglishName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    NativeName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.CurrencyCode);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    BudgetCategoryId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Amount_CurrencyCode = table.Column<int>(type: "int", nullable: true),
                    Amount_Amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                });

            migrationBuilder.CreateTable(
                name: "BudgetCategories",
                columns: table => new
                {
                    BudgetCategoryId = table.Column<Guid>(type: "char(36)", nullable: false),
                    BudgetId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    BudgetCategoryIconId = table.Column<Guid>(type: "char(36)", nullable: true),
                    BudgetCategoryType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetCategories", x => x.BudgetCategoryId);
                    table.ForeignKey(
                        name: "FK_BudgetCategories_Budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budgets",
                        principalColumn: "BudgetId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubTransaction",
                columns: table => new
                {
                    SubTransactionId = table.Column<Guid>(type: "char(36)", nullable: false),
                    ParentTransactionId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Amount_CurrencyCode = table.Column<int>(type: "int", nullable: true),
                    Amount_Amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TransactionId = table.Column<Guid>(type: "char(36)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubTransaction", x => x.SubTransactionId);
                    table.ForeignKey(
                        name: "FK_SubTransaction_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BudgetedAmount",
                columns: table => new
                {
                    BudgetedAmountId = table.Column<Guid>(type: "char(36)", nullable: false),
                    BudgetCategoryId = table.Column<Guid>(type: "char(36)", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Amount_CurrencyCode = table.Column<int>(type: "int", nullable: true),
                    Amount_Amount = table.Column<decimal>(type: "decimal(65,30)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetedAmount", x => x.BudgetedAmountId);
                    table.ForeignKey(
                        name: "FK_BudgetedAmount_BudgetCategories_BudgetCategoryId",
                        column: x => x.BudgetCategoryId,
                        principalTable: "BudgetCategories",
                        principalColumn: "BudgetCategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetCategories_BudgetId",
                table: "BudgetCategories",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetedAmount_BudgetCategoryId",
                table: "BudgetedAmount",
                column: "BudgetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SubTransaction_TransactionId",
                table: "SubTransaction",
                column: "TransactionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetCategoryIcons");

            migrationBuilder.DropTable(
                name: "BudgetedAmount");

            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropTable(
                name: "SubTransaction");

            migrationBuilder.DropTable(
                name: "BudgetCategories");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Budgets");
        }
    }
}
