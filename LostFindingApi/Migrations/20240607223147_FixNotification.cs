using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LostFindingApi.Migrations
{
    /// <inheritdoc />
    public partial class FixNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NotifiedUser",
                table: "Notifications",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "NotificationTime",
                table: "Notifications",
                newName: "DeviceIds");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "Notifications",
                newName: "Content");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Notifications",
                newName: "NotifiedUser");

            migrationBuilder.RenameColumn(
                name: "DeviceIds",
                table: "Notifications",
                newName: "NotificationTime");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Notifications",
                newName: "Message");
        }
    }
}
