using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CIYW.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "CIYW.Note",
                table: "Notes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Notes_UserId",
                schema: "CIYW.Note",
                table: "Notes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_AspNetUsers_UserId",
                schema: "CIYW.Note",
                table: "Notes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notes_AspNetUsers_UserId",
                schema: "CIYW.Note",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_UserId",
                schema: "CIYW.Note",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "CIYW.Note",
                table: "Notes");
        }
    }
}
