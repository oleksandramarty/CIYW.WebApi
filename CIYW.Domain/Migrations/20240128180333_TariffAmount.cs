using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CIYW.Domain.Migrations
{
    /// <inheritdoc />
    public partial class TariffAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "CIYW.Currency");

            migrationBuilder.RenameTable(
                name: "Currencies",
                schema: "CIYW.Dictionary",
                newName: "Currencies",
                newSchema: "CIYW.Currency");

            migrationBuilder.CreateTable(
                name: "CurrencyEntities",
                schema: "CIYW.Currency",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    TariffId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrencyEntities_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "CIYW.Currency",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CurrencyEntities_Tariffs_TariffId",
                        column: x => x.TariffId,
                        principalSchema: "CIYW.Tariff",
                        principalTable: "Tariffs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyEntities_CurrencyId",
                schema: "CIYW.Currency",
                table: "CurrencyEntities",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyEntities_TariffId",
                schema: "CIYW.Currency",
                table: "CurrencyEntities",
                column: "TariffId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyEntities",
                schema: "CIYW.Currency");

            migrationBuilder.EnsureSchema(
                name: "CIYW.Dictionary");

            migrationBuilder.RenameTable(
                name: "Currencies",
                schema: "CIYW.Currency",
                newName: "Currencies",
                newSchema: "CIYW.Dictionary");
        }
    }
}
