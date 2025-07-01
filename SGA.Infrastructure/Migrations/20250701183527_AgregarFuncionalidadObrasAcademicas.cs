using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFuncionalidadObrasAcademicas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
