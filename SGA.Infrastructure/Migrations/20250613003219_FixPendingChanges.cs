using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$mRI2S8DXP3oWwRo5i.SSRuSRKVTQlSFWJZK.jRsrL6Ex38PAHgzNC");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$N8qvJ3F5k2lQ8vJ9x5G2H.QJKzF3FJGTj8K5vQ3F2h7Bx8vN3F5k2");
        }
    }
}
