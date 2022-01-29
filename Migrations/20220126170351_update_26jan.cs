using Microsoft.EntityFrameworkCore.Migrations;

namespace Kanban.Migrations
{
    public partial class update_26jan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boards_Users_UserId",
                table: "Boards");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Boards",
                newName: "CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Boards_UserId",
                table: "Boards",
                newName: "IX_Boards_CreatedByUserId");

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "UserBoards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Boards_Users_CreatedByUserId",
                table: "Boards",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boards_Users_CreatedByUserId",
                table: "Boards");

            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "UserBoards");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "Boards",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Boards_CreatedByUserId",
                table: "Boards",
                newName: "IX_Boards_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Boards_Users_UserId",
                table: "Boards",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
