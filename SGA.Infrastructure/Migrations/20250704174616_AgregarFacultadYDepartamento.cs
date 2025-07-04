using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFacultadYDepartamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepartamentoId",
                table: "Docentes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Facultades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EsActiva = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facultades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departamentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FacultadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departamentos_Facultades_FacultadId",
                        column: x => x.FacultadId,
                        principalTable: "Facultades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
                columns: new[] { "DepartamentoId", "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { null, new DateTime(2025, 7, 4, 17, 46, 15, 605, DateTimeKind.Utc).AddTicks(780), new DateTime(2023, 7, 4, 17, 46, 15, 605, DateTimeKind.Utc).AddTicks(778) });

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("8ef569a9-342c-4e85-a8e1-29b5e697f2b6"),
                columns: new[] { "DepartamentoId", "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { null, new DateTime(2025, 7, 4, 17, 46, 15, 605, DateTimeKind.Utc).AddTicks(771), new DateTime(2020, 7, 4, 17, 46, 15, 605, DateTimeKind.Utc).AddTicks(384) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("c24cd969-b99a-4354-b49f-0cae93b0b7ad"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 4, 17, 46, 15, 437, DateTimeKind.Utc).AddTicks(1509), "$2a$11$hqmt2yQyzPXjajJvXbOFd.vaInnZAaezNqMg264vUXplxs4e1vyye", new DateTime(2020, 7, 4, 17, 46, 15, 437, DateTimeKind.Utc).AddTicks(1276) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 4, 17, 46, 15, 604, DateTimeKind.Utc).AddTicks(3261), "$2a$11$5juwYcYDePaGa13koD/Klu.Re.HL/NKHENcJLNiTBEP.jBa0p5Yda", new DateTime(2025, 6, 4, 17, 46, 15, 604, DateTimeKind.Utc).AddTicks(3123) });

            migrationBuilder.CreateIndex(
                name: "IX_Docentes_DepartamentoId",
                table: "Docentes",
                column: "DepartamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Departamentos_Codigo",
                table: "Departamentos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departamentos_FacultadId",
                table: "Departamentos",
                column: "FacultadId");

            migrationBuilder.CreateIndex(
                name: "IX_Facultades_Codigo",
                table: "Facultades",
                column: "Codigo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Docentes_Departamentos_DepartamentoId",
                table: "Docentes",
                column: "DepartamentoId",
                principalTable: "Departamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Docentes_Departamentos_DepartamentoId",
                table: "Docentes");

            migrationBuilder.DropTable(
                name: "Departamentos");

            migrationBuilder.DropTable(
                name: "Facultades");

            migrationBuilder.DropIndex(
                name: "IX_Docentes_DepartamentoId",
                table: "Docentes");

            migrationBuilder.DropColumn(
                name: "DepartamentoId",
                table: "Docentes");

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
        }
    }
}
