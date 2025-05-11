using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapplication.Migrations
{
    /// <inheritdoc />
    public partial class userlogin21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LotoPath",
                table: "Teams",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LotoPath",
                table: "Teams");
        }
    }
}
