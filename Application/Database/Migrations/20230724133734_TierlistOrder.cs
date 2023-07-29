using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Database.Migrations
{
    /// <inheritdoc />
    public partial class TierlistOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "Tierlist",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "Tierlist");
        }
    }
}
