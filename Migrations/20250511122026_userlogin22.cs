using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapplication.Migrations
{
    /// <inheritdoc />
    public partial class userlogin22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LotoPath",
                table: "Teams",
                newName: "LogoPath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LogoPath",
                table: "Teams",
                newName: "LotoPath");
        }
    }
}
