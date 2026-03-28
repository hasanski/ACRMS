using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ACRMS.Migrations
{
    /// <inheritdoc />
    public partial class AddSectionChatPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVisibleToStudents",
                table: "Conversations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "StudentsCanReply",
                table: "Conversations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVisibleToStudents",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "StudentsCanReply",
                table: "Conversations");
        }
    }
}
