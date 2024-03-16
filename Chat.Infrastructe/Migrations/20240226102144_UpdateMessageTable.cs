using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Chat.Infrastructe.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMessageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.AddColumn<string>(
                name: "RecipientId",
                table: "Messages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SenderId1",
                table: "Messages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RecipientId",
                table: "Messages",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId1",
                table: "Messages",
                column: "SenderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_RecipientId",
                table: "Messages",
                column: "RecipientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId1",
                table: "Messages",
                column: "SenderId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_RecipientId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId1",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_RecipientId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SenderId1",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "RecipientId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SenderId1",
                table: "Messages");

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "Content", "DateRead", "IsActive", "MessageSend", "ModifiedDate", "RecieptId", "RecieptUserName", "RecipientDeleted", "SenderDeleted", "SenderId", "SenderUserName" },
                values: new object[,]
                {
                    { 1, "Abo Samra", null, true, new DateTime(2024, 2, 24, 19, 32, 9, 573, DateTimeKind.Utc).AddTicks(7859), new DateOnly(1, 1, 1), 1, "Ahmed Samir", false, false, 1, "Samir Sakr " },
                    { 2, "Abo Samra", null, true, new DateTime(2024, 2, 24, 19, 32, 9, 573, DateTimeKind.Utc).AddTicks(7868), new DateOnly(1, 1, 1), 2, "Ahmed Samir", false, false, 2, "Samir Sakr " },
                    { 3, "Abo Samra", null, true, new DateTime(2024, 2, 24, 19, 32, 9, 573, DateTimeKind.Utc).AddTicks(7869), new DateOnly(1, 1, 1), 3, "Ahmed Samir", false, false, 3, "Samir Sakr " },
                    { 4, "Abo Samra", null, true, new DateTime(2024, 2, 24, 19, 32, 9, 573, DateTimeKind.Utc).AddTicks(7870), new DateOnly(1, 1, 1), 4, "Ahmed Samir", false, false, 4, "Samir Sakr " }
                });
        }
    }
}
