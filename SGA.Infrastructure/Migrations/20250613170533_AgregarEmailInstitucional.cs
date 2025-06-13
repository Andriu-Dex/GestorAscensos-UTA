using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEmailInstitucional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FechaHora",
                table: "LogsAuditoria",
                newName: "FechaRegistro");

            migrationBuilder.AddColumn<int>(
                name: "TipoLog",
                table: "LogsAuditoria",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EmailInstitucional",
                table: "DatosTTHH",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoLog",
                table: "LogsAuditoria");

            migrationBuilder.DropColumn(
                name: "EmailInstitucional",
                table: "DatosTTHH");

            migrationBuilder.RenameColumn(
                name: "FechaRegistro",
                table: "LogsAuditoria",
                newName: "FechaHora");
        }
    }
}
