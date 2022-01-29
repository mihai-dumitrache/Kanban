using Microsoft.EntityFrameworkCore.Migrations;

namespace Kanban.Migrations
{
    public partial class updated_mycontext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBoard_Boards_BoardId",
                table: "UserBoard");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBoard_Users_UserId",
                table: "UserBoard");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBoard",
                table: "UserBoard");

            migrationBuilder.RenameTable(
                name: "UserBoard",
                newName: "UserBoards");

            migrationBuilder.RenameIndex(
                name: "IX_UserBoard_UserId",
                table: "UserBoards",
                newName: "IX_UserBoards_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserBoard_BoardId",
                table: "UserBoards",
                newName: "IX_UserBoards_BoardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBoards",
                table: "UserBoards",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBoards_Boards_BoardId",
                table: "UserBoards",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBoards_Users_UserId",
                table: "UserBoards",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBoards_Boards_BoardId",
                table: "UserBoards");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBoards_Users_UserId",
                table: "UserBoards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBoards",
                table: "UserBoards");

            migrationBuilder.RenameTable(
                name: "UserBoards",
                newName: "UserBoard");

            migrationBuilder.RenameIndex(
                name: "IX_UserBoards_UserId",
                table: "UserBoard",
                newName: "IX_UserBoard_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserBoards_BoardId",
                table: "UserBoard",
                newName: "IX_UserBoard_BoardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBoard",
                table: "UserBoard",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBoard_Boards_BoardId",
                table: "UserBoard",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBoard_Users_UserId",
                table: "UserBoard",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
