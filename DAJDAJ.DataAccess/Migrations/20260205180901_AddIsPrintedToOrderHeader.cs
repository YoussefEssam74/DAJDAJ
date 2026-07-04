using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAJDAJ.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPrintedToOrderHeader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrinted",
                table: "OrderHeaders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrinted",
                table: "OrderHeaders");
        }
    }
}
