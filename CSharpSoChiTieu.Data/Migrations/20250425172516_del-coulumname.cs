using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSharpSoChiTieu.Data.Migrations
{
    /// <inheritdoc />
    public partial class delcoulumname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ct_IncomeExpense");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ct_IncomeExpense",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
