using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapplication.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentLeague",
                table: "Teams",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CurrentScore",
                table: "Teams",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Last5Match",
                table: "Teams",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentLeague",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "CurrentScore",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Last5Match",
                table: "Teams");
        }
    }
}
