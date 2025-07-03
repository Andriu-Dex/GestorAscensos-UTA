using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddArchivoContenidoSolamente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ArchivoContenido",
                table: "SolicitudesObrasAcademicas",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 3, 18, 40, 42, 850, DateTimeKind.Utc).AddTicks(9368), new DateTime(2023, 7, 3, 18, 40, 42, 850, DateTimeKind.Utc).AddTicks(9365) });

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("8ef569a9-342c-4e85-a8e1-29b5e697f2b6"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 3, 18, 40, 42, 850, DateTimeKind.Utc).AddTicks(9354), new DateTime(2020, 7, 3, 18, 40, 42, 850, DateTimeKind.Utc).AddTicks(8746) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("c24cd969-b99a-4354-b49f-0cae93b0b7ad"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 3, 18, 40, 42, 666, DateTimeKind.Utc).AddTicks(1743), "$2a$11$la1YZaVLKcoRQOpjlRP.XuPunKYgKgBRMAs4YcmaPXv/TXrtvpASy", new DateTime(2020, 7, 3, 18, 40, 42, 666, DateTimeKind.Utc).AddTicks(1510) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 3, 18, 40, 42, 850, DateTimeKind.Utc).AddTicks(395), "$2a$11$kG2G3sDLcEDpuQQYUqnITuLwAEda3.RZlLmbxd.1YRnvwvTOShbgu", new DateTime(2025, 6, 3, 18, 40, 42, 850, DateTimeKind.Utc).AddTicks(259) });

            // Verificar si el índice no existe antes de crearlo
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Documentos_DocenteId' AND object_id = OBJECT_ID('Documentos'))
                BEGIN
                    CREATE INDEX IX_Documentos_DocenteId ON Documentos (DocenteId)
                END
            ");

            // Verificar si la foreign key no existe antes de crearla
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Documentos_Docentes_DocenteId')
                BEGIN
                    ALTER TABLE Documentos
                    ADD CONSTRAINT FK_Documentos_Docentes_DocenteId
                    FOREIGN KEY (DocenteId) REFERENCES Docentes(Id) ON DELETE SET NULL
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArchivoContenido",
                table: "SolicitudesObrasAcademicas");

            // Eliminar foreign key si existe
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Documentos_Docentes_DocenteId')
                BEGIN
                    ALTER TABLE Documentos DROP CONSTRAINT FK_Documentos_Docentes_DocenteId
                END
            ");

            // Eliminar índice si existe
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Documentos_DocenteId' AND object_id = OBJECT_ID('Documentos'))
                BEGIN
                    DROP INDEX IX_Documentos_DocenteId ON Documentos
                END
            ");

            // ...existing code... (UpdateData statements)

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
        }
    }
}
