using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarControlReutilizacionDocumentosV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaUtilizacion",
                table: "Documentos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FueUtilizadoEnSolicitudAprobada",
                table: "Documentos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "SolicitudAprobadaId",
                table: "Documentos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 7, 6, 40, 31, 706, DateTimeKind.Utc).AddTicks(8248), new DateTime(2023, 7, 7, 6, 40, 31, 706, DateTimeKind.Utc).AddTicks(8245) });

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("8ef569a9-342c-4e85-a8e1-29b5e697f2b6"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 7, 6, 40, 31, 706, DateTimeKind.Utc).AddTicks(8234), new DateTime(2020, 7, 7, 6, 40, 31, 706, DateTimeKind.Utc).AddTicks(7929) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("c24cd969-b99a-4354-b49f-0cae93b0b7ad"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 7, 6, 40, 31, 553, DateTimeKind.Utc).AddTicks(6017), "$2a$11$UuMJZpvVXBQwH1P1EBhZvOECeiH7oJxNTB75rgOXIkxJfVokwN8vG", new DateTime(2020, 7, 7, 6, 40, 31, 553, DateTimeKind.Utc).AddTicks(5729) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 7, 6, 40, 31, 706, DateTimeKind.Utc).AddTicks(1639), "$2a$11$AIxcF1sZFmJflekhCCEbbeaLSnQcAMOXzlKOJrhsRpJGzcWND250S", new DateTime(2025, 6, 7, 6, 40, 31, 706, DateTimeKind.Utc).AddTicks(1533) });

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_SolicitudAprobadaId",
                table: "Documentos",
                column: "SolicitudAprobadaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documentos_SolicitudesAscenso_SolicitudAprobadaId",
                table: "Documentos",
                column: "SolicitudAprobadaId",
                principalTable: "SolicitudesAscenso",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documentos_SolicitudesAscenso_SolicitudAprobadaId",
                table: "Documentos");

            migrationBuilder.DropIndex(
                name: "IX_Documentos_SolicitudAprobadaId",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "FechaUtilizacion",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "FueUtilizadoEnSolicitudAprobada",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "SolicitudAprobadaId",
                table: "Documentos");

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 6, 1, 24, 27, 235, DateTimeKind.Utc).AddTicks(7697), new DateTime(2023, 7, 6, 1, 24, 27, 235, DateTimeKind.Utc).AddTicks(7695) });

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("8ef569a9-342c-4e85-a8e1-29b5e697f2b6"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 6, 1, 24, 27, 235, DateTimeKind.Utc).AddTicks(7682), new DateTime(2020, 7, 6, 1, 24, 27, 235, DateTimeKind.Utc).AddTicks(7254) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("c24cd969-b99a-4354-b49f-0cae93b0b7ad"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 6, 1, 24, 27, 57, DateTimeKind.Utc).AddTicks(3260), "$2a$11$rgF8tGHu4/l7oiruEkl0c.KiZN0uj9yXtBIbAxdyHuG9qyy4cL07W", new DateTime(2020, 7, 6, 1, 24, 27, 57, DateTimeKind.Utc).AddTicks(2959) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 6, 1, 24, 27, 234, DateTimeKind.Utc).AddTicks(8878), "$2a$11$sh6.erhDFnhfxunQlaFxb.7Bz2F5dYxmRzb5X2Wy3f0YQpbDKzio6", new DateTime(2025, 6, 6, 1, 24, 27, 234, DateTimeKind.Utc).AddTicks(8653) });
        }
    }
}
