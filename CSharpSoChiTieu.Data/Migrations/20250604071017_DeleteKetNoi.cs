using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSharpSoChiTieu.Data.Migrations
{
    /// <inheritdoc />
    public partial class DeleteKetNoi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ct_IncomeExpense_ct_IncomeExpenseCategory_CategoryId",
                table: "ct_IncomeExpense");

            migrationBuilder.DropIndex(
                name: "IX_ct_IncomeExpense_CategoryId",
                table: "ct_IncomeExpense");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ct_IncomeExpense_CategoryId",
                table: "ct_IncomeExpense",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ct_IncomeExpense_ct_IncomeExpenseCategory_CategoryId",
                table: "ct_IncomeExpense",
                column: "CategoryId",
                principalTable: "ct_IncomeExpenseCategory",
                principalColumn: "Id");
        }
    }
}
