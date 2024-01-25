using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CIYW.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UserSchemaPasswordRestore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "CIYW.User");

            migrationBuilder.EnsureSchema(
                name: "CIYW.Notification");

            migrationBuilder.EnsureSchema(
                name: "CIYW.Common");

            migrationBuilder.RenameTable(
                name: "UserCategories",
                schema: "CIYW.Users",
                newName: "UserCategories",
                newSchema: "CIYW.User");

            migrationBuilder.RenameTable(
                name: "ActiveUsers",
                schema: "CIYW.Users",
                newName: "ActiveUsers",
                newSchema: "CIYW.User");

            migrationBuilder.AddColumn<DateTime>(
                name: "Restored",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "CIYW.Notification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Read = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EntityType = table.Column<int>(type: "integer", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RestorePasswords",
                schema: "CIYW.Common",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Used = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestorePasswords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                schema: "CIYW.Notification",
                table: "Notifications",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "CIYW.Notification");

            migrationBuilder.DropTable(
                name: "RestorePasswords",
                schema: "CIYW.Common");

            migrationBuilder.DropColumn(
                name: "Restored",
                table: "AspNetUsers");

            migrationBuilder.EnsureSchema(
                name: "CIYW.Users");

            migrationBuilder.RenameTable(
                name: "UserCategories",
                schema: "CIYW.User",
                newName: "UserCategories",
                newSchema: "CIYW.Users");

            migrationBuilder.RenameTable(
                name: "ActiveUsers",
                schema: "CIYW.User",
                newName: "ActiveUsers",
                newSchema: "CIYW.Users");
        }
    }
}
