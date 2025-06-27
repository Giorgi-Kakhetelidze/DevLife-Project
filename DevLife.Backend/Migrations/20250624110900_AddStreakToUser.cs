using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevLife.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddStreakToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastCorrectGuessDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Streak",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastCorrectGuessDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Streak",
                table: "Users");
        }
    }
}
