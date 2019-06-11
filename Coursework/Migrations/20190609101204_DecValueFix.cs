using Microsoft.EntityFrameworkCore.Migrations;

namespace ConsulService.Migrations
{
    public partial class DecValueFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "Transactions",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Value",
                table: "Transactions",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
