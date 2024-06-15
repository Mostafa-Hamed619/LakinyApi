using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LostFindingApi.Migrations
{
    /// <inheritdoc />
    public partial class ADDLISCENCE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "liscences",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    traffic_unit = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    nationality = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    job = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    name_English = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    birth_date = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    birth_place = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    gender = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    age = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_liscences", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "liscences");
        }
    }
}
