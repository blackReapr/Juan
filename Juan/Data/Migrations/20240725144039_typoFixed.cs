using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Juan.Data.Migrations
{
    /// <inheritdoc />
    public partial class typoFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Raiting",
                table: "Reviews",
                newName: "Rating");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "Reviews",
                newName: "Raiting");
        }
    }
}
