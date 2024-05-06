using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.Infrastructe.Migrations
{
    /// <inheritdoc />
    public partial class AddPost2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PictureUrl",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureUrl",
                table: "Posts");
        }
    }
}
