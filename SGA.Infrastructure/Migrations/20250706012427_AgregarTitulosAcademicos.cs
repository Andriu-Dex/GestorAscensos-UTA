using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTitulosAcademicos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ConfiguracionesRequisitos_NivelActual_NivelSolicitado",
                table: "ConfiguracionesRequisitos");

            migrationBuilder.AlterColumn<string>(
                name: "NivelSolicitado",
                table: "ConfiguracionesRequisitos",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "NivelActual",
                table: "ConfiguracionesRequisitos",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<Guid>(
                name: "TituloActualId",
                table: "ConfiguracionesRequisitos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TituloSolicitadoId",
                table: "ConfiguracionesRequisitos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TitulosAcademicos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OrdenJerarquico = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EstaActivo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EsTituloSistema = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    NivelEquivalente = table.Column<int>(type: "int", nullable: true),
                    ColorHex = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TitulosAcademicos", x => x.Id);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesRequisitos_NivelActual_NivelSolicitado",
                table: "ConfiguracionesRequisitos",
                columns: new[] { "NivelActual", "NivelSolicitado" });

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesRequisitos_TituloActualId",
                table: "ConfiguracionesRequisitos",
                column: "TituloActualId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesRequisitos_TituloActualId_TituloSolicitadoId",
                table: "ConfiguracionesRequisitos",
                columns: new[] { "TituloActualId", "TituloSolicitadoId" });

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesRequisitos_TituloSolicitadoId",
                table: "ConfiguracionesRequisitos",
                column: "TituloSolicitadoId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ConfiguracionRequisito_TipoNivel",
                table: "ConfiguracionesRequisitos",
                sql: "(NivelActual IS NOT NULL AND TituloActualId IS NULL AND NivelSolicitado IS NOT NULL AND TituloSolicitadoId IS NULL) OR (NivelActual IS NULL AND TituloActualId IS NOT NULL AND NivelSolicitado IS NULL AND TituloSolicitadoId IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_TitulosAcademicos_Codigo",
                table: "TitulosAcademicos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TitulosAcademicos_EstaActivo",
                table: "TitulosAcademicos",
                column: "EstaActivo");

            migrationBuilder.CreateIndex(
                name: "IX_TitulosAcademicos_EsTituloSistema",
                table: "TitulosAcademicos",
                column: "EsTituloSistema");

            migrationBuilder.CreateIndex(
                name: "IX_TitulosAcademicos_Nombre",
                table: "TitulosAcademicos",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TitulosAcademicos_OrdenJerarquico",
                table: "TitulosAcademicos",
                column: "OrdenJerarquico",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ConfiguracionesRequisitos_TitulosAcademicos_TituloActualId",
                table: "ConfiguracionesRequisitos",
                column: "TituloActualId",
                principalTable: "TitulosAcademicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ConfiguracionesRequisitos_TitulosAcademicos_TituloSolicitadoId",
                table: "ConfiguracionesRequisitos",
                column: "TituloSolicitadoId",
                principalTable: "TitulosAcademicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConfiguracionesRequisitos_TitulosAcademicos_TituloActualId",
                table: "ConfiguracionesRequisitos");

            migrationBuilder.DropForeignKey(
                name: "FK_ConfiguracionesRequisitos_TitulosAcademicos_TituloSolicitadoId",
                table: "ConfiguracionesRequisitos");

            migrationBuilder.DropTable(
                name: "TitulosAcademicos");

            migrationBuilder.DropIndex(
                name: "IX_ConfiguracionesRequisitos_NivelActual_NivelSolicitado",
                table: "ConfiguracionesRequisitos");

            migrationBuilder.DropIndex(
                name: "IX_ConfiguracionesRequisitos_TituloActualId",
                table: "ConfiguracionesRequisitos");

            migrationBuilder.DropIndex(
                name: "IX_ConfiguracionesRequisitos_TituloActualId_TituloSolicitadoId",
                table: "ConfiguracionesRequisitos");

            migrationBuilder.DropIndex(
                name: "IX_ConfiguracionesRequisitos_TituloSolicitadoId",
                table: "ConfiguracionesRequisitos");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ConfiguracionRequisito_TipoNivel",
                table: "ConfiguracionesRequisitos");

            migrationBuilder.DropColumn(
                name: "TituloActualId",
                table: "ConfiguracionesRequisitos");

            migrationBuilder.DropColumn(
                name: "TituloSolicitadoId",
                table: "ConfiguracionesRequisitos");

            migrationBuilder.AlterColumn<string>(
                name: "NivelSolicitado",
                table: "ConfiguracionesRequisitos",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NivelActual",
                table: "ConfiguracionesRequisitos",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 4, 23, 42, 0, 108, DateTimeKind.Utc).AddTicks(7989), new DateTime(2023, 7, 4, 23, 42, 0, 108, DateTimeKind.Utc).AddTicks(7987) });

            migrationBuilder.UpdateData(
                table: "Docentes",
                keyColumn: "Id",
                keyValue: new Guid("8ef569a9-342c-4e85-a8e1-29b5e697f2b6"),
                columns: new[] { "FechaCreacion", "FechaInicioNivelActual" },
                values: new object[] { new DateTime(2025, 7, 4, 23, 42, 0, 108, DateTimeKind.Utc).AddTicks(7979), new DateTime(2020, 7, 4, 23, 42, 0, 108, DateTimeKind.Utc).AddTicks(7615) });

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

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesRequisitos_NivelActual_NivelSolicitado",
                table: "ConfiguracionesRequisitos",
                columns: new[] { "NivelActual", "NivelSolicitado" },
                unique: true);
        }
    }
}
