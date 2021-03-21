using Microsoft.EntityFrameworkCore.Migrations;

namespace BARAKVPNSERVER.Migrations
{
    public partial class atciceActive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Actice",
                table: "Users",
                newName: "Active");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Active",
                table: "Users",
                newName: "Actice");
        }
    }
}
