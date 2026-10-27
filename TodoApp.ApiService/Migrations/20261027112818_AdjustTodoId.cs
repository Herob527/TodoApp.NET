using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoApp.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class AdjustTodoId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Todos_AspNetUsers_OwnerId1",
                table: "Todos");

            migrationBuilder.DropIndex(
                name: "IX_Todos_OwnerId1",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "OwnerId1",
                table: "Todos");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Todos",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Todos_OwnerId",
                table: "Todos",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_AspNetUsers_OwnerId",
                table: "Todos",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Todos_AspNetUsers_OwnerId",
                table: "Todos");

            migrationBuilder.DropIndex(
                name: "IX_Todos_OwnerId",
                table: "Todos");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Todos",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "OwnerId1",
                table: "Todos",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Todos_OwnerId1",
                table: "Todos",
                column: "OwnerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_AspNetUsers_OwnerId1",
                table: "Todos",
                column: "OwnerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
