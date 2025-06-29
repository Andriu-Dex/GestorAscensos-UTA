using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGA.Infrastructure.Migrations.DIRINV
{
    /// <inheritdoc />
    public partial class UpdateDIRINVSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Editorial", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "TipoObra" },
                values: new object[] { null, new DateTime(2020, 9, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(2853), "ISSN-5589-8221", "Scopus", "Ponencia" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra" },
                values: new object[] { new DateTime(2021, 11, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3489), "ISSN-9021-4107", "Latindex", "Revista Científica 2", "Artículo" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "TipoObra" },
                values: new object[] { new DateTime(2022, 10, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3498), "ISSN-2358-4883", "Latindex", "Ponencia" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Editorial", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion" },
                values: new object[] { "Editorial 3", false, new DateTime(2020, 7, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3503), "ISSN-3457-2375", "Latindex" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Editorial", "FechaPublicacion", "ISBN_ISSN", "TipoObra" },
                values: new object[] { null, new DateTime(2023, 11, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3509), "ISSN-2412-3998", "Capítulo" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Cedula", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000004", true, new DateTime(2023, 4, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3516), "ISSN-2186-1253", "Latindex", null, "Ponencia", "Obra Académica 1 del Docente 4" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Cedula", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000004", false, new DateTime(2022, 1, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3519), "ISSN-9390-9454", "Latindex", "Revista Científica 1", "Artículo", "Obra Académica 2 del Docente 4" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Cedula", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "TipoObra", "Titulo" },
                values: new object[] { "1800000004", new DateTime(2023, 1, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3532), "ISSN-8115-1156", "Scopus", "Ponencia", "Obra Académica 3 del Docente 4" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Cedula", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Titulo" },
                values: new object[] { "1800000005", new DateTime(2021, 8, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3535), "ISSN-9970-7732", "Latindex", "Obra Académica 1 del Docente 5" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Cedula", "Editorial", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "TipoObra", "Titulo" },
                values: new object[] { "1800000005", "Editorial 1", new DateTime(2024, 11, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3539), "ISSN-4832-6400", "Scopus", "Libro", "Obra Académica 2 del Docente 5" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Cedula", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000005", false, new DateTime(2023, 11, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3543), "ISSN-1339-8438", "Latindex", null, "Capítulo", "Obra Académica 3 del Docente 5" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Cedula", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000006", false, new DateTime(2023, 6, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3546), "ISSN-3979-3213", "Scopus", null, "Ponencia", "Obra Académica 1 del Docente 6" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Editorial", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "TipoObra", "Titulo" },
                values: new object[] { "Editorial 2", new DateTime(2021, 2, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3549), "ISSN-1803-8686", "Scopus", "Libro", "Obra Académica 2 del Docente 6" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "FechaPublicacion", "ISBN_ISSN", "Titulo" },
                values: new object[] { new DateTime(2023, 5, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3552), "ISSN-5600-2632", "Obra Académica 3 del Docente 6" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Cedula", "FechaPublicacion", "ISBN_ISSN", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000006", new DateTime(2023, 8, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3555), "ISSN-2086-9640", "Revista Científica 1", "Artículo", "Obra Académica 4 del Docente 6" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Cedula", "Editorial", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000007", null, false, new DateTime(2023, 8, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3558), "ISSN-9293-6727", "Revista Científica 3", "Artículo", "Obra Académica 1 del Docente 7" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Cedula", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000007", new DateTime(2024, 11, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3561), "ISSN-5847-5243", "Latindex", "Revista Científica 2", "Artículo", "Obra Académica 2 del Docente 7" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Cedula", "Editorial", "FechaPublicacion", "ISBN_ISSN", "TipoObra", "Titulo" },
                values: new object[] { "1800000007", "Editorial 3", new DateTime(2021, 3, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3566), "ISSN-1257-5297", "Libro", "Obra Académica 3 del Docente 7" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Cedula", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "TipoObra", "Titulo" },
                values: new object[] { "1800000008", true, new DateTime(2023, 4, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3569), "ISSN-5595-6972", "Ponencia", "Obra Académica 1 del Docente 8" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Cedula", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000008", true, new DateTime(2023, 10, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3572), "ISSN-8412-1726", null, "Ponencia", "Obra Académica 2 del Docente 8" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Titulo" },
                values: new object[] { true, new DateTime(2024, 1, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3575), "ISSN-1509-7046", "Scopus", "Obra Académica 1 del Docente 9" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "FechaPublicacion", "ISBN_ISSN", "Titulo" },
                values: new object[] { new DateTime(2020, 12, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3577), "ISSN-5987-8463", "Obra Académica 2 del Docente 9" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Cedula", "Editorial", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "TipoObra", "Titulo" },
                values: new object[] { "1800000009", null, false, new DateTime(2022, 4, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3580), "ISSN-9677-6134", "Ponencia", "Obra Académica 3 del Docente 9" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Cedula", "Editorial", "FechaPublicacion", "ISBN_ISSN", "TipoObra", "Titulo" },
                values: new object[] { "1800000009", "Editorial 3", new DateTime(2024, 4, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3583), "ISSN-6972-2886", "Libro", "Obra Académica 4 del Docente 9" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Cedula", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000009", new DateTime(2022, 1, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3586), "ISSN-8307-5029", "Latindex", null, "Ponencia", "Obra Académica 5 del Docente 9" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "Cedula", "Editorial", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "TipoObra", "Titulo" },
                values: new object[] { "1800000010", "Editorial 1", true, new DateTime(2021, 11, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3591), "ISSN-7480-5291", "Scopus", "Libro", "Obra Académica 1 del Docente 10" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "Cedula", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "TipoObra", "Titulo" },
                values: new object[] { "1800000010", new DateTime(2024, 12, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3594), "ISSN-5851-1266", "Scopus", "Ponencia", "Obra Académica 2 del Docente 10" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "Cedula", "Editorial", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000010", "Editorial 3", false, new DateTime(2020, 8, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3597), "ISSN-8044-9186", null, "Libro", "Obra Académica 3 del Docente 10" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "Cedula", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000010", new DateTime(2022, 8, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3600), "ISSN-5369-8706", "Latindex", "Revista Científica 2", "Artículo", "Obra Académica 4 del Docente 10" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "Cedula", "FechaPublicacion", "ISBN_ISSN", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000011", new DateTime(2021, 10, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3604), "ISSN-1203-8287", null, "Capítulo", "Obra Académica 1 del Docente 11" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "Cedula", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Titulo" },
                values: new object[] { "1800000011", true, new DateTime(2023, 6, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3608), "ISSN-4434-7020", "Scopus", "Obra Académica 2 del Docente 11" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "Cedula", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000011", new DateTime(2023, 2, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3616), "ISSN-7914-7479", "Scopus", null, "Capítulo", "Obra Académica 3 del Docente 11" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "Cedula", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000011", true, new DateTime(2024, 4, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3619), "ISSN-5471-1029", "Scopus", null, "Ponencia", "Obra Académica 4 del Docente 11" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "Cedula", "Editorial", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000011", null, false, new DateTime(2020, 7, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3624), "ISSN-1709-9341", "Latindex", "Revista Científica 4", "Artículo", "Obra Académica 5 del Docente 11" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "Cedula", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000013", true, new DateTime(2024, 9, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3628), "ISSN-7715-4220", "Latindex", null, "Ponencia", "Obra Académica 1 del Docente 13" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "Cedula", "Editorial", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000013", "Editorial 2", new DateTime(2021, 4, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3631), "ISSN-5424-8477", "Scopus", null, "Libro", "Obra Académica 2 del Docente 13" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "Revista", "TipoObra" },
                values: new object[] { false, new DateTime(2021, 6, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3634), "ISSN-1725-7812", "Revista Científica 1", "Artículo" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "Editorial", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion" },
                values: new object[] { "Editorial 3", new DateTime(2024, 1, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3637), "ISSN-1693-8195", "Latindex" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista" },
                values: new object[] { new DateTime(2020, 9, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3641), "ISSN-1584-1058", "Scopus", "Revista Científica 3" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "Cedula", "Editorial", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "TipoObra", "Titulo" },
                values: new object[] { "1800000015", "Editorial 4", new DateTime(2023, 2, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3644), "ISSN-3817-6588", "Latindex", "Libro", "Obra Académica 4 del Docente 15" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "Editorial", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { null, new DateTime(2021, 8, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3647), "ISSN-8549-4594", "Latindex", "Revista Científica 3", "Artículo", "Obra Académica 1 del Docente 16" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "Editorial", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "TipoObra", "Titulo" },
                values: new object[] { "Editorial 2", new DateTime(2023, 9, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3650), "ISSN-6226-2875", "Scopus", "Libro", "Obra Académica 2 del Docente 16" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "FechaPublicacion", "ISBN_ISSN", "Revista", "TipoObra", "Titulo" },
                values: new object[] { new DateTime(2021, 6, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3654), "ISSN-7657-1501", "Revista Científica 2", "Artículo", "Obra Académica 3 del Docente 16" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "Cedula", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "TipoObra", "Titulo" },
                values: new object[] { "1800000017", new DateTime(2023, 12, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3657), "ISSN-3940-8346", "Latindex", "Ponencia", "Obra Académica 1 del Docente 17" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "FechaPublicacion", "ISBN_ISSN", "TipoObra", "Titulo" },
                values: new object[] { new DateTime(2022, 1, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3659), "ISSN-4481-5668", "Capítulo", "Obra Académica 2 del Docente 17" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "Editorial", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { null, new DateTime(2023, 5, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3662), "ISSN-2041-3230", "Scopus", "Revista Científica 1", "Artículo", "Obra Académica 3 del Docente 17" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "FechaPublicacion", "ISBN_ISSN" },
                values: new object[] { new DateTime(2023, 9, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3666), "ISSN-7594-9629" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "Cedula", "Editorial", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000018", "Editorial 1", new DateTime(2022, 4, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3668), "ISSN-3225-7627", "Latindex", null, "Libro", "Obra Académica 2 del Docente 18" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "Cedula", "Editorial", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Titulo" },
                values: new object[] { "1800000018", "Editorial 2", false, new DateTime(2020, 7, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3672), "ISSN-1725-2284", "Latindex", "Obra Académica 3 del Docente 18" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "Cedula", "Editorial", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "TipoObra", "Titulo" },
                values: new object[] { "1800000018", "Editorial 1", false, new DateTime(2022, 2, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3675), "ISSN-5252-7949", "Latindex", "Libro", "Obra Académica 4 del Docente 18" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "Cedula", "FechaPublicacion", "ISBN_ISSN", "Titulo" },
                values: new object[] { "1800000018", new DateTime(2022, 9, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3678), "ISSN-3198-1237", "Obra Académica 5 del Docente 18" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "Editorial", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "Editorial 1", new DateTime(2020, 7, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3680), "ISSN-7220-1526", "Scopus", null, "Libro", "Obra Académica 1 del Docente 19" });

            migrationBuilder.InsertData(
                table: "ObrasAcademicas",
                columns: new[] { "Id", "Autores", "Cedula", "DOI", "Descripcion", "Editorial", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[,]
                {
                    { 53, null, "1800000019", null, null, null, false, new DateTime(2023, 8, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3684), "ISSN-7923-9684", "Latindex", null, "Capítulo", "Obra Académica 2 del Docente 19" },
                    { 54, null, "1800000019", null, null, null, true, new DateTime(2024, 2, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3690), "ISSN-7819-9778", "Latindex", "Revista Científica 4", "Artículo", "Obra Académica 3 del Docente 19" },
                    { 55, null, "1800000020", null, null, "Editorial 3", true, new DateTime(2024, 12, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3695), "ISSN-9756-3397", "Scopus", null, "Libro", "Obra Académica 1 del Docente 20" },
                    { 56, null, "1800000020", null, null, "Editorial 3", false, new DateTime(2022, 1, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3698), "ISSN-1992-3299", "Latindex", null, "Libro", "Obra Académica 2 del Docente 20" },
                    { 57, null, "1800000020", null, null, null, false, new DateTime(2022, 9, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3701), "ISSN-3873-8526", "Scopus", "Revista Científica 2", "Artículo", "Obra Académica 3 del Docente 20" },
                    { 58, null, "1800000020", null, null, null, true, new DateTime(2023, 9, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3705), "ISSN-6647-3575", "Latindex", "Revista Científica 3", "Artículo", "Obra Académica 4 del Docente 20" },
                    { 59, null, "1800000020", null, null, null, false, new DateTime(2021, 3, 27, 19, 40, 6, 650, DateTimeKind.Utc).AddTicks(3707), "ISSN-5075-2943", "Latindex", null, "Ponencia", "Obra Académica 5 del Docente 20" }
                });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000002", new DateTime(2023, 12, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5661), new DateTime(2022, 10, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5661), 6, 5, "Investigador" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId" },
                values: new object[] { new DateTime(2024, 10, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5657), new DateTime(2023, 7, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5657), 9, 4 });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000003", new DateTime(2025, 3, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5760), new DateTime(2023, 4, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5760), 11, 6, "Colaborador" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "ProyectoId" },
                values: new object[] { "1800000003", new DateTime(2024, 6, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5769), new DateTime(2022, 7, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5769), 7 });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000004", new DateTime(2023, 10, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5772), new DateTime(2021, 10, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5772), 6, 8, "Director" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId" },
                values: new object[] { "1800000004", new DateTime(2023, 12, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5661), new DateTime(2022, 10, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5661), 17, 5 });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000005", new DateTime(2024, 10, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5657), new DateTime(2023, 7, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5657), 18, 4, "Director" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000005", new DateTime(2023, 10, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5653), new DateTime(2021, 7, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5653), 19, 3, "Director" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "ProyectoId" },
                values: new object[] { "1800000006", new DateTime(2023, 12, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5661), new DateTime(2022, 10, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5661), 5 });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000007", new DateTime(2023, 12, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5661), new DateTime(2022, 10, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5661), 5, "Investigador" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000007", new DateTime(2024, 10, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5657), new DateTime(2023, 7, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5657), 11, 4, "Colaborador" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000009", new DateTime(2023, 10, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5772), new DateTime(2021, 10, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5772), 6, 8, "Director" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000009", new DateTime(2025, 11, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5778), new DateTime(2024, 2, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5778), 17, 10, "Colaborador" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId" },
                values: new object[] { "1800000010", new DateTime(2025, 10, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(3688), new DateTime(2023, 8, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(3688), 19, 1 });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000011", new DateTime(2023, 3, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5645), new DateTime(2022, 1, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5645), 18, 2, "Colaborador" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000011", new DateTime(2024, 10, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5657), new DateTime(2023, 7, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5657), 6, 4, "Director" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000013", new DateTime(2025, 8, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5774), new DateTime(2022, 11, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5774), 17, 9, "Investigador" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000013", new DateTime(2024, 6, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5769), new DateTime(2022, 7, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5769), 9, 7, "Investigador" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000014", new DateTime(2025, 8, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5774), new DateTime(2022, 11, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5774), 6, 9, "Colaborador" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId" },
                values: new object[] { "1800000015", new DateTime(2023, 12, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5661), new DateTime(2022, 10, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5661), 16, 5 });

            migrationBuilder.InsertData(
                table: "ParticipacionesProyecto",
                columns: new[] { "Id", "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[,]
                {
                    { 21, "1800000016", new DateTime(2023, 10, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5653), new DateTime(2021, 7, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5653), 13, 3, "Investigador" },
                    { 22, "1800000017", new DateTime(2023, 10, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5772), new DateTime(2021, 10, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5772), 5, 8, "Colaborador" },
                    { 23, "1800000018", new DateTime(2023, 12, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5661), new DateTime(2022, 10, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5661), 12, 5, "Colaborador" },
                    { 24, "1800000019", new DateTime(2024, 6, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5769), new DateTime(2022, 7, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5769), 12, 7, "Colaborador" },
                    { 25, "1800000020", new DateTime(2025, 3, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5760), new DateTime(2023, 4, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5760), 9, 6, "Director" },
                    { 26, "1800000020", new DateTime(2025, 8, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5774), new DateTime(2022, 11, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5774), 5, 9, "Investigador" }
                });

            migrationBuilder.UpdateData(
                table: "ProyectosInvestigacion",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Estado", "FechaFin", "FechaInicio", "PresupuestoTotal" },
                values: new object[] { "Suspendido", new DateTime(2025, 10, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(3688), new DateTime(2023, 8, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(3688), 10115m });

            migrationBuilder.UpdateData(
                table: "ProyectosInvestigacion",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Estado", "FechaFin", "FechaInicio", "FuenteFinanciamiento", "PresupuestoTotal" },
                values: new object[] { "En curso", new DateTime(2023, 3, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5645), new DateTime(2022, 1, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5645), "Universidad", 17655m });

            migrationBuilder.UpdateData(
                table: "ProyectosInvestigacion",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Estado", "FechaFin", "FechaInicio", "FuenteFinanciamiento", "PresupuestoTotal" },
                values: new object[] { "En curso", new DateTime(2023, 10, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5653), new DateTime(2021, 7, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5653), "Externo", 42815m });

            migrationBuilder.UpdateData(
                table: "ProyectosInvestigacion",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "FechaFin", "FechaInicio", "FuenteFinanciamiento", "PresupuestoTotal" },
                values: new object[] { new DateTime(2024, 10, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5657), new DateTime(2023, 7, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5657), "Universidad", 47794m });

            migrationBuilder.UpdateData(
                table: "ProyectosInvestigacion",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "FechaFin", "FechaInicio", "FuenteFinanciamiento", "PresupuestoTotal" },
                values: new object[] { new DateTime(2023, 12, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5661), new DateTime(2022, 10, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5661), "Externo", 20696m });

            migrationBuilder.UpdateData(
                table: "ProyectosInvestigacion",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Estado", "FechaFin", "FechaInicio", "FuenteFinanciamiento", "PresupuestoTotal" },
                values: new object[] { "Finalizado", new DateTime(2025, 3, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5760), new DateTime(2023, 4, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5760), "Universidad", 30505m });

            migrationBuilder.UpdateData(
                table: "ProyectosInvestigacion",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Estado", "FechaFin", "FechaInicio", "FuenteFinanciamiento", "PresupuestoTotal" },
                values: new object[] { "Suspendido", new DateTime(2024, 6, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5769), new DateTime(2022, 7, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5769), "Externo", 44733m });

            migrationBuilder.UpdateData(
                table: "ProyectosInvestigacion",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Estado", "FechaFin", "FechaInicio", "FuenteFinanciamiento", "PresupuestoTotal" },
                values: new object[] { "Suspendido", new DateTime(2023, 10, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5772), new DateTime(2021, 10, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5772), "Universidad", 20629m });

            migrationBuilder.UpdateData(
                table: "ProyectosInvestigacion",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Estado", "FechaFin", "FechaInicio", "PresupuestoTotal" },
                values: new object[] { "Finalizado", new DateTime(2025, 8, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5774), new DateTime(2022, 11, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5774), 24496m });

            migrationBuilder.UpdateData(
                table: "ProyectosInvestigacion",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "FechaFin", "FechaInicio", "PresupuestoTotal" },
                values: new object[] { new DateTime(2025, 11, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5778), new DateTime(2024, 2, 27, 19, 40, 6, 649, DateTimeKind.Utc).AddTicks(5778), 11584m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Editorial", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "TipoObra" },
                values: new object[] { "Editorial 4", new DateTime(2022, 12, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(7707), "ISSN-5800-6989", "Latindex", "Libro" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra" },
                values: new object[] { new DateTime(2021, 7, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8432), "ISSN-6335-6781", "Scopus", null, "Capítulo" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "TipoObra" },
                values: new object[] { new DateTime(2024, 10, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8442), "ISSN-2533-7872", "Scopus", "Capítulo" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Editorial", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion" },
                values: new object[] { "Editorial 1", true, new DateTime(2024, 5, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8449), "ISSN-8138-6098", "Scopus" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Editorial", "FechaPublicacion", "ISBN_ISSN", "TipoObra" },
                values: new object[] { "Editorial 4", new DateTime(2024, 7, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8454), "ISSN-7319-8302", "Libro" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Cedula", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000003", false, new DateTime(2021, 8, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8461), "ISSN-3468-8345", "Scopus", "Revista Científica 1", "Artículo", "Obra Académica 2 del Docente 3" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Cedula", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000003", true, new DateTime(2023, 3, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8466), "ISSN-3910-3074", "Scopus", null, "Ponencia", "Obra Académica 3 del Docente 3" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Cedula", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "TipoObra", "Titulo" },
                values: new object[] { "1800000003", new DateTime(2024, 11, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8481), "ISSN-9689-5881", "Latindex", "Capítulo", "Obra Académica 4 del Docente 3" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Cedula", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Titulo" },
                values: new object[] { "1800000003", new DateTime(2022, 7, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8485), "ISSN-3285-4103", "Scopus", "Obra Académica 5 del Docente 3" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Cedula", "Editorial", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "TipoObra", "Titulo" },
                values: new object[] { "1800000004", null, new DateTime(2023, 3, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8489), "ISSN-8757-4752", "Latindex", "Ponencia", "Obra Académica 1 del Docente 4" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Cedula", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000004", true, new DateTime(2020, 11, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8493), "ISSN-9305-7254", "Scopus", "Revista Científica 2", "Artículo", "Obra Académica 2 del Docente 4" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Cedula", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000004", true, new DateTime(2024, 3, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8497), "ISSN-9632-2053", "Latindex", "Revista Científica 4", "Artículo", "Obra Académica 3 del Docente 4" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Editorial", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "TipoObra", "Titulo" },
                values: new object[] { null, new DateTime(2023, 1, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8501), "ISSN-8769-9632", "Latindex", "Capítulo", "Obra Académica 1 del Docente 6" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "FechaPublicacion", "ISBN_ISSN", "Titulo" },
                values: new object[] { new DateTime(2021, 11, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8504), "ISSN-7132-7005", "Obra Académica 2 del Docente 6" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Cedula", "FechaPublicacion", "ISBN_ISSN", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000008", new DateTime(2021, 11, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8508), "ISSN-2660-2862", null, "Ponencia", "Obra Académica 1 del Docente 8" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Cedula", "Editorial", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000008", "Editorial 4", true, new DateTime(2022, 9, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8510), "ISSN-1023-7832", null, "Libro", "Obra Académica 2 del Docente 8" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Cedula", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000008", new DateTime(2020, 9, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8514), "ISSN-8827-3697", "Scopus", null, "Capítulo", "Obra Académica 3 del Docente 8" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Cedula", "Editorial", "FechaPublicacion", "ISBN_ISSN", "TipoObra", "Titulo" },
                values: new object[] { "1800000009", null, new DateTime(2021, 2, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8519), "ISSN-8593-3760", "Ponencia", "Obra Académica 1 del Docente 9" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Cedula", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "TipoObra", "Titulo" },
                values: new object[] { "1800000009", false, new DateTime(2021, 12, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8522), "ISSN-8800-8666", "Capítulo", "Obra Académica 2 del Docente 9" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Cedula", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000009", false, new DateTime(2023, 6, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8525), "ISSN-7477-9925", "Revista Científica 4", "Artículo", "Obra Académica 3 del Docente 9" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Titulo" },
                values: new object[] { false, new DateTime(2022, 3, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8528), "ISSN-8134-4240", "Latindex", "Obra Académica 4 del Docente 9" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "FechaPublicacion", "ISBN_ISSN", "Titulo" },
                values: new object[] { new DateTime(2024, 1, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8531), "ISSN-2143-1538", "Obra Académica 5 del Docente 9" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Cedula", "Editorial", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "TipoObra", "Titulo" },
                values: new object[] { "1800000011", "Editorial 2", true, new DateTime(2022, 7, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8536), "ISSN-5900-5784", "Libro", "Obra Académica 1 del Docente 11" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Cedula", "Editorial", "FechaPublicacion", "ISBN_ISSN", "TipoObra", "Titulo" },
                values: new object[] { "1800000011", null, new DateTime(2023, 10, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8539), "ISSN-1252-9250", "Ponencia", "Obra Académica 2 del Docente 11" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Cedula", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000011", new DateTime(2022, 10, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8542), "ISSN-6998-1524", "Scopus", "Revista Científica 2", "Artículo", "Obra Académica 3 del Docente 11" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "Cedula", "Editorial", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "TipoObra", "Titulo" },
                values: new object[] { "1800000011", null, false, new DateTime(2020, 11, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8546), "ISSN-7495-6143", "Latindex", "Ponencia", "Obra Académica 4 del Docente 11" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "Cedula", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "TipoObra", "Titulo" },
                values: new object[] { "1800000011", new DateTime(2021, 6, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8550), "ISSN-9566-5298", "Latindex", "Capítulo", "Obra Académica 5 del Docente 11" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "Cedula", "Editorial", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000012", null, true, new DateTime(2021, 7, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8554), "ISSN-7947-7804", "Revista Científica 1", "Artículo", "Obra Académica 1 del Docente 12" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "Cedula", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000012", new DateTime(2021, 9, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8557), "ISSN-8403-2197", "Scopus", null, "Ponencia", "Obra Académica 2 del Docente 12" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "Cedula", "FechaPublicacion", "ISBN_ISSN", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000012", new DateTime(2021, 11, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8560), "ISSN-5291-2736", "Revista Científica 2", "Artículo", "Obra Académica 3 del Docente 12" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "Cedula", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Titulo" },
                values: new object[] { "1800000012", false, new DateTime(2021, 6, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8563), "ISSN-1877-9085", "Latindex", "Obra Académica 4 del Docente 12" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "Cedula", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000014", new DateTime(2023, 2, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8576), "ISSN-6968-9412", "Latindex", "Revista Científica 4", "Artículo", "Obra Académica 1 del Docente 14" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "Cedula", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000014", false, new DateTime(2021, 11, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8580), "ISSN-6702-1005", "Latindex", "Revista Científica 2", "Artículo", "Obra Académica 2 del Docente 14" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "Cedula", "Editorial", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000014", "Editorial 1", true, new DateTime(2023, 12, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8585), "ISSN-2006-4103", "Scopus", null, "Libro", "Obra Académica 3 del Docente 14" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "Cedula", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000014", false, new DateTime(2022, 6, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8588), "ISSN-2199-5162", "Scopus", "Revista Científica 2", "Artículo", "Obra Académica 4 del Docente 14" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "Cedula", "Editorial", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000014", null, new DateTime(2022, 11, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8591), "ISSN-3632-6404", "Latindex", "Revista Científica 1", "Artículo", "Obra Académica 5 del Docente 14" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "Revista", "TipoObra" },
                values: new object[] { true, new DateTime(2021, 11, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8595), "ISSN-5579-4876", null, "Capítulo" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "Editorial", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion" },
                values: new object[] { "Editorial 4", new DateTime(2020, 11, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8598), "ISSN-4796-9682", "Scopus" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista" },
                values: new object[] { new DateTime(2023, 10, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8602), "ISSN-2630-9550", "Latindex", "Revista Científica 4" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "Cedula", "Editorial", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "TipoObra", "Titulo" },
                values: new object[] { "1800000016", null, new DateTime(2023, 8, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8605), "ISSN-8017-1527", "Scopus", "Capítulo", "Obra Académica 1 del Docente 16" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "Editorial", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "Editorial 2", new DateTime(2021, 9, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8608), "ISSN-6627-8392", "Scopus", null, "Libro", "Obra Académica 2 del Docente 16" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "Editorial", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "TipoObra", "Titulo" },
                values: new object[] { null, new DateTime(2023, 8, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8611), "ISSN-6603-8262", "Latindex", "Ponencia", "Obra Académica 3 del Docente 16" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "FechaPublicacion", "ISBN_ISSN", "Revista", "TipoObra", "Titulo" },
                values: new object[] { new DateTime(2021, 3, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8614), "ISSN-2744-7354", null, "Capítulo", "Obra Académica 4 del Docente 16" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "Cedula", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "TipoObra", "Titulo" },
                values: new object[] { "1800000016", new DateTime(2023, 11, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8617), "ISSN-1457-8728", "Scopus", "Capítulo", "Obra Académica 5 del Docente 16" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "FechaPublicacion", "ISBN_ISSN", "TipoObra", "Titulo" },
                values: new object[] { new DateTime(2023, 9, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8620), "ISSN-8034-7226", "Ponencia", "Obra Académica 1 del Docente 17" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "Editorial", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "Editorial 1", new DateTime(2023, 2, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8623), "ISSN-5416-2445", "Latindex", null, "Libro", "Obra Académica 2 del Docente 17" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "FechaPublicacion", "ISBN_ISSN" },
                values: new object[] { new DateTime(2021, 9, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8627), "ISSN-4488-9342" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "Cedula", "Editorial", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { "1800000019", null, new DateTime(2022, 7, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8630), "ISSN-1246-5637", "Scopus", "Revista Científica 3", "Artículo", "Obra Académica 1 del Docente 19" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "Cedula", "Editorial", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Titulo" },
                values: new object[] { "1800000019", "Editorial 1", true, new DateTime(2020, 9, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8633), "ISSN-6703-8689", "Scopus", "Obra Académica 2 del Docente 19" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "Cedula", "Editorial", "EsIndexada", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "TipoObra", "Titulo" },
                values: new object[] { "1800000019", null, true, new DateTime(2022, 12, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8637), "ISSN-6558-9157", "Scopus", "Capítulo", "Obra Académica 3 del Docente 19" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "Cedula", "FechaPublicacion", "ISBN_ISSN", "Titulo" },
                values: new object[] { "1800000019", new DateTime(2023, 6, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8640), "ISSN-2485-5785", "Obra Académica 4 del Docente 19" });

            migrationBuilder.UpdateData(
                table: "ObrasAcademicas",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "Editorial", "FechaPublicacion", "ISBN_ISSN", "IndiceIndexacion", "Revista", "TipoObra", "Titulo" },
                values: new object[] { null, new DateTime(2022, 10, 27, 19, 39, 26, 757, DateTimeKind.Utc).AddTicks(8643), "ISSN-4791-1005", "Latindex", "Revista Científica 3", "Artículo", "Obra Académica 5 del Docente 19" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000001", new DateTime(2025, 9, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9817), new DateTime(2023, 10, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9817), 11, 6, "Colaborador" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId" },
                values: new object[] { new DateTime(2026, 1, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(7337), new DateTime(2024, 6, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(7337), 12, 1 });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000004", new DateTime(2024, 7, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9810), new DateTime(2022, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9810), 16, 5, "Investigador" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "ProyectoId" },
                values: new object[] { "1800000004", new DateTime(2025, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9804), new DateTime(2023, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9804), 3 });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000005", new DateTime(2026, 1, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(7337), new DateTime(2024, 6, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(7337), 14, 1, "Colaborador" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId" },
                values: new object[] { "1800000005", new DateTime(2025, 2, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9822), new DateTime(2023, 12, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9822), 9, 8 });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000007", new DateTime(2025, 2, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9822), new DateTime(2023, 12, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9822), 17, 8, "Investigador" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000009", new DateTime(2026, 1, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(7337), new DateTime(2024, 6, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(7337), 15, 1, "Colaborador" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "ProyectoId" },
                values: new object[] { "1800000012", new DateTime(2025, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9804), new DateTime(2023, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9804), 3 });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000012", new DateTime(2025, 2, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9828), new DateTime(2022, 6, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9828), 10, "Colaborador" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000013", new DateTime(2024, 11, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9825), new DateTime(2022, 8, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9825), 16, 9, "Director" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000013", new DateTime(2024, 5, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9789), new DateTime(2022, 11, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9789), 15, 2, "Investigador" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000014", new DateTime(2025, 2, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9822), new DateTime(2023, 12, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9822), 12, 8, "Investigador" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId" },
                values: new object[] { "1800000015", new DateTime(2024, 2, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9819), new DateTime(2022, 4, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9819), 15, 7 });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000015", new DateTime(2025, 2, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9807), new DateTime(2023, 8, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9807), 5, 4, "Investigador" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000016", new DateTime(2025, 9, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9817), new DateTime(2023, 10, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9817), 14, 6, "Colaborador" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000017", new DateTime(2025, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9804), new DateTime(2023, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9804), 18, 3, "Colaborador" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000017", new DateTime(2024, 11, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9825), new DateTime(2022, 8, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9825), 8, 9, "Colaborador" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId", "RolEnProyecto" },
                values: new object[] { "1800000018", new DateTime(2024, 7, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9810), new DateTime(2022, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9810), 11, 5, "Investigador" });

            migrationBuilder.UpdateData(
                table: "ParticipacionesProyecto",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Cedula", "FechaFin", "FechaInicio", "HorasSemanales", "ProyectoId" },
                values: new object[] { "1800000020", new DateTime(2025, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9804), new DateTime(2023, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9804), 7, 3 });

            migrationBuilder.UpdateData(
                table: "ProyectosInvestigacion",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Estado", "FechaFin", "FechaInicio", "PresupuestoTotal" },
                values: new object[] { "Finalizado", new DateTime(2026, 1, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(7337), new DateTime(2024, 6, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(7337), 38029m });

            migrationBuilder.UpdateData(
                table: "ProyectosInvestigacion",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Estado", "FechaFin", "FechaInicio", "FuenteFinanciamiento", "PresupuestoTotal" },
                values: new object[] { "Finalizado", new DateTime(2024, 5, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9789), new DateTime(2022, 11, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9789), "Externo", 24335m });

            migrationBuilder.UpdateData(
                table: "ProyectosInvestigacion",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Estado", "FechaFin", "FechaInicio", "FuenteFinanciamiento", "PresupuestoTotal" },
                values: new object[] { "Finalizado", new DateTime(2025, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9804), new DateTime(2023, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9804), "Universidad", 45790m });

            migrationBuilder.UpdateData(
                table: "ProyectosInvestigacion",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "FechaFin", "FechaInicio", "FuenteFinanciamiento", "PresupuestoTotal" },
                values: new object[] { new DateTime(2025, 2, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9807), new DateTime(2023, 8, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9807), "Externo", 18103m });

            migrationBuilder.UpdateData(
                table: "ProyectosInvestigacion",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "FechaFin", "FechaInicio", "FuenteFinanciamiento", "PresupuestoTotal" },
                values: new object[] { new DateTime(2024, 7, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9810), new DateTime(2022, 3, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9810), "Universidad", 16816m });

            migrationBuilder.UpdateData(
                table: "ProyectosInvestigacion",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Estado", "FechaFin", "FechaInicio", "FuenteFinanciamiento", "PresupuestoTotal" },
                values: new object[] { "En curso", new DateTime(2025, 9, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9817), new DateTime(2023, 10, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9817), "Externo", 22207m });

            migrationBuilder.UpdateData(
                table: "ProyectosInvestigacion",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Estado", "FechaFin", "FechaInicio", "FuenteFinanciamiento", "PresupuestoTotal" },
                values: new object[] { "Finalizado", new DateTime(2024, 2, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9819), new DateTime(2022, 4, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9819), "Universidad", 47090m });

            migrationBuilder.UpdateData(
                table: "ProyectosInvestigacion",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Estado", "FechaFin", "FechaInicio", "FuenteFinanciamiento", "PresupuestoTotal" },
                values: new object[] { "Finalizado", new DateTime(2025, 2, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9822), new DateTime(2023, 12, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9822), "Externo", 47270m });

            migrationBuilder.UpdateData(
                table: "ProyectosInvestigacion",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Estado", "FechaFin", "FechaInicio", "PresupuestoTotal" },
                values: new object[] { "Suspendido", new DateTime(2024, 11, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9825), new DateTime(2022, 8, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9825), 48793m });

            migrationBuilder.UpdateData(
                table: "ProyectosInvestigacion",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "FechaFin", "FechaInicio", "PresupuestoTotal" },
                values: new object[] { new DateTime(2025, 2, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9828), new DateTime(2022, 6, 27, 19, 39, 26, 756, DateTimeKind.Utc).AddTicks(9828), 46657m });
        }
    }
}
