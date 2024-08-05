using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Realchat.Migrations
{
    /// <inheritdoc />
    public partial class AddChatRoomToChatMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChatRoom",
                table: "ChatMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatRoom",
                table: "ChatMessages");
        }
    }
}
