using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSolicitudEvidenciaInvestigacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolicitudesEvidenciasInvestigacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocenteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocenteCedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TipoEvidencia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TituloProyecto = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    InstitucionFinanciadora = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RolInvestigador = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MesesDuracion = table.Column<int>(type: "int", nullable: false),
                    CodigoProyecto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AreaTematica = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ArchivoNombre = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ArchivoRuta = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ArchivoTamano = table.Column<long>(type: "bigint", nullable: false),
                    ArchivoTipo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ComentariosRevision = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MotivoRechazo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FechaRevision = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ComentariosSolicitud = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesEvidenciasInvestigacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesEvidenciasInvestigacion_Docentes_DocenteId",
                        column: x => x.DocenteId,
                        principalTable: "Docentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesEvidenciasInvestigacion_DocenteCedula",
                table: "SolicitudesEvidenciasInvestigacion",
                column: "DocenteCedula");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesEvidenciasInvestigacion_DocenteId",
                table: "SolicitudesEvidenciasInvestigacion",
                column: "DocenteId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesEvidenciasInvestigacion_Estado",
                table: "SolicitudesEvidenciasInvestigacion",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesEvidenciasInvestigacion_FechaFin",
                table: "SolicitudesEvidenciasInvestigacion",
                column: "FechaFin");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesEvidenciasInvestigacion_FechaInicio",
                table: "SolicitudesEvidenciasInvestigacion",
                column: "FechaInicio");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesEvidenciasInvestigacion_TipoEvidencia",
                table: "SolicitudesEvidenciasInvestigacion",
                column: "TipoEvidencia");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitudesEvidenciasInvestigacion");

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 2, 4, 10, 52, 581, DateTimeKind.Utc).AddTicks(2400), new DateTime(2023, 7, 2, 4, 10, 52, 581, DateTimeKind.Utc).AddTicks(2398) });

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("8ef569a9-342c-4e85-a8e1-29b5e697f2b6"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 2, 4, 10, 52, 581, DateTimeKind.Utc).AddTicks(2389), new DateTime(2020, 7, 2, 4, 10, 52, 581, DateTimeKind.Utc).AddTicks(1893) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("c24cd969-b99a-4354-b49f-0cae93b0b7ad"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 2, 4, 10, 52, 479, DateTimeKind.Utc).AddTicks(141), "$2a$11$VSOdlxPrxXNrnUDfL91O5ubr1GhJCGTjC/OQKVPAlQ4AYZ4ZTW.R6", new DateTime(2020, 7, 2, 4, 10, 52, 478, DateTimeKind.Utc).AddTicks(9841) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 2, 4, 10, 52, 580, DateTimeKind.Utc).AddTicks(2858), "$2a$11$LhCiomd5yHw3cruoOlWPAOIYMkmJFpL6ktTAR4HoKfuGHoxHOm0hK", new DateTime(2025, 6, 2, 4, 10, 52, 580, DateTimeKind.Utc).AddTicks(2746) });
        }
    }
}
