using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Signpdf.Migrations
{
    public partial class addFilePath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PdfFilePath",
                table: "ocr",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PdfFilePath",
                table: "ocr");
        }
    }
}
