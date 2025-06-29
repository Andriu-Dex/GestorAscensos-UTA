using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGA.Infrastructure.Migrations.DIRINV
{
    /// <inheritdoc />
    public partial class CreateObrasAcademicasTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ObrasAcademicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TipoObra = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Editorial = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Revista = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ISBN_ISSN = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DOI = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EsIndexada = table.Column<bool>(type: "bit", nullable: false),
                    IndiceIndexacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Autores = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObrasAcademicas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProyectosInvestigacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PresupuestoTotal = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    FuenteFinanciamiento = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectosInvestigacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParticipacionesProyecto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ProyectoId = table.Column<int>(type: "int", nullable: false),
                    RolEnProyecto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HorasSemanales = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipacionesProyecto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParticipacionesProyecto_ProyectosInvestigacion_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "ProyectosInvestigacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ObrasAcademicas",
                columns: new[] { "Id", "Autores", "Cedula", "DOI", "Descripcion", "Editorial", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[,]
                {
                    { 1, null, "1800000001", null, null, "Editorial 4", false, new DateTime(2022, 12, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(7707), "ISSN-5800-6989", "Latindex", null, "Libro", "Obra Académica 1 del Docente 1" },
                    { 2, null, "1800000001", null, null, null, false, new DateTime(2021, 7, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8432), "ISSN-6335-6781", "Scopus", null, "Capítulo", "Obra Académica 2 del Docente 1" },
                    { 3, null, "1800000001", null, null, null, false, new DateTime(2024, 10, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8442), "ISSN-2533-7872", "Scopus", null, "Capítulo", "Obra Académica 3 del Docente 1" },
                    { 4, null, "1800000002", null, null, "Editorial 1", true, new DateTime(2024, 5, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8449), "ISSN-8138-6098", "Scopus", null, "Libro", "Obra Académica 1 del Docente 2" },
                    { 5, null, "1800000003", null, null, "Editorial 4", false, new DateTime(2024, 7, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8454), "ISSN-7319-8302", "Scopus", null, "Libro", "Obra Académica 1 del Docente 3" },
                    { 6, null, "1800000003", null, null, null, false, new DateTime(2021, 8, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8461), "ISSN-3468-8345", "Scopus", "Revista Científica 1", "Artículo", "Obra Académica 2 del Docente 3" },
                    { 7, null, "1800000003", null, null, null, true, new DateTime(2023, 3, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8466), "ISSN-3910-3074", "Scopus", null, "Ponencia", "Obra Académica 3 del Docente 3" },
                    { 8, null, "1800000003", null, null, null, true, new DateTime(2024, 11, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8481), "ISSN-9689-5881", "Latindex", null, "Capítulo", "Obra Académica 4 del Docente 3" },
                    { 9, null, "1800000003", null, null, null, false, new DateTime(2022, 7, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8485), "ISSN-3285-4103", "Scopus", "Revista Científica 3", "Artículo", "Obra Académica 5 del Docente 3" },
                    { 10, null, "1800000004", null, null, null, false, new DateTime(2023, 3, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8489), "ISSN-8757-4752", "Latindex", null, "Ponencia", "Obra Académica 1 del Docente 4" },
                    { 11, null, "1800000004", null, null, null, true, new DateTime(2020, 11, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8493), "ISSN-9305-7254", "Scopus", "Revista Científica 2", "Artículo", "Obra Académica 2 del Docente 4" },
                    { 12, null, "1800000004", null, null, null, true, new DateTime(2024, 3, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8497), "ISSN-9632-2053", "Latindex", "Revista Científica 4", "Artículo", "Obra Académica 3 del Docente 4" },
                    { 13, null, "1800000006", null, null, null, true, new DateTime(2023, 1, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8501), "ISSN-8769-9632", "Latindex", null, "Capítulo", "Obra Académica 1 del Docente 6" },
                    { 14, null, "1800000006", null, null, null, false, new DateTime(2021, 11, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8504), "ISSN-7132-7005", "Latindex", null, "Capítulo", "Obra Académica 2 del Docente 6" },
                    { 15, null, "1800000008", null, null, null, true, new DateTime(2021, 11, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8508), "ISSN-2660-2862", "Latindex", null, "Ponencia", "Obra Académica 1 del Docente 8" },
                    { 16, null, "1800000008", null, null, "Editorial 4", true, new DateTime(2022, 9, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8510), "ISSN-1023-7832", "Latindex", null, "Libro", "Obra Académica 2 del Docente 8" },
                    { 17, null, "1800000008", null, null, null, false, new DateTime(2020, 9, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8514), "ISSN-8827-3697", "Scopus", null, "Capítulo", "Obra Académica 3 del Docente 8" },
                    { 18, null, "1800000009", null, null, null, false, new DateTime(2021, 2, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8519), "ISSN-8593-3760", "Scopus", null, "Ponencia", "Obra Académica 1 del Docente 9" },
                    { 19, null, "1800000009", null, null, null, false, new DateTime(2021, 12, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8522), "ISSN-8800-8666", "Latindex", null, "Capítulo", "Obra Académica 2 del Docente 9" },
                    { 20, null, "1800000009", null, null, null, false, new DateTime(2023, 6, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8525), "ISSN-7477-9925", "Scopus", "Revista Científica 4", "Artículo", "Obra Académica 3 del Docente 9" },
                    { 21, null, "1800000009", null, null, null, false, new DateTime(2022, 3, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8528), "ISSN-8134-4240", "Latindex", null, "Capítulo", "Obra Académica 4 del Docente 9" },
                    { 22, null, "1800000009", null, null, null, false, new DateTime(2024, 1, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8531), "ISSN-2143-1538", "Latindex", null, "Capítulo", "Obra Académica 5 del Docente 9" },
                    { 23, null, "1800000011", null, null, "Editorial 2", true, new DateTime(2022, 7, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8536), "ISSN-5900-5784", "Scopus", null, "Libro", "Obra Académica 1 del Docente 11" },
                    { 24, null, "1800000011", null, null, null, false, new DateTime(2023, 10, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8539), "ISSN-1252-9250", "Latindex", null, "Ponencia", "Obra Académica 2 del Docente 11" },
                    { 25, null, "1800000011", null, null, null, false, new DateTime(2022, 10, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8542), "ISSN-6998-1524", "Scopus", "Revista Científica 2", "Artículo", "Obra Académica 3 del Docente 11" },
                    { 26, null, "1800000011", null, null, null, false, new DateTime(2020, 11, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8546), "ISSN-7495-6143", "Latindex", null, "Ponencia", "Obra Académica 4 del Docente 11" },
                    { 27, null, "1800000011", null, null, null, true, new DateTime(2021, 6, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8550), "ISSN-9566-5298", "Latindex", null, "Capítulo", "Obra Académica 5 del Docente 11" },
                    { 28, null, "1800000012", null, null, null, true, new DateTime(2021, 7, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8554), "ISSN-7947-7804", "Latindex", "Revista Científica 1", "Artículo", "Obra Académica 1 del Docente 12" },
                    { 29, null, "1800000012", null, null, null, false, new DateTime(2021, 9, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8557), "ISSN-8403-2197", "Scopus", null, "Ponencia", "Obra Académica 2 del Docente 12" },
                    { 30, null, "1800000012", null, null, null, true, new DateTime(2021, 11, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8560), "ISSN-5291-2736", "Latindex", "Revista Científica 2", "Artículo", "Obra Académica 3 del Docente 12" },
                    { 31, null, "1800000012", null, null, null, false, new DateTime(2021, 6, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8563), "ISSN-1877-9085", "Latindex", null, "Capítulo", "Obra Académica 4 del Docente 12" },
                    { 32, null, "1800000014", null, null, null, true, new DateTime(2023, 2, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8576), "ISSN-6968-9412", "Latindex", "Revista Científica 4", "Artículo", "Obra Académica 1 del Docente 14" },
                    { 33, null, "1800000014", null, null, null, false, new DateTime(2021, 11, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8580), "ISSN-6702-1005", "Latindex", "Revista Científica 2", "Artículo", "Obra Académica 2 del Docente 14" },
                    { 34, null, "1800000014", null, null, "Editorial 1", true, new DateTime(2023, 12, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8585), "ISSN-2006-4103", "Scopus", null, "Libro", "Obra Académica 3 del Docente 14" },
                    { 35, null, "1800000014", null, null, null, false, new DateTime(2022, 6, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8588), "ISSN-2199-5162", "Scopus", "Revista Científica 2", "Artículo", "Obra Académica 4 del Docente 14" },
                    { 36, null, "1800000014", null, null, null, false, new DateTime(2022, 11, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8591), "ISSN-3632-6404", "Latindex", "Revista Científica 1", "Artículo", "Obra Académica 5 del Docente 14" },
                    { 37, null, "1800000015", null, null, null, true, new DateTime(2021, 11, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8595), "ISSN-5579-4876", "Latindex", null, "Capítulo", "Obra Académica 1 del Docente 15" },
                    { 38, null, "1800000015", null, null, "Editorial 4", false, new DateTime(2020, 11, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8598), "ISSN-4796-9682", "Scopus", null, "Libro", "Obra Académica 2 del Docente 15" },
                    { 39, null, "1800000015", null, null, null, false, new DateTime(2023, 10, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8602), "ISSN-2630-9550", "Latindex", "Revista Científica 4", "Artículo", "Obra Académica 3 del Docente 15" },
                    { 40, null, "1800000016", null, null, null, true, new DateTime(2023, 8, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8605), "ISSN-8017-1527", "Scopus", null, "Capítulo", "Obra Académica 1 del Docente 16" },
                    { 41, null, "1800000016", null, null, "Editorial 2", false, new DateTime(2021, 9, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8608), "ISSN-6627-8392", "Scopus", null, "Libro", "Obra Académica 2 del Docente 16" },
                    { 42, null, "1800000016", null, null, null, false, new DateTime(2023, 8, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8611), "ISSN-6603-8262", "Latindex", null, "Ponencia", "Obra Académica 3 del Docente 16" },
                    { 43, null, "1800000016", null, null, null, false, new DateTime(2021, 3, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8614), "ISSN-2744-7354", "Scopus", null, "Capítulo", "Obra Académica 4 del Docente 16" },
                    { 44, null, "1800000016", null, null, null, false, new DateTime(2023, 11, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8617), "ISSN-1457-8728", "Scopus", null, "Capítulo", "Obra Académica 5 del Docente 16" },
                    { 45, null, "1800000017", null, null, null, true, new DateTime(2023, 9, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8620), "ISSN-8034-7226", "Scopus", null, "Ponencia", "Obra Académica 1 del Docente 17" },
                    { 46, null, "1800000017", null, null, "Editorial 1", true, new DateTime(2023, 2, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8623), "ISSN-5416-2445", "Latindex", null, "Libro", "Obra Académica 2 del Docente 17" },
                    { 47, null, "1800000018", null, null, null, true, new DateTime(2021, 9, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8627), "ISSN-4488-9342", "Scopus", null, "Capítulo", "Obra Académica 1 del Docente 18" },
                    { 48, null, "1800000019", null, null, null, false, new DateTime(2022, 7, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8630), "ISSN-1246-5637", "Scopus", "Revista Científica 3", "Artículo", "Obra Académica 1 del Docente 19" },
                    { 49, null, "1800000019", null, null, "Editorial 1", true, new DateTime(2020, 9, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8633), "ISSN-6703-8689", "Scopus", null, "Libro", "Obra Académica 2 del Docente 19" },
                    { 50, null, "1800000019", null, null, null, true, new DateTime(2022, 12, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8637), "ISSN-6558-9157", "Scopus", null, "Capítulo", "Obra Académica 3 del Docente 19" },
                    { 51, null, "1800000019", null, null, null, true, new DateTime(2023, 6, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8640), "ISSN-2485-5785", "Scopus", null, "Capítulo", "Obra Académica 4 del Docente 19" },
                    { 52, null, "1800000019", null, null, null, false, new DateTime(2022, 10, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8643), "ISSN-4791-1005", "Latindex", "Revista Científica 3", "Artículo", "Obra Académica 5 del Docente 19" }
                });

            migrationBuilder.InsertData(
                table: "ProyectosInvestigacion",
                columns: new[] { "Id", "Descripcion", "Estado", "FechaFin", "FechaInicio", "FuenteFinanciamiento", "PresupuestoTotal", "Titulo" },
                values: new object[,]
                {
                    { 1, "Descripción del proyecto de investigación número 1", "Finalizado", new DateTime(2026, 1, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(7337), new DateTime(2024, 6, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(7337), "Externo", 38029m, "Proyecto de Investigación 1" },
                    { 2, "Descripción del proyecto de investigación número 2", "Finalizado", new DateTime(2024, 5, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9789), new DateTime(2022, 11, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9789), "Externo", 24335m, "Proyecto de Investigación 2" },
                    { 3, "Descripción del proyecto de investigación número 3", "Finalizado", new DateTime(2025, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9804), new DateTime(2023, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9804), "Universidad", 45790m, "Proyecto de Investigación 3" },
                    { 4, "Descripción del proyecto de investigación número 4", "Suspendido", new DateTime(2025, 2, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9807), new DateTime(2023, 8, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9807), "Externo", 18103m, "Proyecto de Investigación 4" },
                    { 5, "Descripción del proyecto de investigación número 5", "En curso", new DateTime(2024, 7, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9810), new DateTime(2022, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9810), "Universidad", 16816m, "Proyecto de Investigación 5" },
                    { 6, "Descripción del proyecto de investigación número 6", "En curso", new DateTime(2025, 9, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9817), new DateTime(2023, 10, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9817), "Externo", 22207m, "Proyecto de Investigación 6" },
                    { 7, "Descripción del proyecto de investigación número 7", "Finalizado", new DateTime(2024, 2, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9819), new DateTime(2022, 4, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9819), "Universidad", 47090m, "Proyecto de Investigación 7" },
                    { 8, "Descripción del proyecto de investigación número 8", "Finalizado", new DateTime(2025, 2, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9822), new DateTime(2023, 12, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9822), "Externo", 47270m, "Proyecto de Investigación 8" },
                    { 9, "Descripción del proyecto de investigación número 9", "Suspendido", new DateTime(2024, 11, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9825), new DateTime(2022, 8, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9825), "Universidad", 48793m, "Proyecto de Investigación 9" },
                    { 10, "Descripción del proyecto de investigación número 10", "En curso", new DateTime(2025, 2, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9828), new DateTime(2022, 6, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9828), "Universidad", 46657m, "Proyecto de Investigación 10" }
                });

            migrationBuilder.InsertData(
                table: "ParticipacionesProyecto",
                columns: new[] { "Id", "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[,]
                {
                    { 1, "1800000001", new DateTime(2025, 9, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9817), new DateTime(2023, 10, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9817), 11, 6, "Colaborador" },
                    { 2, "1800000002", new DateTime(2026, 1, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(7337), new DateTime(2024, 6, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(7337), 12, 1, "Investigador" },
                    { 3, "1800000004", new DateTime(2024, 7, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9810), new DateTime(2022, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9810), 16, 5, "Investigador" },
                    { 4, "1800000004", new DateTime(2025, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9804), new DateTime(2023, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9804), 13, 3, "Colaborador" },
                    { 5, "1800000005", new DateTime(2026, 1, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(7337), new DateTime(2024, 6, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(7337), 14, 1, "Colaborador" },
                    { 6, "1800000005", new DateTime(2025, 2, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9822), new DateTime(2023, 12, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9822), 9, 8, "Colaborador" },
                    { 7, "1800000007", new DateTime(2025, 2, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9822), new DateTime(2023, 12, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9822), 17, 8, "Investigador" },
                    { 8, "1800000009", new DateTime(2026, 1, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(7337), new DateTime(2024, 6, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(7337), 15, 1, "Colaborador" },
                    { 9, "1800000012", new DateTime(2025, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9804), new DateTime(2023, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9804), 15, 3, "Investigador" },
                    { 10, "1800000012", new DateTime(2025, 2, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9828), new DateTime(2022, 6, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9828), 15, 10, "Colaborador" },
                    { 11, "1800000013", new DateTime(2024, 11, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9825), new DateTime(2022, 8, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9825), 16, 9, "Director" },
                    { 12, "1800000013", new DateTime(2024, 5, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9789), new DateTime(2022, 11, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9789), 15, 2, "Investigador" },
                    { 13, "1800000014", new DateTime(2025, 2, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9822), new DateTime(2023, 12, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9822), 12, 8, "Investigador" },
                    { 14, "1800000015", new DateTime(2024, 2, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9819), new DateTime(2022, 4, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9819), 15, 7, "Colaborador" },
                    { 15, "1800000015", new DateTime(2025, 2, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9807), new DateTime(2023, 8, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9807), 5, 4, "Investigador" },
                    { 16, "1800000016", new DateTime(2025, 9, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9817), new DateTime(2023, 10, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9817), 14, 6, "Colaborador" },
                    { 17, "1800000017", new DateTime(2025, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9804), new DateTime(2023, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9804), 18, 3, "Colaborador" },
                    { 18, "1800000017", new DateTime(2024, 11, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9825), new DateTime(2022, 8, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9825), 8, 9, "Colaborador" },
                    { 19, "1800000018", new DateTime(2024, 7, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9810), new DateTime(2022, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9810), 11, 5, "Investigador" },
                    { 20, "1800000020", new DateTime(2025, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9804), new DateTime(2023, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9804), 7, 3, "Investigador" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObrasAcademicas_Cedula",
                table: "ObrasAcademicas",
                column: "Cedula");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipacionesProyecto_Cedula",
                table: "ParticipacionesProyecto",
                column: "Cedula");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipacionesProyecto_ProyectoId",
                table: "ParticipacionesProyecto",
                column: "ProyectoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObrasAcademicas");

            migrationBuilder.DropTable(
                name: "ParticipacionesProyecto");

            migrationBuilder.DropTable(
                name: "ProyectosInvestigacion");
        }
    }
}
