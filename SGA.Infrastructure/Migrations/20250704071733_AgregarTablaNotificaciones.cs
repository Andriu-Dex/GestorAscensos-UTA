using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablaNotificaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Leida = table.Column<bool>(type: "bit", nullable: false),
                    FechaLeida = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatosAdicionales = table.Column<string>(type: "text", nullable: true),
                    UrlAccion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notificaciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_FechaCreacion",
                table: "Notificaciones",
                column: "FechaCreacion");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_UsuarioId_Leida",
                table: "Notificaciones",
                columns: new[] { "UsuarioId", "Leida" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notificaciones");

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 4, 2, 56, 11, 343, DateTimeKind.Utc).AddTicks(3247), new DateTime(2023, 7, 4, 2, 56, 11, 343, DateTimeKind.Utc).AddTicks(3245) });

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("8ef569a9-342c-4e85-a8e1-29b5e697f2b6"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 4, 2, 56, 11, 343, DateTimeKind.Utc).AddTicks(3239), new DateTime(2020, 7, 4, 2, 56, 11, 343, DateTimeKind.Utc).AddTicks(2934) });

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
    }
}
