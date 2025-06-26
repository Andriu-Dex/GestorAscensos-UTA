using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGA.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogsAuditoria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EntidadAfectada = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ValoresAnteriores = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValoresNuevos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DireccionIP = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    FechaAccion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogsAuditoria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstaActivo = table.Column<bool>(type: "bit", nullable: false),
                    IntentosLogin = table.Column<int>(type: "int", nullable: false),
                    UltimoBloqueado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UltimoLogin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Docentes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NivelActual = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaInicioNivelActual = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaUltimoAscenso = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstaActivo = table.Column<bool>(type: "bit", nullable: false),
                    FechaNombramiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PromedioEvaluaciones = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    HorasCapacitacion = table.Column<int>(type: "int", nullable: true),
                    NumeroObrasAcademicas = table.Column<int>(type: "int", nullable: true),
                    MesesInvestigacion = table.Column<int>(type: "int", nullable: true),
                    FechaUltimaImportacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Docentes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Docentes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesAscenso",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocenteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NivelActual = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NivelSolicitado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotivoRechazo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAprobacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AprobadoPorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PromedioEvaluaciones = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    HorasCapacitacion = table.Column<int>(type: "int", nullable: false),
                    NumeroObrasAcademicas = table.Column<int>(type: "int", nullable: false),
                    MesesInvestigacion = table.Column<int>(type: "int", nullable: false),
                    TiempoEnNivelDias = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesAscenso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesAscenso_Docentes_DocenteId",
                        column: x => x.DocenteId,
                        principalTable: "Docentes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SolicitudesAscenso_Usuarios_AprobadoPorId",
                        column: x => x.AprobadoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Documentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombreArchivo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RutaArchivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TamanoArchivo = table.Column<long>(type: "bigint", nullable: false),
                    TipoDocumento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContenidoArchivo = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SolicitudAscensoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documentos_SolicitudesAscenso_SolicitudAscensoId",
                        column: x => x.SolicitudAscensoId,
                        principalTable: "SolicitudesAscenso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Email", "EstaActivo", "FechaCreacion", "FechaModificacion", "IntentosLogin", "PasswordHash", "Rol", "UltimoBloqueado", "UltimoLogin" },
                values: new object[] { new Guid("6fa6dce8-66df-465c-a79c-0f6e2d2651c1"), "admin@uta.edu.ec", true, new DateTime(2025, 6, 25, 21, 26, 12, 45, DateTimeKind.Utc).AddTicks(621), null, 0, "$2a$11$eWDAhgGAMRNpUkpUHlnwaefg3uyYXXV8CUkLZU76AaiP1bFNcUZuC", "Administrador", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "Docentes",
                columns: new[] { "Id", "Apellidos", "Cedula", "Email", "EstaActivo", "FechaCreacion", "FechaInicioNivelActual", "FechaModificacion", "FechaNombramiento", "FechaUltimaImportacion", "FechaUltimoAscenso", "HorasCapacitacion", "MesesInvestigacion", "NivelActual", "Nombres", "NumeroObrasAcademicas", "PromedioEvaluaciones", "UsuarioId" },
                values: new object[] { new Guid("2f3111eb-1567-48f9-a438-d90493df25f1"), "Sistema", "1800000000", "admin@uta.edu.ec", true, new DateTime(2025, 6, 25, 21, 26, 12, 45, DateTimeKind.Utc).AddTicks(6624), new DateTime(2020, 6, 25, 21, 26, 12, 45, DateTimeKind.Utc).AddTicks(6273), null, null, null, null, null, null, "Titular5", "Administrador", null, null, new Guid("6fa6dce8-66df-465c-a79c-0f6e2d2651c1") });

            migrationBuilder.CreateIndex(
                name: "IX_Docentes_Cedula",
                table: "Docentes",
                column: "Cedula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Docentes_Email",
                table: "Docentes",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Docentes_UsuarioId",
                table: "Docentes",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_SolicitudAscensoId",
                table: "Documentos",
                column: "SolicitudAscensoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesAscenso_AprobadoPorId",
                table: "SolicitudesAscenso",
                column: "AprobadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesAscenso_DocenteId",
                table: "SolicitudesAscenso",
                column: "DocenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documentos");

            migrationBuilder.DropTable(
                name: "LogsAuditoria");

            migrationBuilder.DropTable(
                name: "SolicitudesAscenso");

            migrationBuilder.DropTable(
                name: "Docentes");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
