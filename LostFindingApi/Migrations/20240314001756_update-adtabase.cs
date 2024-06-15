using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LostFindingApi.Migrations
{
    /// <inheritdoc />
    public partial class updateadtabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cards");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cards",
                columns: table => new
                {
                    CardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    age = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    birth_date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    birth_place = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    face = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    frist_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    regin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    rest_of_name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cards", x => x.CardId);
                });
        }
    }
}
