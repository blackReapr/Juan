using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Juan.Data.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_AspNetUsers_UserId1",
                table: "Blogs");

            migrationBuilder.DropForeignKey(
                name: "FK_CartProduct_AspNetUsers_UserId1",
                table: "CartProduct");

            migrationBuilder.DropIndex(
                name: "IX_CartProduct_UserId1",
                table: "CartProduct");

            migrationBuilder.DropIndex(
                name: "IX_Blogs_UserId1",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "CartProduct");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Blogs");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "CartProduct",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "CartProduct",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Blogs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_CartProduct_UserId",
                table: "CartProduct",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_UserId",
                table: "Blogs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_AspNetUsers_UserId",
                table: "Blogs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartProduct_AspNetUsers_UserId",
                table: "CartProduct",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_AspNetUsers_UserId",
                table: "Blogs");

            migrationBuilder.DropForeignKey(
                name: "FK_CartProduct_AspNetUsers_UserId",
                table: "CartProduct");

            migrationBuilder.DropIndex(
                name: "IX_CartProduct_UserId",
                table: "CartProduct");

            migrationBuilder.DropIndex(
                name: "IX_Blogs_UserId",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "Count",
                table: "CartProduct");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "CartProduct",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "CartProduct",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Blogs",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Blogs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartProduct_UserId1",
                table: "CartProduct",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_UserId1",
                table: "Blogs",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_AspNetUsers_UserId1",
                table: "Blogs",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CartProduct_AspNetUsers_UserId1",
                table: "CartProduct",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
