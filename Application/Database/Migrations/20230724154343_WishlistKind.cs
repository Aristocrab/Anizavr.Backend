using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Database.Migrations
{
    /// <inheritdoc />
    public partial class WishlistKind : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Kind",
                table: "Wishlist",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Kind",
                table: "Wishlist");
        }
    }
}
