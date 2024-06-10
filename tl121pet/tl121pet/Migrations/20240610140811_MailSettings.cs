using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace tl121pet.Migrations
{
    /// <inheritdoc />
    public partial class MailSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MailSettingsUserMailSettingId",
                table: "Users",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserMailSetting",
                columns: table => new
                {
                    UserMailSettingId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    EMailPassword = table.Column<string>(type: "text", nullable: false),
                    EmailHostAddress = table.Column<string>(type: "text", nullable: false),
                    EmailPort = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMailSetting", x => x.UserMailSettingId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_MailSettingsUserMailSettingId",
                table: "Users",
                column: "MailSettingsUserMailSettingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserMailSetting_MailSettingsUserMailSettingId",
                table: "Users",
                column: "MailSettingsUserMailSettingId",
                principalTable: "UserMailSetting",
                principalColumn: "UserMailSettingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserMailSetting_MailSettingsUserMailSettingId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "UserMailSetting");

            migrationBuilder.DropIndex(
                name: "IX_Users_MailSettingsUserMailSettingId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MailSettingsUserMailSettingId",
                table: "Users");
        }
    }
}
