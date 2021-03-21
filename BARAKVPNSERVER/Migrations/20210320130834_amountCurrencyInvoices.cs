using Microsoft.EntityFrameworkCore.Migrations;

namespace BARAKVPNSERVER.Migrations
{
    public partial class amountCurrencyInvoices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Amount",
                table: "InvoiceDocuments",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "InvoiceDocuments",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "InvoiceDocuments");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "InvoiceDocuments");
        }
    }
}
