using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.Infrastructe.Migrations
{
    /// <inheritdoc />
    public partial class IntialCreate4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                column: "MessageSend",
                value: new DateTime(2024, 2, 19, 20, 45, 52, 919, DateTimeKind.Utc).AddTicks(4333));

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 2,
                column: "MessageSend",
                value: new DateTime(2024, 2, 19, 20, 45, 52, 919, DateTimeKind.Utc).AddTicks(4340));

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 3,
                column: "MessageSend",
                value: new DateTime(2024, 2, 19, 20, 45, 52, 919, DateTimeKind.Utc).AddTicks(4342));

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 4,
                column: "MessageSend",
                value: new DateTime(2024, 2, 19, 20, 45, 52, 919, DateTimeKind.Utc).AddTicks(4343));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                column: "MessageSend",
                value: new DateTime(2024, 2, 19, 20, 42, 35, 644, DateTimeKind.Utc).AddTicks(925));

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 2,
                column: "MessageSend",
                value: new DateTime(2024, 2, 19, 20, 42, 35, 644, DateTimeKind.Utc).AddTicks(932));

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 3,
                column: "MessageSend",
                value: new DateTime(2024, 2, 19, 20, 42, 35, 644, DateTimeKind.Utc).AddTicks(933));

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 4,
                column: "MessageSend",
                value: new DateTime(2024, 2, 19, 20, 42, 35, 644, DateTimeKind.Utc).AddTicks(935));
        }
    }
}
