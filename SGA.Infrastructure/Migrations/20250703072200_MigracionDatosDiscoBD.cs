using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigracionDatosDiscoBD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Agregar nuevos campos para almacenamiento de PDFs en BD - SolicitudCertificadoCapacitacion
            migrationBuilder.AddColumn<byte[]>(
                name: "ArchivoContenido",
                table: "SolicitudesCertificadosCapacitacion",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ArchivoTamanoComprimido",
                table: "SolicitudesCertificadosCapacitacion",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ArchivoEstaComprimido",
                table: "SolicitudesCertificadosCapacitacion",
                type: "bit",
                nullable: true,
                defaultValue: true);

            // Agregar nuevos campos para almacenamiento de PDFs en BD - SolicitudEvidenciaInvestigacion
            migrationBuilder.AddColumn<byte[]>(
                name: "ArchivoContenido",
                table: "SolicitudesEvidenciasInvestigacion",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ArchivoTamanoComprimido",
                table: "SolicitudesEvidenciasInvestigacion",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ArchivoEstaComprimido",
                table: "SolicitudesEvidenciasInvestigacion",
                type: "bit",
                nullable: true,
                defaultValue: true);

            // Eliminar las columnas de ArchivoRuta después de migrar los datos
            // NOTA: Los datos deben ser migrados ANTES de ejecutar estas líneas
            // Descomenta estas líneas después de ejecutar el script de migración de datos
            
            /*
            migrationBuilder.DropColumn(
                name: "ArchivoRuta",
                table: "SolicitudesCertificadosCapacitacion");

            migrationBuilder.DropColumn(
                name: "ArchivoRuta",
                table: "SolicitudesEvidenciasInvestigacion");
            */
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revertir la migración - agregar de vuelta las columnas ArchivoRuta
            migrationBuilder.AddColumn<string>(
                name: "ArchivoRuta",
                table: "SolicitudesCertificadosCapacitacion",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchivoRuta",
                table: "SolicitudesEvidenciasInvestigacion",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            // Eliminar las nuevas columnas de BD
            migrationBuilder.DropColumn(
                name: "ArchivoContenido",
                table: "SolicitudesCertificadosCapacitacion");

            migrationBuilder.DropColumn(
                name: "ArchivoTamanoComprimido",
                table: "SolicitudesCertificadosCapacitacion");

            migrationBuilder.DropColumn(
                name: "ArchivoEstaComprimido",
                table: "SolicitudesCertificadosCapacitacion");

            migrationBuilder.DropColumn(
                name: "ArchivoContenido",
                table: "SolicitudesEvidenciasInvestigacion");

            migrationBuilder.DropColumn(
                name: "ArchivoTamanoComprimido",
                table: "SolicitudesEvidenciasInvestigacion");

            migrationBuilder.DropColumn(
                name: "ArchivoEstaComprimido",
                table: "SolicitudesEvidenciasInvestigacion");
        }
    }
}
