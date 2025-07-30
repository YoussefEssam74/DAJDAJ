using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAJDAJ.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateShoppingCartTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Colors",
                table: "Shoppingcarts");

            migrationBuilder.RenameColumn(
                name: "Sizes",
                table: "Shoppingcarts",
                newName: "SelectedSize");

            migrationBuilder.RenameColumn(
                name: "ProductImages",
                table: "Shoppingcarts",
                newName: "SelectedColor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SelectedSize",
                table: "Shoppingcarts",
                newName: "Sizes");

            migrationBuilder.RenameColumn(
                name: "SelectedColor",
                table: "Shoppingcarts",
                newName: "ProductImages");

            migrationBuilder.AddColumn<string>(
                name: "Colors",
                table: "Shoppingcarts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
