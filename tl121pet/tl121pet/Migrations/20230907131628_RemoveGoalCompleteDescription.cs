using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tl121pet.Migrations
{
    public partial class RemoveGoalCompleteDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompleteDescription",
                table: "MeetingGoals");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompleteDescription",
                table: "MeetingGoals",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
