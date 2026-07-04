using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAJDAJ.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueEmailIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EmailOtps_Email_Unique",
                table: "EmailOtps",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmailOtps_Email_Unique",
                table: "EmailOtps");
        }
    }
}
