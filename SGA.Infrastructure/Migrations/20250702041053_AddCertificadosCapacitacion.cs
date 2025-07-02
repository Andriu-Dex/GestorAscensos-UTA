using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCertificadosCapacitacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolicitudesCertificadosCapacitacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocenteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocenteCedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NombreCurso = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    InstitucionOfertante = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TipoCapacitacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HorasDuracion = table.Column<int>(type: "int", nullable: false),
                    Modalidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NumeroRegistro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AreaTematica = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ArchivoNombre = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ArchivoRuta = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ArchivoTipo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ArchivoTamano = table.Column<long>(type: "bigint", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ComentariosRevision = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RevisadoPorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FechaRevision = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MotivoRechazo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ComentariosSolicitud = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SolicitudGrupoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesCertificadosCapacitacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesCertificadosCapacitacion_Docentes_DocenteId",
                        column: x => x.DocenteId,
                        principalTable: "Docentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesCertificadosCapacitacion_Usuarios_RevisadoPorId",
                        column: x => x.RevisadoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesCertificadosCapacitacion_DocenteCedula",
                table: "SolicitudesCertificadosCapacitacion",
                column: "DocenteCedula");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesCertificadosCapacitacion_DocenteId",
                table: "SolicitudesCertificadosCapacitacion",
                column: "DocenteId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesCertificadosCapacitacion_Estado",
                table: "SolicitudesCertificadosCapacitacion",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesCertificadosCapacitacion_FechaFin",
                table: "SolicitudesCertificadosCapacitacion",
                column: "FechaFin");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesCertificadosCapacitacion_FechaInicio",
                table: "SolicitudesCertificadosCapacitacion",
                column: "FechaInicio");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesCertificadosCapacitacion_RevisadoPorId",
                table: "SolicitudesCertificadosCapacitacion",
                column: "RevisadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesCertificadosCapacitacion_SolicitudGrupoId",
                table: "SolicitudesCertificadosCapacitacion",
                column: "SolicitudGrupoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitudesCertificadosCapacitacion");

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 1, 18, 35, 26, 745, DateTimeKind.Utc).AddTicks(2777), new DateTime(2023, 7, 1, 18, 35, 26, 745, DateTimeKind.Utc).AddTicks(2775) });

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("8ef569a9-342c-4e85-a8e1-29b5e697f2b6"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 1, 18, 35, 26, 745, DateTimeKind.Utc).AddTicks(2767), new DateTime(2020, 7, 1, 18, 35, 26, 745, DateTimeKind.Utc).AddTicks(2495) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("c24cd969-b99a-4354-b49f-0cae93b0b7ad"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 1, 18, 35, 26, 641, DateTimeKind.Utc).AddTicks(1190), "$2a$11$yX3Z3Qbj1C.vvJXXbaJhaeF7JypS.DZZzEF9DoPrCYgOKZuD0e4Au", new DateTime(2020, 7, 1, 18, 35, 26, 641, DateTimeKind.Utc).AddTicks(958) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 1, 18, 35, 26, 744, DateTimeKind.Utc).AddTicks(6713), "$2a$11$trrANMQH8/7bG4Ai1AsdxuNOVm22DlNTLz5pUiARpW81jOo48qer2", new DateTime(2025, 6, 1, 18, 35, 26, 744, DateTimeKind.Utc).AddTicks(6633) });
        }
    }
}
