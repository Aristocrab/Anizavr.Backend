using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Database.Migrations
{
    /// <inheritdoc />
    public partial class EmailFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnimes_Users_UserId",
                table: "UserAnimes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserWatchedAnime_Users_UserId",
                table: "UserWatchedAnime");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserWatchedAnime",
                table: "UserWatchedAnime");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAnimes",
                table: "UserAnimes");

            migrationBuilder.RenameTable(
                name: "UserWatchedAnime",
                newName: "UserWatchedAnimeList");

            migrationBuilder.RenameTable(
                name: "UserAnimes",
                newName: "UserWatchingAnimeList");

            migrationBuilder.RenameIndex(
                name: "IX_UserWatchedAnime_UserId",
                table: "UserWatchedAnimeList",
                newName: "IX_UserWatchedAnimeList_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserAnimes_UserId",
                table: "UserWatchingAnimeList",
                newName: "IX_UserWatchingAnimeList_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserWatchedAnimeList",
                table: "UserWatchedAnimeList",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserWatchingAnimeList",
                table: "UserWatchingAnimeList",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserWatchedAnimeList_Users_UserId",
                table: "UserWatchedAnimeList",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserWatchingAnimeList_Users_UserId",
                table: "UserWatchingAnimeList",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserWatchedAnimeList_Users_UserId",
                table: "UserWatchedAnimeList");

            migrationBuilder.DropForeignKey(
                name: "FK_UserWatchingAnimeList_Users_UserId",
                table: "UserWatchingAnimeList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserWatchingAnimeList",
                table: "UserWatchingAnimeList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserWatchedAnimeList",
                table: "UserWatchedAnimeList");

            migrationBuilder.RenameTable(
                name: "UserWatchingAnimeList",
                newName: "UserAnimes");

            migrationBuilder.RenameTable(
                name: "UserWatchedAnimeList",
                newName: "UserWatchedAnime");

            migrationBuilder.RenameIndex(
                name: "IX_UserWatchingAnimeList_UserId",
                table: "UserAnimes",
                newName: "IX_UserAnimes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserWatchedAnimeList_UserId",
                table: "UserWatchedAnime",
                newName: "IX_UserWatchedAnime_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAnimes",
                table: "UserAnimes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserWatchedAnime",
                table: "UserWatchedAnime",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnimes_Users_UserId",
                table: "UserAnimes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserWatchedAnime_Users_UserId",
                table: "UserWatchedAnime",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
