using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSharpSoChiTieu.Data.Migrations
{
    /// <inheritdoc />
    public partial class update_clounm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "ct_IncomeExpense",
                newName: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "ct_IncomeExpense",
                newName: "Status");
        }
    }
}
