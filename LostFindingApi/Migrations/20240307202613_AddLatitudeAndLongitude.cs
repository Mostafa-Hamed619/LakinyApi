using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LostFindingApi.Migrations
{
    /// <inheritdoc />
    public partial class AddLatitudeAndLongitude : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Item",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Item",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Item");
        }
    }
}
