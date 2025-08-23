using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAJDAJ.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderHeaderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InstgramUserName",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstgramUserName",
                table: "OrderHeaders");
        }
    }
}
