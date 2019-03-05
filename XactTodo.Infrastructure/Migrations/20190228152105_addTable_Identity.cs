using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace XactTodo.Infrastructure.Migrations
{
    public partial class addTable_Identity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(maxLength: 50, nullable: false),
                    Nickname = table.Column<string>(maxLength: 50, nullable: false),
                    AccessToken = table.Column<string>(maxLength: 32, nullable: true),
                    RefreshToken = table.Column<string>(maxLength: 32, nullable: true),
                    IssueTime = table.Column<DateTime>(nullable: false),
                    ExpiresIn = table.Column<int>(nullable: false),
                    Invalid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identity", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Identity");
        }
    }
}
