using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAJDAJ.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveColorSizeColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Commented out because these constraints may not exist in all databases
            /*
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_ProductColors_ProductColorId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_ProductSizes_ProductSizeId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_ProductColors_ProductColorId",
                table: "ProductImages");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_products_ProductId",
                table: "ProductImages");

            migrationBuilder.DropForeignKey(
                name: "FK_Shoppingcarts_ProductColors_ProductColorId",
                table: "Shoppingcarts");

            migrationBuilder.DropForeignKey(
                name: "FK_Shoppingcarts_ProductSizes_ProductSizeId",
                table: "Shoppingcarts");

            migrationBuilder.DropTable(
                name: "ProductColorSizeStocks");

            migrationBuilder.DropTable(
                name: "ProductColors");

            migrationBuilder.DropTable(
                name: "ProductSizes");

            migrationBuilder.DropIndex(
                name: "IX_Shoppingcarts_ProductColorId",
                table: "Shoppingcarts");

            migrationBuilder.DropIndex(
                name: "IX_Shoppingcarts_ProductSizeId",
                table: "Shoppingcarts");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_ProductColorId",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProductColorId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProductSizeId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ProductColorId",
                table: "Shoppingcarts");

            migrationBuilder.DropColumn(
                name: "ProductSizeId",
                table: "Shoppingcarts");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "products");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "products");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "ProductColorId",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "UploadedAt",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "ProductColorId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ProductSizeId",
                table: "OrderDetails");

            migrationBuilder.AlterColumn<string>(
                name: "Size",
                table: "products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductImages",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "ProductImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SelectedSize",
                table: "OrderDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SelectedColor",
                table: "OrderDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_products_ProductId",
                table: "ProductImages",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Down migration commented out as the Up migration is also commented
            /*
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_products_ProductId",
                table: "ProductImages");

            migrationBuilder.AddColumn<int>(
                name: "ProductColorId",
                table: "Shoppingcarts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductSizeId",
                table: "Shoppingcarts",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Size",
                table: "products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductImages",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "ProductImages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "ProductImages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductColorId",
                table: "ProductImages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadedAt",
                table: "ProductImages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "SelectedSize",
                table: "OrderDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SelectedColor",
                table: "OrderDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ProductColorId",
                table: "OrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductSizeId",
                table: "OrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductColors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ColorCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ColorName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductColors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductColors_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductSizes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SizeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSizes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductColorSizeStocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductColorId = table.Column<int>(type: "int", nullable: false),
                    ProductSizeId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ReservedQuantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductColorSizeStocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductColorSizeStocks_ProductColors_ProductColorId",
                        column: x => x.ProductColorId,
                        principalTable: "ProductColors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductColorSizeStocks_ProductSizes_ProductSizeId",
                        column: x => x.ProductSizeId,
                        principalTable: "ProductSizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ProductSizes",
                columns: new[] { "Id", "CreatedAt", "Description", "DisplayOrder", "IsActive", "SizeName" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 28, 20, 23, 20, 232, DateTimeKind.Utc).AddTicks(7711), "Extra Small", 1, true, "XS" },
                    { 2, new DateTime(2026, 1, 28, 20, 23, 20, 232, DateTimeKind.Utc).AddTicks(7717), "Small", 2, true, "S" },
                    { 3, new DateTime(2026, 1, 28, 20, 23, 20, 232, DateTimeKind.Utc).AddTicks(7720), "Medium", 3, true, "M" },
                    { 4, new DateTime(2026, 1, 28, 20, 23, 20, 232, DateTimeKind.Utc).AddTicks(7721), "Large", 4, true, "L" },
                    { 5, new DateTime(2026, 1, 28, 20, 23, 20, 232, DateTimeKind.Utc).AddTicks(7723), "Extra Large", 5, true, "XL" },
                    { 6, new DateTime(2026, 1, 28, 20, 23, 20, 232, DateTimeKind.Utc).AddTicks(7724), "Double Extra Large", 6, true, "XXL" },
                    { 7, new DateTime(2026, 1, 28, 20, 23, 20, 232, DateTimeKind.Utc).AddTicks(7725), "Universal Size", 99, true, "One Size" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shoppingcarts_ProductColorId",
                table: "Shoppingcarts",
                column: "ProductColorId");

            migrationBuilder.CreateIndex(
                name: "IX_Shoppingcarts_ProductSizeId",
                table: "Shoppingcarts",
                column: "ProductSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductColorId",
                table: "ProductImages",
                column: "ProductColorId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductColorId",
                table: "OrderDetails",
                column: "ProductColorId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductSizeId",
                table: "OrderDetails",
                column: "ProductSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductColor_ProductId_ColorName",
                table: "ProductColors",
                columns: new[] { "ProductId", "ColorName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductColorSizeStock_ProductColorId_ProductSizeId",
                table: "ProductColorSizeStocks",
                columns: new[] { "ProductColorId", "ProductSizeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductColorSizeStocks_ProductSizeId",
                table: "ProductColorSizeStocks",
                column: "ProductSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSize_SizeName",
                table: "ProductSizes",
                column: "SizeName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_ProductColors_ProductColorId",
                table: "OrderDetails",
                column: "ProductColorId",
                principalTable: "ProductColors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_ProductSizes_ProductSizeId",
                table: "OrderDetails",
                column: "ProductSizeId",
                principalTable: "ProductSizes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_ProductColors_ProductColorId",
                table: "ProductImages",
                column: "ProductColorId",
                principalTable: "ProductColors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_products_ProductId",
                table: "ProductImages",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Shoppingcarts_ProductColors_ProductColorId",
                table: "Shoppingcarts",
                column: "ProductColorId",
                principalTable: "ProductColors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Shoppingcarts_ProductSizes_ProductSizeId",
                table: "Shoppingcarts",
                column: "ProductSizeId",
                principalTable: "ProductSizes",
                principalColumn: "Id");
            */
        }
    }
}
