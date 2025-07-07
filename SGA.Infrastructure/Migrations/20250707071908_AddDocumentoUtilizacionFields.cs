using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentoUtilizacionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Documentos_DocenteId",
                table: "Documentos");

            migrationBuilder.AlterColumn<string>(
                name: "TipoDocumento",
                table: "Documentos",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 7, 7, 19, 8, 232, DateTimeKind.Utc).AddTicks(420), new DateTime(2023, 7, 7, 7, 19, 8, 232, DateTimeKind.Utc).AddTicks(417) });

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("8ef569a9-342c-4e85-a8e1-29b5e697f2b6"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 7, 7, 19, 8, 232, DateTimeKind.Utc).AddTicks(412), new DateTime(2020, 7, 7, 7, 19, 8, 232, DateTimeKind.Utc).AddTicks(127) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("c24cd969-b99a-4354-b49f-0cae93b0b7ad"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 7, 7, 19, 8, 89, DateTimeKind.Utc).AddTicks(3181), "$2a$11$a0vZecgAtYE/glYm4YkemOxFD16/0KUC2E1em/FJkdIRd1Rmmg13u", new DateTime(2020, 7, 7, 7, 19, 8, 89, DateTimeKind.Utc).AddTicks(2979) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 7, 7, 19, 8, 231, DateTimeKind.Utc).AddTicks(4542), "$2a$11$p2hLUXxH6ex5f4JHjweujOjq1Lur0GkXryPM2NoWcRRNCWiTjEmmy", new DateTime(2025, 6, 7, 7, 19, 8, 231, DateTimeKind.Utc).AddTicks(4491) });

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_DocenteId_FueUtilizadoEnSolicitudAprobada",
                table: "Documentos",
                columns: new[] { "DocenteId", "FueUtilizadoEnSolicitudAprobada" });

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_FueUtilizadoEnSolicitudAprobada",
                table: "Documentos",
                column: "FueUtilizadoEnSolicitudAprobada");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_TipoDocumento_FueUtilizadoEnSolicitudAprobada",
                table: "Documentos",
                columns: new[] { "TipoDocumento", "FueUtilizadoEnSolicitudAprobada" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Documentos_DocenteId_FueUtilizadoEnSolicitudAprobada",
                table: "Documentos");

            migrationBuilder.DropIndex(
                name: "IX_Documentos_FueUtilizadoEnSolicitudAprobada",
                table: "Documentos");

            migrationBuilder.DropIndex(
                name: "IX_Documentos_TipoDocumento_FueUtilizadoEnSolicitudAprobada",
                table: "Documentos");

            migrationBuilder.AlterColumn<string>(
                name: "TipoDocumento",
                table: "Documentos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

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
                name: "IX_Documentos_DocenteId",
                table: "Documentos",
                column: "DocenteId");
        }
    }
}
