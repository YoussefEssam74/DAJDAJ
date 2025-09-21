using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAJDAJ.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSelectedColorAndSelectedSizeToOrderDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SelectedColor",
                table: "OrderDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SelectedSize",
                table: "OrderDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedColor",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "SelectedSize",
                table: "OrderDetails");
        }
    }
}
