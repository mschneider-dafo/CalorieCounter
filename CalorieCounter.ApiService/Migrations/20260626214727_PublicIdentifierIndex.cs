using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalorieCounter.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class PublicIdentifierIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FoodEntries_PublicIdentifier",
                table: "FoodEntries",
                column: "PublicIdentifier",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FoodEntries_PublicIdentifier",
                table: "FoodEntries");
        }
    }
}
