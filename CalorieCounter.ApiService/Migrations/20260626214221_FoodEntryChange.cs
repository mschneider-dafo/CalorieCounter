using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalorieCounter.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class FoodEntryChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodEntries_FoodItems_FoodItemId",
                table: "FoodEntries");

            migrationBuilder.AlterColumn<Guid>(
                name: "FoodItemId",
                table: "FoodEntries",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<Guid>(
                name: "PublicIdentifier",
                table: "FoodEntries",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_FoodEntries_FoodItems_FoodItemId",
                table: "FoodEntries",
                column: "FoodItemId",
                principalTable: "FoodItems",
                principalColumn: "InternalId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodEntries_FoodItems_FoodItemId",
                table: "FoodEntries");

            migrationBuilder.DropColumn(
                name: "PublicIdentifier",
                table: "FoodEntries");

            migrationBuilder.AlterColumn<int>(
                name: "FoodItemId",
                table: "FoodEntries",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodEntries_FoodItems_FoodItemId",
                table: "FoodEntries",
                column: "FoodItemId",
                principalTable: "FoodItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
