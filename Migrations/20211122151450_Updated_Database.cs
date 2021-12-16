using Microsoft.EntityFrameworkCore.Migrations;

namespace Kanban.Migrations
{
    public partial class Updated_Database : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "taskTitle",
                table: "Board",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "taskDescription",
                table: "Board",
                newName: "Description");

            migrationBuilder.AddColumn<int>(
                name: "ProjectStatus",
                table: "Board",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Board",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Board_UserId",
                table: "Board",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Board_Users_UserId",
                table: "Board",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Board_Users_UserId",
                table: "Board");

            migrationBuilder.DropIndex(
                name: "IX_Board_UserId",
                table: "Board");

            migrationBuilder.DropColumn(
                name: "ProjectStatus",
                table: "Board");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Board");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Board",
                newName: "taskTitle");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Board",
                newName: "taskDescription");
        }
    }
}
