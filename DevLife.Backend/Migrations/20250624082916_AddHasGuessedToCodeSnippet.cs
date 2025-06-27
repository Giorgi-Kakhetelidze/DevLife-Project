using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevLife.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddHasGuessedToCodeSnippet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasGuessed",
                table: "CodeSnippets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "CodeSnippets",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasGuessed",
                table: "CodeSnippets");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CodeSnippets");
        }
    }
}
