using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarObrasAcademicas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ObrasAcademicas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocenteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TipoObra = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Editorial = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Revista = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ISBN_ISSN = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DOI = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EsIndexada = table.Column<bool>(type: "bit", nullable: false),
                    IndiceIndexacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Autores = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    NombreArchivo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ContenidoArchivoPDF = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    TamanoArchivo = table.Column<long>(type: "bigint", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OrigenDatos = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EsVerificada = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObrasAcademicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObrasAcademicas_Docentes_DocenteId",
                        column: x => x.DocenteId,
                        principalTable: "Docentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 1, 17, 56, 42, 566, DateTimeKind.Utc).AddTicks(113), new DateTime(2023, 7, 1, 17, 56, 42, 566, DateTimeKind.Utc).AddTicks(110) });

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("8ef569a9-342c-4e85-a8e1-29b5e697f2b6"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 1, 17, 56, 42, 566, DateTimeKind.Utc).AddTicks(103), new DateTime(2020, 7, 1, 17, 56, 42, 565, DateTimeKind.Utc).AddTicks(9718) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("c24cd969-b99a-4354-b49f-0cae93b0b7ad"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 1, 17, 56, 42, 464, DateTimeKind.Utc).AddTicks(6410), "$2a$11$AqlmDI.rfpnev0gs/CNsGexO4HEEF651hE9GBBHx8b8tiPSEf2ceO", new DateTime(2020, 7, 1, 17, 56, 42, 464, DateTimeKind.Utc).AddTicks(6166) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 1, 17, 56, 42, 565, DateTimeKind.Utc).AddTicks(3669), "$2a$11$ODb9rH.WQAlyt9DUhBD7L.A5gqI2loYp6q4hy1Q8PKuREseVIKIXi", new DateTime(2025, 6, 1, 17, 56, 42, 565, DateTimeKind.Utc).AddTicks(3583) });

            migrationBuilder.CreateIndex(
                name: "IX_ObrasAcademicas_DocenteId",
                table: "ObrasAcademicas",
                column: "DocenteId");

            migrationBuilder.CreateIndex(
                name: "IX_ObrasAcademicas_DocenteId_FechaPublicacion",
                table: "ObrasAcademicas",
                columns: new[] { "DocenteId", "FechaPublicacion" });

            migrationBuilder.CreateIndex(
                name: "IX_ObrasAcademicas_Titulo_DocenteId",
                table: "ObrasAcademicas",
                columns: new[] { "Titulo", "DocenteId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObrasAcademicas");

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 6, 29, 22, 54, 47, 805, DateTimeKind.Utc).AddTicks(3211), new DateTime(2023, 6, 29, 22, 54, 47, 805, DateTimeKind.Utc).AddTicks(3209) });

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("8ef569a9-342c-4e85-a8e1-29b5e697f2b6"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 6, 29, 22, 54, 47, 805, DateTimeKind.Utc).AddTicks(3203), new DateTime(2020, 6, 29, 22, 54, 47, 805, DateTimeKind.Utc).AddTicks(2822) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("c24cd969-b99a-4354-b49f-0cae93b0b7ad"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 6, 29, 22, 54, 47, 674, DateTimeKind.Utc).AddTicks(4703), "$2a$11$BJNwOb1tr/dlva7ZvEk6y.KhZYU/y/FvEqJqIpdGfEXfK3IZ9mFwu", new DateTime(2020, 6, 29, 22, 54, 47, 674, DateTimeKind.Utc).AddTicks(4388) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 6, 29, 22, 54, 47, 804, DateTimeKind.Utc).AddTicks(5125), "$2a$11$9j6NA1/h0FNRg/HD/ZmE.e8ijZ2Jj.dvi6boc57JiZ/cpN3ot6CFu", new DateTime(2025, 5, 30, 22, 54, 47, 804, DateTimeKind.Utc).AddTicks(5039) });
        }
    }
}
