using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.Infrastructe.Migrations
{
    /// <inheritdoc />
    public partial class IntialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                column: "MessageSend",
                value: new DateTime(2024, 2, 19, 19, 8, 24, 113, DateTimeKind.Utc).AddTicks(2336));

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 2,
                column: "MessageSend",
                value: new DateTime(2024, 2, 19, 19, 8, 24, 113, DateTimeKind.Utc).AddTicks(2347));

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 3,
                column: "MessageSend",
                value: new DateTime(2024, 2, 19, 19, 8, 24, 113, DateTimeKind.Utc).AddTicks(2348));

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 4,
                column: "MessageSend",
                value: new DateTime(2024, 2, 19, 19, 8, 24, 113, DateTimeKind.Utc).AddTicks(2350));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                column: "MessageSend",
                value: new DateTime(2024, 2, 19, 19, 6, 21, 701, DateTimeKind.Utc).AddTicks(2372));

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 2,
                column: "MessageSend",
                value: new DateTime(2024, 2, 19, 19, 6, 21, 701, DateTimeKind.Utc).AddTicks(2385));

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 3,
                column: "MessageSend",
                value: new DateTime(2024, 2, 19, 19, 6, 21, 701, DateTimeKind.Utc).AddTicks(2387));

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 4,
                column: "MessageSend",
                value: new DateTime(2024, 2, 19, 19, 6, 21, 701, DateTimeKind.Utc).AddTicks(2388));
        }
    }
}
