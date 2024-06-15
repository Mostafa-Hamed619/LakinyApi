using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LostFindingApi.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "hubConnections");

            migrationBuilder.DropColumn(
                name: "MessageType",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Notifications",
                newName: "NotifiedUser");

            migrationBuilder.RenameColumn(
                name: "NotificationDateTime",
                table: "Notifications",
                newName: "NotificationTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NotifiedUser",
                table: "Notifications",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "NotificationTime",
                table: "Notifications",
                newName: "NotificationDateTime");

            migrationBuilder.AddColumn<string>(
                name: "MessageType",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "hubConnections",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConnectionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hubConnections", x => x.id);
                });
        }
    }
}
