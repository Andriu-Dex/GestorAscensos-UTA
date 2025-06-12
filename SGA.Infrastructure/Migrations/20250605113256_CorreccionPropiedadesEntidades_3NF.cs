using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CorreccionPropiedadesEntidades_3NF : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Activo",
                table: "TiposDocumento",
                newName: "EsActivo");

            migrationBuilder.RenameColumn(
                name: "Activa",
                table: "Facultades",
                newName: "EsActiva");

            migrationBuilder.RenameColumn(
                name: "ColorHex",
                table: "EstadosSolicitud",
                newName: "Color");

            migrationBuilder.RenameColumn(
                name: "Activo",
                table: "EstadosSolicitud",
                newName: "RequiereRevision");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "TiposDocumento",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "SolicitudesAscenso",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Facultades",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "Facultades",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsActivo",
                table: "EstadosSolicitud",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "EstadosSolicitud",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "EstadosSolicitud",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EsActivo", "FechaActualizacion", "RequiereRevision" },
                values: new object[] { true, null, false });

            migrationBuilder.UpdateData(
                table: "EstadosSolicitud",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "EsActivo", "FechaActualizacion", "RequiereRevision" },
                values: new object[] { true, null, false });

            migrationBuilder.UpdateData(
                table: "EstadosSolicitud",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "EsActivo", "FechaActualizacion", "RequiereRevision" },
                values: new object[] { true, null, false });

            migrationBuilder.UpdateData(
                table: "EstadosSolicitud",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "EsActivo", "FechaActualizacion", "RequiereRevision" },
                values: new object[] { true, null, false });

            migrationBuilder.UpdateData(
                table: "EstadosSolicitud",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "EsActivo", "FechaActualizacion", "RequiereRevision" },
                values: new object[] { true, null, false });

            migrationBuilder.UpdateData(
                table: "Facultades",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Color", "FechaActualizacion" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Facultades",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Color", "FechaActualizacion" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Facultades",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Color", "FechaActualizacion" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Facultades",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Color", "FechaActualizacion" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Facultades",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Color", "FechaActualizacion" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaActualizacion",
                value: null);

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaActualizacion",
                value: null);

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaActualizacion",
                value: null);

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaActualizacion",
                value: null);

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaActualizacion",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "TiposDocumento");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "SolicitudesAscenso");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Facultades");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "Facultades");

            migrationBuilder.DropColumn(
                name: "EsActivo",
                table: "EstadosSolicitud");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "EstadosSolicitud");

            migrationBuilder.RenameColumn(
                name: "EsActivo",
                table: "TiposDocumento",
                newName: "Activo");

            migrationBuilder.RenameColumn(
                name: "EsActiva",
                table: "Facultades",
                newName: "Activa");

            migrationBuilder.RenameColumn(
                name: "RequiereRevision",
                table: "EstadosSolicitud",
                newName: "Activo");

            migrationBuilder.RenameColumn(
                name: "Color",
                table: "EstadosSolicitud",
                newName: "ColorHex");

            migrationBuilder.UpdateData(
                table: "EstadosSolicitud",
                keyColumn: "Id",
                keyValue: 1,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "EstadosSolicitud",
                keyColumn: "Id",
                keyValue: 2,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "EstadosSolicitud",
                keyColumn: "Id",
                keyValue: 3,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "EstadosSolicitud",
                keyColumn: "Id",
                keyValue: 4,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "EstadosSolicitud",
                keyColumn: "Id",
                keyValue: 5,
                column: "Activo",
                value: true);
        }
    }
}
