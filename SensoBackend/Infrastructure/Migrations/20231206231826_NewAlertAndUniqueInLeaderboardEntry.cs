using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SensoBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewAlertAndUniqueInLeaderboardEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LeaderboardEntries_AccountId",
                table: "LeaderboardEntries");

            migrationBuilder.CreateIndex(
                name: "IX_LeaderboardEntries_AccountId_Game",
                table: "LeaderboardEntries",
                columns: new[] { "AccountId", "Game" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LeaderboardEntries_AccountId_Game",
                table: "LeaderboardEntries");

            migrationBuilder.CreateIndex(
                name: "IX_LeaderboardEntries_AccountId",
                table: "LeaderboardEntries",
                column: "AccountId");
        }
    }
}
