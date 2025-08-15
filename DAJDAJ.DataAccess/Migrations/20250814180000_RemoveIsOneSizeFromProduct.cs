using Microsoft.EntityFrameworkCore.Migrations;

namespace DAJDAJ.DataAccess.Migrations
{
    public partial class RemoveIsOneSizeFromProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOneSize",
                table: "products");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOneSize",
                table: "products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
