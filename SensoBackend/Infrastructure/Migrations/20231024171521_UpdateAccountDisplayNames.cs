using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SensoBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAccountDisplayNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "DisplayName",
                keyValue: null,
                column: "DisplayName",
                value: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //fck this
        }
    }
}
