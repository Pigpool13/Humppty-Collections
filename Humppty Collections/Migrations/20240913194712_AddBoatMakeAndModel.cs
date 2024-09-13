using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Humppty_Collections.Migrations
{
    /// <inheritdoc />
    public partial class AddBoatMakeAndModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BoatMake",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BoatModel",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoatMake",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "BoatModel",
                table: "Customers");
        }
    }
}
