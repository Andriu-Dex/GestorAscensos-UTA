using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigracionPDFCompresionBD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArchivoRuta",
                table: "SolicitudesEvidenciasInvestigacion");

            migrationBuilder.DropColumn(
                name: "ArchivoRuta",
                table: "SolicitudesCertificadosCapacitacion");

            migrationBuilder.AddColumn<byte[]>(
                name: "ArchivoContenido",
                table: "SolicitudesEvidenciasInvestigacion",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "ArchivoEstaComprimido",
                table: "SolicitudesEvidenciasInvestigacion",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<long>(
                name: "ArchivoTamanoComprimido",
                table: "SolicitudesEvidenciasInvestigacion",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<byte[]>(
                name: "ArchivoContenido",
                table: "SolicitudesCertificadosCapacitacion",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ArchivoEstaComprimido",
                table: "SolicitudesCertificadosCapacitacion",
                type: "bit",
                nullable: true,
                defaultValue: true);

            migrationBuilder.AddColumn<long>(
                name: "ArchivoTamanoComprimido",
                table: "SolicitudesCertificadosCapacitacion",
                type: "bigint",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 3, 7, 21, 47, 56, DateTimeKind.Utc).AddTicks(5679), new DateTime(2023, 7, 3, 7, 21, 47, 56, DateTimeKind.Utc).AddTicks(5678) });

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("8ef569a9-342c-4e85-a8e1-29b5e697f2b6"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 3, 7, 21, 47, 56, DateTimeKind.Utc).AddTicks(5664), new DateTime(2020, 7, 3, 7, 21, 47, 56, DateTimeKind.Utc).AddTicks(5381) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("c24cd969-b99a-4354-b49f-0cae93b0b7ad"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 3, 7, 21, 46, 947, DateTimeKind.Utc).AddTicks(7238), "$2a$11$A8stdfxsLMiqfpxdvex50eHgKn2p9nQygY6wosdIz5lVdKvZ.8jIW", new DateTime(2020, 7, 3, 7, 21, 46, 947, DateTimeKind.Utc).AddTicks(6938) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 3, 7, 21, 47, 55, DateTimeKind.Utc).AddTicks(8610), "$2a$11$XjRAs2CiFOrVgyFg6cc5ZuQKVuE/Fvd1cDlVxrTxv.yVX8ZSpQhjy", new DateTime(2025, 6, 3, 7, 21, 47, 55, DateTimeKind.Utc).AddTicks(8494) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArchivoContenido",
                table: "SolicitudesEvidenciasInvestigacion");

            migrationBuilder.DropColumn(
                name: "ArchivoEstaComprimido",
                table: "SolicitudesEvidenciasInvestigacion");

            migrationBuilder.DropColumn(
                name: "ArchivoTamanoComprimido",
                table: "SolicitudesEvidenciasInvestigacion");

            migrationBuilder.DropColumn(
                name: "ArchivoContenido",
                table: "SolicitudesCertificadosCapacitacion");

            migrationBuilder.DropColumn(
                name: "ArchivoEstaComprimido",
                table: "SolicitudesCertificadosCapacitacion");

            migrationBuilder.DropColumn(
                name: "ArchivoTamanoComprimido",
                table: "SolicitudesCertificadosCapacitacion");

            migrationBuilder.AddColumn<string>(
                name: "ArchivoRuta",
                table: "SolicitudesEvidenciasInvestigacion",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ArchivoRuta",
                table: "SolicitudesCertificadosCapacitacion",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 2, 20, 22, 35, 680, DateTimeKind.Utc).AddTicks(6466), new DateTime(2023, 7, 2, 20, 22, 35, 680, DateTimeKind.Utc).AddTicks(6465) });

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("8ef569a9-342c-4e85-a8e1-29b5e697f2b6"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 2, 20, 22, 35, 680, DateTimeKind.Utc).AddTicks(6419), new DateTime(2020, 7, 2, 20, 22, 35, 680, DateTimeKind.Utc).AddTicks(6127) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("c24cd969-b99a-4354-b49f-0cae93b0b7ad"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 2, 20, 22, 35, 576, DateTimeKind.Utc).AddTicks(8159), "$2a$11$2zSw5Tjp5HxUEZmdkRBVt.xX9OjmQ7HxYUzO7uOFj21qr4LR1KMZC", new DateTime(2020, 7, 2, 20, 22, 35, 576, DateTimeKind.Utc).AddTicks(7809) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 2, 20, 22, 35, 679, DateTimeKind.Utc).AddTicks(9115), "$2a$11$aHJaDb9RgRfHiIYu04m9benmSHPuVGH5qfQ0u2B5Tz3XX9Xtr.NNa", new DateTime(2025, 6, 2, 20, 22, 35, 679, DateTimeKind.Utc).AddTicks(8993) });
        }
    }
}
