using Microsoft.EntityFrameworkCore.Migrations;

namespace BARAKVPNSERVER.Migrations
{
    public partial class descriptionInvoices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "InvoiceDocuments",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "InvoiceDocuments");
        }
    }
}
