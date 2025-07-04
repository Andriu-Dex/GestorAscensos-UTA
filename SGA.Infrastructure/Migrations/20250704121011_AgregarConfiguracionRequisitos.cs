using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarConfiguracionRequisitos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracionesRequisitos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NivelActual = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NivelSolicitado = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TiempoMinimoMeses = table.Column<int>(type: "int", nullable: false),
                    ObrasMinimas = table.Column<int>(type: "int", nullable: false),
                    PuntajeEvaluacionMinimo = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    HorasCapacitacionMinimas = table.Column<int>(type: "int", nullable: false),
                    TiempoInvestigacionMinimo = table.Column<int>(type: "int", nullable: false),
                    EstaActivo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesRequisitos", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 4, 12, 10, 11, 85, DateTimeKind.Utc).AddTicks(9402), new DateTime(2023, 7, 4, 12, 10, 11, 85, DateTimeKind.Utc).AddTicks(9400) });

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("8ef569a9-342c-4e85-a8e1-29b5e697f2b6"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 4, 12, 10, 11, 85, DateTimeKind.Utc).AddTicks(9385), new DateTime(2020, 7, 4, 12, 10, 11, 85, DateTimeKind.Utc).AddTicks(9039) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("c24cd969-b99a-4354-b49f-0cae93b0b7ad"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 4, 12, 10, 10, 933, DateTimeKind.Utc).AddTicks(5199), "$2a$11$j/lMnkcIWT4wqnvTtS5EKOxm1xnWYM93VtXQ0i6sHVgZ1UfQZrhLG", new DateTime(2020, 7, 4, 12, 10, 10, 933, DateTimeKind.Utc).AddTicks(4926) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 4, 12, 10, 11, 85, DateTimeKind.Utc).AddTicks(1395), "$2a$11$lJAB6UPvL5eYMUlEq7MCJefxbNVZ0uHg5lkS9a2zCmyS6GzL0v1R6", new DateTime(2025, 6, 4, 12, 10, 11, 85, DateTimeKind.Utc).AddTicks(1283) });

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesRequisitos_EstaActivo",
                table: "ConfiguracionesRequisitos",
                column: "EstaActivo");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesRequisitos_NivelActual",
                table: "ConfiguracionesRequisitos",
                column: "NivelActual");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesRequisitos_NivelActual_NivelSolicitado",
                table: "ConfiguracionesRequisitos",
                columns: new[] { "NivelActual", "NivelSolicitado" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionesRequisitos");

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 4, 7, 17, 33, 396, DateTimeKind.Utc).AddTicks(8158), new DateTime(2023, 7, 4, 7, 17, 33, 396, DateTimeKind.Utc).AddTicks(8156) });

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("8ef569a9-342c-4e85-a8e1-29b5e697f2b6"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 4, 7, 17, 33, 396, DateTimeKind.Utc).AddTicks(8150), new DateTime(2020, 7, 4, 7, 17, 33, 396, DateTimeKind.Utc).AddTicks(7876) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("c24cd969-b99a-4354-b49f-0cae93b0b7ad"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 4, 7, 17, 33, 285, DateTimeKind.Utc).AddTicks(9984), "$2a$11$5i1nxrRlcsYvC7KBr5Gd4efcnkcthsbSHvrPlImGJP/W3BIwR..r2", new DateTime(2020, 7, 4, 7, 17, 33, 285, DateTimeKind.Utc).AddTicks(9694) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 4, 7, 17, 33, 396, DateTimeKind.Utc).AddTicks(2074), "$2a$11$9EJ09MbumBCEiXBFI57tqO.f/QZ6LWVwkeCHeziXlbPAKj2.gUSNq", new DateTime(2025, 6, 4, 7, 17, 33, 396, DateTimeKind.Utc).AddTicks(1980) });
        }
    }
}
