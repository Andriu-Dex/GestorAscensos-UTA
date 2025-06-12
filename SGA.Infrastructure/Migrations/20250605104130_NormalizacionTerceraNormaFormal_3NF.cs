using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NormalizacionTerceraNormaFormal_3NF : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesAscenso_Docentes_DocenteId",
                table: "SolicitudesAscenso");

            migrationBuilder.DropColumn(
                name: "Facultad",
                table: "Docentes");

            migrationBuilder.DropColumn(
                name: "HorasCapacitacion",
                table: "Docentes");

            migrationBuilder.DropColumn(
                name: "NumeroObras",
                table: "Docentes");

            migrationBuilder.DropColumn(
                name: "PuntajeEvaluacion",
                table: "Docentes");

            migrationBuilder.DropColumn(
                name: "TiempoEnRolActual",
                table: "Docentes");

            migrationBuilder.DropColumn(
                name: "Facultad",
                table: "DatosTTHH");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "SolicitudesAscenso",
                newName: "EstadoSolicitudId");

            migrationBuilder.RenameColumn(
                name: "Tipo",
                table: "Documentos",
                newName: "TipoDocumentoId");

            migrationBuilder.RenameColumn(
                name: "TiempoInvestigacion",
                table: "Docentes",
                newName: "FacultadId");

            migrationBuilder.AlterColumn<decimal>(
                name: "PuntajeEvaluacion",
                table: "SolicitudesAscenso",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "MotivoRechazo",
                table: "SolicitudesAscenso",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "CumpleCapacitacion",
                table: "SolicitudesAscenso",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CumpleEvaluacion",
                table: "SolicitudesAscenso",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CumpleInvestigacion",
                table: "SolicitudesAscenso",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CumpleObras",
                table: "SolicitudesAscenso",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CumpleTiempo",
                table: "SolicitudesAscenso",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "SolicitudesAscenso",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ObservacionesRevisor",
                table: "SolicitudesAscenso",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsObligatorio",
                table: "DocumentosSolicitud",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAsociacion",
                table: "DocumentosSolicitud",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Observaciones",
                table: "DocumentosSolicitud",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Documentos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Documentos",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "Documentos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Documentos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "Documentos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaValidacion",
                table: "Documentos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HashSHA256",
                table: "Documentos",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservacionesValidacion",
                table: "Documentos",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Validado",
                table: "Documentos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ValidadoPorId",
                table: "Documentos",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TelefonoContacto",
                table: "Docentes",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Docentes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombres",
                table: "Docentes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NombreUsuario",
                table: "Docentes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Docentes",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Cedula",
                table: "Docentes",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Apellidos",
                table: "Docentes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Docentes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaBaja",
                table: "Docentes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoBaja",
                table: "Docentes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombres",
                table: "DatosTTHH",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Cedula",
                table: "DatosTTHH",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Apellidos",
                table: "DatosTTHH",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "DatosTTHH",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Celular",
                table: "DatosTTHH",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "DatosTTHH",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailPersonal",
                table: "DatosTTHH",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstadoCivil",
                table: "DatosTTHH",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FacultadId",
                table: "DatosTTHH",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "DatosTTHH",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaIngreso",
                table: "DatosTTHH",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaNacimiento",
                table: "DatosTTHH",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefonoConvencional",
                table: "DatosTTHH",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ConfiguracionesSistema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Clave = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Valor = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TipoDato = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GrupoConfiguracion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EsEditable = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesSistema", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosSolicitud",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EsEstadoFinal = table.Column<bool>(type: "bit", nullable: false),
                    ColorHex = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosSolicitud", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Facultades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Activa = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facultades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IndicadoresDocente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocenteId = table.Column<int>(type: "int", nullable: false),
                    TiempoEnRolActual = table.Column<int>(type: "int", nullable: false),
                    NumeroObras = table.Column<int>(type: "int", nullable: false),
                    PuntajeEvaluacion = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    HorasCapacitacion = table.Column<int>(type: "int", nullable: false),
                    TiempoInvestigacion = table.Column<int>(type: "int", nullable: false),
                    FechaActualizacionObras = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaActualizacionEvaluacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaActualizacionCapacitacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaActualizacionInvestigacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FuenteObras = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FuenteEvaluacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FuenteCapacitacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FuenteInvestigacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndicadoresDocente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndicadoresDocente_Docentes_DocenteId",
                        column: x => x.DocenteId,
                        principalTable: "Docentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LogsAuditoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocenteId = table.Column<int>(type: "int", nullable: true),
                    Accion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Entidad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EntidadId = table.Column<int>(type: "int", nullable: true),
                    ValoresAnteriores = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValoresNuevos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DireccionIP = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogsAuditoria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogsAuditoria_Docentes_DocenteId",
                        column: x => x.DocenteId,
                        principalTable: "Docentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ServiciosExternos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UrlBase = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ApiKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TimeoutSegundos = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaUltimaConexion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UltimoError = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiciosExternos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposDocumento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequiereValidacion = table.Column<bool>(type: "bit", nullable: false),
                    FormatoEsperado = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TamanoMaximoMB = table.Column<int>(type: "int", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposDocumento", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ConfiguracionesSistema",
                columns: new[] { "Id", "Clave", "Descripcion", "EsEditable", "FechaActualizacion", "FechaCreacion", "GrupoConfiguracion", "TipoDato", "Valor" },
                values: new object[,]
                {
                    { 1, "TIEMPO_BLOQUEO_MINUTOS", "Tiempo de bloqueo en minutos después de 3 intentos fallidos", true, new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(7100), new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(7033), "Seguridad", "int", "15" },
                    { 2, "INTENTOS_MAXIMOS_LOGIN", "Número máximo de intentos de login antes del bloqueo", true, new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(7170), new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(7169), "Seguridad", "int", "3" },
                    { 3, "TOKEN_EXPIRACION_HORAS", "Tiempo de expiración del token JWT en horas", true, new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(7172), new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(7172), "Seguridad", "int", "8" },
                    { 4, "TAMAÑO_MAXIMO_ARCHIVO_MB", "Tamaño máximo permitido para archivos en MB", true, new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(7174), new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(7174), "Documentos", "int", "20" }
                });

            migrationBuilder.InsertData(
                table: "EstadosSolicitud",
                columns: new[] { "Id", "Activo", "Codigo", "ColorHex", "Descripcion", "EsEstadoFinal", "FechaCreacion", "Nombre", "Orden" },
                values: new object[,]
                {
                    { 1, true, "ENVIADA", "#FFA500", "Solicitud enviada y pendiente de revisión", false, new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(5353), "Enviada", 1 },
                    { 2, true, "EN_PROCESO", "#0066CC", "Solicitud en proceso de revisión", false, new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(5424), "En Proceso", 2 },
                    { 3, true, "APROBADA", "#28A745", "Solicitud aprobada", true, new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(5486), "Aprobada", 3 },
                    { 4, true, "RECHAZADA", "#DC3545", "Solicitud rechazada", true, new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(5488), "Rechazada", 4 },
                    { 5, true, "ARCHIVADA", "#6C757D", "Solicitud archivada", true, new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(5489), "Archivada", 5 }
                });

            migrationBuilder.InsertData(
                table: "Facultades",
                columns: new[] { "Id", "Activa", "Codigo", "Descripcion", "FechaCreacion", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "FISEI", null, new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(83), "Facultad de Ingeniería en Sistemas, Electrónica e Industrial" },
                    { 2, true, "FCIAL", null, new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(191), "Facultad de Ciencias de la Alimentación" },
                    { 3, true, "FCHE", null, new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(193), "Facultad de Ciencias Humanas y de la Educación" },
                    { 4, true, "FCA", null, new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(194), "Facultad de Contabilidad y Auditoría" },
                    { 5, true, "FCJSE", null, new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(195), "Facultad de Ciencias Jurídicas y Sociales" }
                });

            migrationBuilder.InsertData(
                table: "ServiciosExternos",
                columns: new[] { "Id", "Activo", "ApiKey", "Codigo", "Descripcion", "FechaCreacion", "FechaUltimaConexion", "Nombre", "TimeoutSegundos", "UltimoError", "UrlBase" },
                values: new object[,]
                {
                    { 1, true, null, "DITIC", "Servicio para obtener datos de capacitación", new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(6280), null, "DITIC - Cursos", 30, null, "https://api.ditic.uta.edu.ec" },
                    { 2, true, null, "DAC", "Servicio para obtener evaluaciones docente", new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(6357), null, "DAC - Evaluaciones", 30, null, "https://api.dac.uta.edu.ec" },
                    { 3, true, null, "TTHH", "Servicio de Talento Humano", new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(6359), null, "TTHH - Acción Personal", 30, null, "https://api.tthh.uta.edu.ec" },
                    { 4, true, null, "INVESTIGACION", "Servicio para obtener datos de investigación", new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(6360), null, "Dirección de Investigación", 30, null, "https://api.investigacion.uta.edu.ec" }
                });

            migrationBuilder.InsertData(
                table: "TiposDocumento",
                columns: new[] { "Id", "Activo", "Codigo", "Descripcion", "FechaCreacion", "FormatoEsperado", "Nombre", "RequiereValidacion", "TamanoMaximoMB" },
                values: new object[,]
                {
                    { 1, true, "OBRA", "Documentos que acreditan obras publicadas", new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(4372), "PDF", "Obra Publicada", false, 10 },
                    { 2, true, "CAPACITACION", "Certificados de capacitación y cursos", new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(4441), "PDF", "Capacitación", false, 5 },
                    { 3, true, "INVESTIGACION", "Documentos relacionados a proyectos de investigación", new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(4443), "PDF", "Investigación", false, 15 },
                    { 4, true, "EVALUACION", "Resultados de evaluación docente", new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(4445), "PDF", "Evaluación Docente", false, 5 },
                    { 5, true, "ACCION_PERSONAL", "Documentos de TTHH", new DateTime(2025, 6, 5, 5, 41, 30, 279, DateTimeKind.Local).AddTicks(4446), "PDF", "Acción de Personal", false, 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesAscenso_EstadoSolicitudId",
                table: "SolicitudesAscenso",
                column: "EstadoSolicitudId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesAscenso_RevisorId",
                table: "SolicitudesAscenso",
                column: "RevisorId");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_TipoDocumentoId",
                table: "Documentos",
                column: "TipoDocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_ValidadoPorId",
                table: "Documentos",
                column: "ValidadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Docentes_Email",
                table: "Docentes",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Docentes_FacultadId",
                table: "Docentes",
                column: "FacultadId");

            migrationBuilder.CreateIndex(
                name: "IX_DatosTTHH_FacultadId",
                table: "DatosTTHH",
                column: "FacultadId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesSistema_Clave",
                table: "ConfiguracionesSistema",
                column: "Clave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadosSolicitud_Codigo",
                table: "EstadosSolicitud",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Facultades_Codigo",
                table: "Facultades",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IndicadoresDocente_DocenteId",
                table: "IndicadoresDocente",
                column: "DocenteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogsAuditoria_DocenteId",
                table: "LogsAuditoria",
                column: "DocenteId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosExternos_Codigo",
                table: "ServiciosExternos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposDocumento_Codigo",
                table: "TiposDocumento",
                column: "Codigo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DatosTTHH_Facultades_FacultadId",
                table: "DatosTTHH",
                column: "FacultadId",
                principalTable: "Facultades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Docentes_Facultades_FacultadId",
                table: "Docentes",
                column: "FacultadId",
                principalTable: "Facultades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documentos_Docentes_ValidadoPorId",
                table: "Documentos",
                column: "ValidadoPorId",
                principalTable: "Docentes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Documentos_TiposDocumento_TipoDocumentoId",
                table: "Documentos",
                column: "TipoDocumentoId",
                principalTable: "TiposDocumento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesAscenso_Docentes_DocenteId",
                table: "SolicitudesAscenso",
                column: "DocenteId",
                principalTable: "Docentes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesAscenso_Docentes_RevisorId",
                table: "SolicitudesAscenso",
                column: "RevisorId",
                principalTable: "Docentes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesAscenso_EstadosSolicitud_EstadoSolicitudId",
                table: "SolicitudesAscenso",
                column: "EstadoSolicitudId",
                principalTable: "EstadosSolicitud",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatosTTHH_Facultades_FacultadId",
                table: "DatosTTHH");

            migrationBuilder.DropForeignKey(
                name: "FK_Docentes_Facultades_FacultadId",
                table: "Docentes");

            migrationBuilder.DropForeignKey(
                name: "FK_Documentos_Docentes_ValidadoPorId",
                table: "Documentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Documentos_TiposDocumento_TipoDocumentoId",
                table: "Documentos");

            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesAscenso_Docentes_DocenteId",
                table: "SolicitudesAscenso");

            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesAscenso_Docentes_RevisorId",
                table: "SolicitudesAscenso");

            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesAscenso_EstadosSolicitud_EstadoSolicitudId",
                table: "SolicitudesAscenso");

            migrationBuilder.DropTable(
                name: "ConfiguracionesSistema");

            migrationBuilder.DropTable(
                name: "EstadosSolicitud");

            migrationBuilder.DropTable(
                name: "Facultades");

            migrationBuilder.DropTable(
                name: "IndicadoresDocente");

            migrationBuilder.DropTable(
                name: "LogsAuditoria");

            migrationBuilder.DropTable(
                name: "ServiciosExternos");

            migrationBuilder.DropTable(
                name: "TiposDocumento");

            migrationBuilder.DropIndex(
                name: "IX_SolicitudesAscenso_EstadoSolicitudId",
                table: "SolicitudesAscenso");

            migrationBuilder.DropIndex(
                name: "IX_SolicitudesAscenso_RevisorId",
                table: "SolicitudesAscenso");

            migrationBuilder.DropIndex(
                name: "IX_Documentos_TipoDocumentoId",
                table: "Documentos");

            migrationBuilder.DropIndex(
                name: "IX_Documentos_ValidadoPorId",
                table: "Documentos");

            migrationBuilder.DropIndex(
                name: "IX_Docentes_Email",
                table: "Docentes");

            migrationBuilder.DropIndex(
                name: "IX_Docentes_FacultadId",
                table: "Docentes");

            migrationBuilder.DropIndex(
                name: "IX_DatosTTHH_FacultadId",
                table: "DatosTTHH");

            migrationBuilder.DropColumn(
                name: "CumpleCapacitacion",
                table: "SolicitudesAscenso");

            migrationBuilder.DropColumn(
                name: "CumpleEvaluacion",
                table: "SolicitudesAscenso");

            migrationBuilder.DropColumn(
                name: "CumpleInvestigacion",
                table: "SolicitudesAscenso");

            migrationBuilder.DropColumn(
                name: "CumpleObras",
                table: "SolicitudesAscenso");

            migrationBuilder.DropColumn(
                name: "CumpleTiempo",
                table: "SolicitudesAscenso");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "SolicitudesAscenso");

            migrationBuilder.DropColumn(
                name: "ObservacionesRevisor",
                table: "SolicitudesAscenso");

            migrationBuilder.DropColumn(
                name: "EsObligatorio",
                table: "DocumentosSolicitud");

            migrationBuilder.DropColumn(
                name: "FechaAsociacion",
                table: "DocumentosSolicitud");

            migrationBuilder.DropColumn(
                name: "Observaciones",
                table: "DocumentosSolicitud");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "FechaValidacion",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "HashSHA256",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "ObservacionesValidacion",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "Validado",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "ValidadoPorId",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Docentes");

            migrationBuilder.DropColumn(
                name: "FechaBaja",
                table: "Docentes");

            migrationBuilder.DropColumn(
                name: "MotivoBaja",
                table: "Docentes");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "DatosTTHH");

            migrationBuilder.DropColumn(
                name: "Celular",
                table: "DatosTTHH");

            migrationBuilder.DropColumn(
                name: "Direccion",
                table: "DatosTTHH");

            migrationBuilder.DropColumn(
                name: "EmailPersonal",
                table: "DatosTTHH");

            migrationBuilder.DropColumn(
                name: "EstadoCivil",
                table: "DatosTTHH");

            migrationBuilder.DropColumn(
                name: "FacultadId",
                table: "DatosTTHH");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "DatosTTHH");

            migrationBuilder.DropColumn(
                name: "FechaIngreso",
                table: "DatosTTHH");

            migrationBuilder.DropColumn(
                name: "FechaNacimiento",
                table: "DatosTTHH");

            migrationBuilder.DropColumn(
                name: "TelefonoConvencional",
                table: "DatosTTHH");

            migrationBuilder.RenameColumn(
                name: "EstadoSolicitudId",
                table: "SolicitudesAscenso",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "TipoDocumentoId",
                table: "Documentos",
                newName: "Tipo");

            migrationBuilder.RenameColumn(
                name: "FacultadId",
                table: "Docentes",
                newName: "TiempoInvestigacion");

            migrationBuilder.AlterColumn<decimal>(
                name: "PuntajeEvaluacion",
                table: "SolicitudesAscenso",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<string>(
                name: "MotivoRechazo",
                table: "SolicitudesAscenso",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Documentos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Documentos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "Documentos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "TelefonoContacto",
                table: "Docentes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Docentes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Nombres",
                table: "Docentes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "NombreUsuario",
                table: "Docentes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Docentes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Cedula",
                table: "Docentes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Apellidos",
                table: "Docentes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Facultad",
                table: "Docentes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "HorasCapacitacion",
                table: "Docentes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumeroObras",
                table: "Docentes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PuntajeEvaluacion",
                table: "Docentes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TiempoEnRolActual",
                table: "Docentes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Nombres",
                table: "DatosTTHH",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Cedula",
                table: "DatosTTHH",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Apellidos",
                table: "DatosTTHH",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Facultad",
                table: "DatosTTHH",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesAscenso_Docentes_DocenteId",
                table: "SolicitudesAscenso",
                column: "DocenteId",
                principalTable: "Docentes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
