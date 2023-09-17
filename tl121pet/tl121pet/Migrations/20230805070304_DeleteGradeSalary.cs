using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tl121pet.Migrations
{
    public partial class DeleteGradeSalary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalaryId",
                table: "Grades");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SalaryId",
                table: "Grades",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
