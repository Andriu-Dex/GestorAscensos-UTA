using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataWithAdminAndSteven : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("8ef569a9-342c-4e85-a8e1-29b5e697f2b6"),
                columns: new[] { "Apellidos", "Cedula", "FechaCreacion", "FechaInicioNivelActual", "Nombres" },
                values: new object[] { "Global", "999999999", new DateTime(2025, 6, 29, 22, 54, 47, 805, DateTimeKind.Utc).AddTicks(3203), new DateTime(2020, 6, 29, 22, 54, 47, 805, DateTimeKind.Utc).AddTicks(2822), "Admin" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("c24cd969-b99a-4354-b49f-0cae93b0b7ad"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 6, 29, 22, 54, 47, 674, DateTimeKind.Utc).AddTicks(4703), "$2a$11$BJNwOb1tr/dlva7ZvEk6y.KhZYU/y/FvEqJqIpdGfEXfK3IZ9mFwu", new DateTime(2020, 6, 29, 22, 54, 47, 674, DateTimeKind.Utc).AddTicks(4388) });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Email", "EstaActivo", "FechaCreacion", "FechaModificacion", "IntentosLogin", "PasswordHash", "Rol", "UltimoBloqueado", "UltimoLogin" },
                values: new object[] { new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"), "sparedes@uta.edu.ec", true, new DateTime(2025, 6, 29, 22, 54, 47, 804, DateTimeKind.Utc).AddTicks(5125), null, 0, "$2a$11$9j6NA1/h0FNRg/HD/ZmE.e8ijZ2Jj.dvi6boc57JiZ/cpN3ot6CFu", "Docente", null, new DateTime(2025, 5, 30, 22, 54, 47, 804, DateTimeKind.Utc).AddTicks(5039) });

            migrationBuilder.InsertData(
                table: "Docentes",
                columns: new[] { "Id", "Apellidos", "Cedula", "Email", "EstaActivo", "FechaCreacion", "FechaInicioNivelActual", "FechaModificacion", "FechaNombramiento", "FechaUltimaImportacion", "FechaUltimoAscenso", "HorasCapacitacion", "MesesInvestigacion", "NivelActual", "Nombres", "NumeroObrasAcademicas", "PromedioEvaluaciones", "UsuarioId" },
                values: new object[] { new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8"), "Paredes", "1801000000", "sparedes@uta.edu.ec", true, new DateTime(2025, 6, 29, 22, 54, 47, 805, DateTimeKind.Utc).AddTicks(3211), new DateTime(2023, 6, 29, 22, 54, 47, 805, DateTimeKind.Utc).AddTicks(3209), null, null, null, null, null, null, "Titular1", "Steven", null, null, new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8"));

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"));

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("8ef569a9-342c-4e85-a8e1-29b5e697f2b6"),
                columns: new[] { "Apellidos", "Cedula", "FechaCreacion", "FechaInicioNivelActual", "Nombres" },
                values: new object[] { "Sistema", "1800000000", new DateTime(2025, 6, 29, 7, 29, 27, 141, DateTimeKind.Utc).AddTicks(387), new DateTime(2020, 6, 29, 7, 29, 27, 140, DateTimeKind.Utc).AddTicks(9930), "Administrador" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("c24cd969-b99a-4354-b49f-0cae93b0b7ad"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 6, 29, 7, 29, 27, 140, DateTimeKind.Utc).AddTicks(2564), "$2a$11$Htd5IHWrNNNE9zlTolsnZ.BCk3CAHaoEVr8jH6MFZ1cuLvZecjypC", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }
    }
}
