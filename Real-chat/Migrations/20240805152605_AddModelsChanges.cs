using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Realchat.Migrations
{
    /// <inheritdoc />
    public partial class AddModelsChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "ChatMessages",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ChatMessages",
                newName: "id");
        }
    }
}
