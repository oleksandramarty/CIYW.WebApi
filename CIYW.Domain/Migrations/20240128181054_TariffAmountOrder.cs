using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CIYW.Domain.Migrations
{
    /// <inheritdoc />
    public partial class TariffAmountOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                schema: "CIYW.Tariff",
                table: "Tariffs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                schema: "CIYW.Tariff",
                table: "Tariffs");
        }
    }
}
