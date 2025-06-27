using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevLife.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddHighScoreToScores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HighScore",
                table: "Scores",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HighScore",
                table: "Scores");
        }
    }
}
