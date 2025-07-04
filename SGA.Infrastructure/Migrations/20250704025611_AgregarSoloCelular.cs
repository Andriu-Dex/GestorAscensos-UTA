using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarSoloCelular : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Celular",
                table: "Docentes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
                columns: new[] { "Celular", "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { null, new DateTime(2025, 7, 4, 2, 56, 11, 343, DateTimeKind.Utc).AddTicks(3247), new DateTime(2023, 7, 4, 2, 56, 11, 343, DateTimeKind.Utc).AddTicks(3245) });

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("8ef569a9-342c-4e85-a8e1-29b5e697f2b6"),
                columns: new[] { "Celular", "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { null, new DateTime(2025, 7, 4, 2, 56, 11, 343, DateTimeKind.Utc).AddTicks(3239), new DateTime(2020, 7, 4, 2, 56, 11, 343, DateTimeKind.Utc).AddTicks(2934) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("c24cd969-b99a-4354-b49f-0cae93b0b7ad"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 4, 2, 56, 11, 190, DateTimeKind.Utc).AddTicks(8273), "$2a$11$4/qVBMA43ileUQMa1/FSQe2iAzZ4tjjRla/li3AaLIs..imzGP/2e", new DateTime(2020, 7, 4, 2, 56, 11, 190, DateTimeKind.Utc).AddTicks(8027) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                columns: new[] { "FechaCreacion", "PasswordHash", "UltimoLogin" },
                values: new object[] { new DateTime(2025, 7, 4, 2, 56, 11, 342, DateTimeKind.Utc).AddTicks(6734), "$2a$11$mFPlThc0sz8diuVmogYW7OQS06PIsJywVlgDMBbnnK.rJ/KCaW3mS", new DateTime(2025, 6, 4, 2, 56, 11, 342, DateTimeKind.Utc).AddTicks(6668) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Celular",
                table: "Docentes");

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
        }
    }
}
