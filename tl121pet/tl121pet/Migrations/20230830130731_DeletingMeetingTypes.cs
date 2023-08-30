using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tl121pet.Migrations
{
    public partial class DeletingMeetingTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_MeetingTypes_MeetingTypeId",
                table: "Meetings");

            migrationBuilder.DropIndex(
                name: "IX_Meetings_MeetingTypeId",
                table: "Meetings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Meetings_MeetingTypeId",
                table: "Meetings",
                column: "MeetingTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_MeetingTypes_MeetingTypeId",
                table: "Meetings",
                column: "MeetingTypeId",
                principalTable: "MeetingTypes",
                principalColumn: "MeetingTypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
