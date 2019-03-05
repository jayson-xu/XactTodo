using Microsoft.EntityFrameworkCore.Migrations;

namespace XactTodo.Infrastructure.Migrations
{
    public partial class AddColumn_Matter_CameFrom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CameFrom",
                table: "Matter",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CameFrom",
                table: "Matter");
        }
    }
}
