using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CIYW.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AdminIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "CIYW.Tariff",
                table: "Tariffs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "CIYW.Dictionary",
                table: "Currencies",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "CIYW.Category",
                table: "Categories",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "CIYW.Tariff",
                table: "Tariffs");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "CIYW.Dictionary",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "CIYW.Category",
                table: "Categories");
        }
    }
}
