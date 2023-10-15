using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tl121pet.Migrations
{
    public partial class UserLocales : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Locale",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Locale",
                table: "Users");
        }
    }
}
