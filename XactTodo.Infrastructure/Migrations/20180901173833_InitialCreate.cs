using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace XactTodo.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(maxLength: 50, nullable: false),
                    Password = table.Column<string>(maxLength: 128, nullable: true),
                    DisplayName = table.Column<string>(maxLength: 50, nullable: false),
                    Email = table.Column<string>(maxLength: 100, nullable: false),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    CreatorUserId = table.Column<int>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false, defaultValueSql: "sysdate()"),
                    LastModifierUserId = table.Column<int>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    DeleterUserId = table.Column<int>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_User_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    ProposedTags = table.Column<string>(maxLength: 500, nullable: true),
                    LeaderId = table.Column<int>(nullable: false),
                    CreatorUserId = table.Column<int>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false, defaultValueSql: "sysdate()"),
                    LastModifierUserId = table.Column<int>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    DeleterUserId = table.Column<int>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Team_User_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Team_User_LeaderId",
                        column: x => x.LeaderId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Matter",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Subject = table.Column<string>(maxLength: 200, nullable: false),
                    Content = table.Column<string>(nullable: false),
                    ExecutantId = table.Column<int>(nullable: true),
                    Password = table.Column<string>(maxLength: 128, nullable: true),
                    RelatedMatterId = table.Column<int>(nullable: true),
                    Importance = table.Column<int>(nullable: false),
                    Deadline = table.Column<DateTime>(nullable: true),
                    Finished = table.Column<bool>(nullable: false),
                    FinishTime = table.Column<DateTime>(nullable: true),
                    Periodic = table.Column<bool>(nullable: false),
                    Remark = table.Column<string>(maxLength: 500, nullable: true),
                    TeamId = table.Column<int>(nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false, defaultValueSql: "sysdate()"),
                    LastModifierUserId = table.Column<int>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    DeleterUserId = table.Column<int>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matter_User_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matter_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TeamId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    IsSupervisor = table.Column<bool>(nullable: false),
                    CreatorUserId = table.Column<int>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false, defaultValueSql: "sysdate()"),
                    LastModifierUserId = table.Column<int>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    DeleterUserId = table.Column<int>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Member_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Member_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Evolvement",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MatterId = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(maxLength: 500, nullable: true),
                    CreatorUserId = table.Column<int>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false, defaultValueSql: "sysdate()"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evolvement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evolvement_User_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Evolvement_Matter_MatterId",
                        column: x => x.MatterId,
                        principalTable: "Matter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Matter.EstimatedTimeRequired#PeriodOfTime",
                columns: table => new
                {
                    Num = table.Column<decimal>(nullable: false),
                    Unit = table.Column<int>(nullable: false),
                    MatterId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matter.EstimatedTimeRequired#PeriodOfTime", x => x.MatterId);
                    table.ForeignKey(
                        name: "FK_Matter.EstimatedTimeRequired#PeriodOfTime_Matter_MatterId",
                        column: x => x.MatterId,
                        principalTable: "Matter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Matter.IntervalPeriod#PeriodOfTime",
                columns: table => new
                {
                    Num = table.Column<decimal>(nullable: false),
                    Unit = table.Column<int>(nullable: false),
                    MatterId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matter.IntervalPeriod#PeriodOfTime", x => x.MatterId);
                    table.ForeignKey(
                        name: "FK_Matter.IntervalPeriod#PeriodOfTime_Matter_MatterId",
                        column: x => x.MatterId,
                        principalTable: "Matter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatterTag",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MatterId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    Tag = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatterTag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatterTag_Matter_MatterId",
                        column: x => x.MatterId,
                        principalTable: "Matter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatterTag_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Evolvement_CreatorUserId",
                table: "Evolvement",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Evolvement_MatterId",
                table: "Evolvement",
                column: "MatterId");

            migrationBuilder.CreateIndex(
                name: "IX_Matter_CreatorUserId",
                table: "Matter",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Matter_Deadline",
                table: "Matter",
                column: "Deadline");

            migrationBuilder.CreateIndex(
                name: "IX_Matter_Importance",
                table: "Matter",
                column: "Importance");

            migrationBuilder.CreateIndex(
                name: "IX_Matter_Subject",
                table: "Matter",
                column: "Subject");

            migrationBuilder.CreateIndex(
                name: "IX_Matter_TeamId",
                table: "Matter",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_MatterTag_MatterId",
                table: "MatterTag",
                column: "MatterId");

            migrationBuilder.CreateIndex(
                name: "IX_MatterTag_UserId",
                table: "MatterTag",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_TeamId",
                table: "Member",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_UserId",
                table: "Member",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Team_CreatorUserId",
                table: "Team",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Team_LeaderId",
                table: "Team",
                column: "LeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_User_CreatorUserId",
                table: "User",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_UserName",
                table: "User",
                column: "UserName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Evolvement");

            migrationBuilder.DropTable(
                name: "Matter.EstimatedTimeRequired#PeriodOfTime");

            migrationBuilder.DropTable(
                name: "Matter.IntervalPeriod#PeriodOfTime");

            migrationBuilder.DropTable(
                name: "MatterTag");

            migrationBuilder.DropTable(
                name: "Member");

            migrationBuilder.DropTable(
                name: "Matter");

            migrationBuilder.DropTable(
                name: "Team");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
