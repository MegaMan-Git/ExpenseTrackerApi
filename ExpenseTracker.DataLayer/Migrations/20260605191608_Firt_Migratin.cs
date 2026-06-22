using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class Firt_Migratin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "StaticPasswordHashForSeedData" });

            migrationBuilder.UpdateData(
                table: "expenses",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 5, 19, 16, 8, 137, DateTimeKind.Utc).AddTicks(2200));

            migrationBuilder.UpdateData(
                table: "expenses",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 5, 19, 16, 8, 137, DateTimeKind.Utc).AddTicks(2819));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 5, 19, 10, 8, 539, DateTimeKind.Utc).AddTicks(4221), "hashed-password" });

            migrationBuilder.UpdateData(
                table: "expenses",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 5, 19, 10, 8, 539, DateTimeKind.Utc).AddTicks(8121));

            migrationBuilder.UpdateData(
                table: "expenses",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 5, 19, 10, 8, 539, DateTimeKind.Utc).AddTicks(8866));
        }
    }
}
