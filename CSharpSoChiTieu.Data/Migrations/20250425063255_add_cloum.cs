using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSharpSoChiTieu.Data.Migrations
{
    /// <inheritdoc />
    public partial class add_cloum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ct_IncomeExpenseCategory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "ct_IncomeExpense",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "ct_IncomeExpenseCategory");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "ct_IncomeExpense");
        }
    }
}
