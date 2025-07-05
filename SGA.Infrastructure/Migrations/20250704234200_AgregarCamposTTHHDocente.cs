using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCamposTTHHDocente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepartamentoTTHH",
                table: "Docentes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FacultadTTHH",
                table: "Docentes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
                columns: new[] { "DepartamentoTTHH", "FacultadTTHH", "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { null, null, new DateTime(2025, 7, 4, 23, 42, 0, 108, DateTimeKind.Utc).AddTicks(7989), new DateTime(2023, 7, 4, 23, 42, 0, 108, DateTimeKind.Utc).AddTicks(7987) });

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("8ef569a9-342c-4e85-a8e1-29b5e697f2b6"),
                columns: new[] { "DepartamentoTTHH", "FacultadTTHH", "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { null, null, new DateTime(2025, 7, 4, 23, 42, 0, 108, DateTimeKind.Utc).AddTicks(7979), new DateTime(2020, 7, 4, 23, 42, 0, 108, DateTimeKind.Utc).AddTicks(7615) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("c24cd969-b99a-4354-b49f-0cae93b0b7ad"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 4, 23, 41, 59, 909, DateTimeKind.Utc).AddTicks(388), "$2a$11$tduzNjNAPD1.2zhVZmhrW.VVp0AwU1OOYb656hMVDw/CQznsFJkt.", new DateTime(2020, 7, 4, 23, 41, 59, 909, DateTimeKind.Utc).AddTicks(75) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 4, 23, 42, 0, 107, DateTimeKind.Utc).AddTicks(9774), "$2a$11$jQFJKDVDtyEcJvqysVgsyOTDx7nPhFy.flhWhT2WximYhrYCtTH/K", new DateTime(2025, 6, 4, 23, 42, 0, 107, DateTimeKind.Utc).AddTicks(9650) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartamentoTTHH",
                table: "Docentes");

            migrationBuilder.DropColumn(
                name: "FacultadTTHH",
                table: "Docentes");

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 4, 17, 46, 15, 605, DateTimeKind.Utc).AddTicks(780), new DateTime(2023, 7, 4, 17, 46, 15, 605, DateTimeKind.Utc).AddTicks(778) });

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("8ef569a9-342c-4e85-a8e1-29b5e697f2b6"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 4, 17, 46, 15, 605, DateTimeKind.Utc).AddTicks(771), new DateTime(2020, 7, 4, 17, 46, 15, 605, DateTimeKind.Utc).AddTicks(384) });

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
        }
    }
}
