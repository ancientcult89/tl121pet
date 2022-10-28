using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tl121pet.Migrations
{
    public partial class MeetingUpgrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "MeetingDate",
                table: "Meeting",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<bool>(
                name: "FollowUpIsSended",
                table: "Meeting",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MeetingPlanDate",
                table: "Meeting",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "MeetingGoal",
                columns: table => new
                {
                    MeetingGoalId = table.Column<Guid>(type: "uuid", nullable: false),
                    MeetingGoalDescription = table.Column<string>(type: "text", nullable: false),
                    MeetingId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingGoal", x => x.MeetingGoalId);
                    table.ForeignKey(
                        name: "FK_MeetingGoal_Meeting_MeetingId",
                        column: x => x.MeetingId,
                        principalTable: "Meeting",
                        principalColumn: "MeetingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeetingGoal_MeetingId",
                table: "MeetingGoal",
                column: "MeetingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeetingGoal");

            migrationBuilder.DropColumn(
                name: "FollowUpIsSended",
                table: "Meeting");

            migrationBuilder.DropColumn(
                name: "MeetingPlanDate",
                table: "Meeting");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MeetingDate",
                table: "Meeting",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
