using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeDocumentoSolicitudIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documentos_SolicitudesAscenso_SolicitudAscensoId",
                table: "Documentos");

            migrationBuilder.AlterColumn<Guid>(
                name: "SolicitudAscensoId",
                table: "Documentos",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 3, 16, 25, 42, 237, DateTimeKind.Utc).AddTicks(5628), new DateTime(2023, 7, 3, 16, 25, 42, 237, DateTimeKind.Utc).AddTicks(5626) });

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("8ef569a9-342c-4e85-a8e1-29b5e697f2b6"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 3, 16, 25, 42, 237, DateTimeKind.Utc).AddTicks(5610), new DateTime(2020, 7, 3, 16, 25, 42, 237, DateTimeKind.Utc).AddTicks(5248) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("c24cd969-b99a-4354-b49f-0cae93b0b7ad"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 3, 16, 25, 42, 110, DateTimeKind.Utc).AddTicks(3000), "$2a$11$6crq5OhM8ARmxABdo4/peegxqzI9P21E8RLpxXeQmQWTzTUO5Rox6", new DateTime(2020, 7, 3, 16, 25, 42, 110, DateTimeKind.Utc).AddTicks(2684) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 3, 16, 25, 42, 236, DateTimeKind.Utc).AddTicks(7431), "$2a$11$FHAJv33dhswzrY/ULmIujOkEvrTSlk9xxXRVhWSpuvssFpcMdDIuS", new DateTime(2025, 6, 3, 16, 25, 42, 236, DateTimeKind.Utc).AddTicks(7309) });

            migrationBuilder.AddForeignKey(
                name: "FK_Documentos_SolicitudesAscenso_SolicitudAscensoId",
                table: "Documentos",
                column: "SolicitudAscensoId",
                principalTable: "SolicitudesAscenso",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documentos_SolicitudesAscenso_SolicitudAscensoId",
                table: "Documentos");

            migrationBuilder.AlterColumn<Guid>(
                name: "SolicitudAscensoId",
                table: "Documentos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 3, 7, 21, 47, 56, DateTimeKind.Utc).AddTicks(5679), new DateTime(2023, 7, 3, 7, 21, 47, 56, DateTimeKind.Utc).AddTicks(5678) });

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("8ef569a9-342c-4e85-a8e1-29b5e697f2b6"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 3, 7, 21, 47, 56, DateTimeKind.Utc).AddTicks(5664), new DateTime(2020, 7, 3, 7, 21, 47, 56, DateTimeKind.Utc).AddTicks(5381) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("c24cd969-b99a-4354-b49f-0cae93b0b7ad"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 3, 7, 21, 46, 947, DateTimeKind.Utc).AddTicks(7238), "$2a$11$A8stdfxsLMiqfpxdvex50eHgKn2p9nQygY6wosdIz5lVdKvZ.8jIW", new DateTime(2020, 7, 3, 7, 21, 46, 947, DateTimeKind.Utc).AddTicks(6938) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 3, 7, 21, 47, 55, DateTimeKind.Utc).AddTicks(8610), "$2a$11$XjRAs2CiFOrVgyFg6cc5ZuQKVuE/Fvd1cDlVxrTxv.yVX8ZSpQhjy", new DateTime(2025, 6, 3, 7, 21, 47, 55, DateTimeKind.Utc).AddTicks(8494) });

            migrationBuilder.AddForeignKey(
                name: "FK_Documentos_SolicitudesAscenso_SolicitudAscensoId",
                table: "Documentos",
                column: "SolicitudAscensoId",
                principalTable: "SolicitudesAscenso",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
