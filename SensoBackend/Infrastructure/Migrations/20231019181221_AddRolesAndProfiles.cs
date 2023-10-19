using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SensoBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesAndProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "Accounts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    SeniorId = table.Column<int>(type: "integer", nullable: false),
                    Alias = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profiles_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Profiles_Accounts_SeniorId",
                        column: x => x.SeniorId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Member" }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Active", "CreatedAt", "DisplayName", "Email", "LastLoginAt", "LastPasswordChangeAt", "Password", "PhoneNumber", "RoleId", "Verified" },
                values: new object[,]
                {
                    { 1, true, new DateTimeOffset(new DateTime(2023, 10, 19, 18, 12, 21, 72, DateTimeKind.Unspecified).AddTicks(6517), new TimeSpan(0, 0, 0, 0, 0)), "admin_senso", "admin@senso.pl", new DateTimeOffset(new DateTime(2023, 10, 19, 18, 12, 21, 72, DateTimeKind.Unspecified).AddTicks(6517), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2023, 10, 19, 18, 12, 21, 72, DateTimeKind.Unspecified).AddTicks(6517), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$2U.MLkSys/NuC5JXy.Hsie.XSMlhamOammkHZBuWAqIR7cPbyCSVO", "123456789", 1, true },
                    { 2, true, new DateTimeOffset(new DateTime(2023, 10, 19, 18, 12, 21, 201, DateTimeKind.Unspecified).AddTicks(6130), new TimeSpan(0, 0, 0, 0, 0)), "senior_senso", "senior@senso.pl", new DateTimeOffset(new DateTime(2023, 10, 19, 18, 12, 21, 201, DateTimeKind.Unspecified).AddTicks(6130), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2023, 10, 19, 18, 12, 21, 201, DateTimeKind.Unspecified).AddTicks(6130), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$ElheHSU35MNH438lzLMgge1d5LOiO2ByC6f6Mh74PTQXCeQHFrkOe", "123456789", 2, true },
                    { 3, true, new DateTimeOffset(new DateTime(2023, 10, 19, 18, 12, 21, 331, DateTimeKind.Unspecified).AddTicks(9750), new TimeSpan(0, 0, 0, 0, 0)), "caretaker_senso", "caretaker@senso.pl", new DateTimeOffset(new DateTime(2023, 10, 19, 18, 12, 21, 331, DateTimeKind.Unspecified).AddTicks(9750), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2023, 10, 19, 18, 12, 21, 331, DateTimeKind.Unspecified).AddTicks(9750), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$qBumOFo2yqLQ5Sbap0SmpOvlZcTI7pHBno03B.U4XLlUkeR0iyFaS", "123456789", 2, true }
                });

            migrationBuilder.InsertData(
                table: "Profiles",
                columns: new[] { "Id", "AccountId", "Alias", "SeniorId" },
                values: new object[,]
                {
                    { 1, 2, null, 2 },
                    { 2, 3, "Senior", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_RoleId",
                table: "Accounts",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_AccountId",
                table: "Profiles",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_SeniorId",
                table: "Profiles",
                column: "SeniorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Roles_RoleId",
                table: "Accounts",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Roles_RoleId",
                table: "Accounts");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_RoleId",
                table: "Accounts");

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Accounts");
        }
    }
}
