using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JapanApp.Migrations
{
    /// <inheritdoc />
    public partial class AddFestivalMapLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Festivals",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Festivals",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Festivals");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Festivals");
        }
    }
}
