using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shelf.Life.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveExtraFoodColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CookingTimeMinutes",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "TotalCalories",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "TotalGrams",
                table: "Foods");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CookingTimeMinutes",
                table: "Foods",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalCalories",
                table: "Foods",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalGrams",
                table: "Foods",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
