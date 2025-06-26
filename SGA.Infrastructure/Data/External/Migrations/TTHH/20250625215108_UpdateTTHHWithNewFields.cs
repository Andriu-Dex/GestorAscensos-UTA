using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGA.Infrastructure.Data.External.Migrations.TTHH
{
    /// <inheritdoc />
    public partial class UpdateTTHHWithNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CargoActual", "FechaNacimiento", "FechaNombramiento", "NivelAcademico" },
                values: new object[] { "Docente Titular 2", new DateTime(1985, 6, 25, 21, 51, 7, 737, DateTimeKind.Utc).AddTicks(2164), new DateTime(2020, 6, 25, 21, 51, 7, 737, DateTimeKind.Utc).AddTicks(2466), "Ingeniero" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico" },
                values: new object[] { "Docente Titular 4", "0976091719", "Calle 67 #80, Ambato", "Soltero", new DateTime(1975, 6, 25, 21, 51, 7, 737, DateTimeKind.Utc).AddTicks(5166), new DateTime(2022, 6, 25, 21, 51, 7, 737, DateTimeKind.Utc).AddTicks(5163), "Ingeniero" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 4", "0914955605", "Calle 66 #73, Ambato", "Soltero", new DateTime(1980, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3389), new DateTime(2018, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3382), "PhD", "Contrato Indefinido" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 5", "0913414899", "Calle 92 #94, Ambato", "Casado", new DateTime(1983, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3417), new DateTime(2016, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3416), "Licenciado", "Contrato a Término Fijo" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento" },
                values: new object[] { "Docente Titular 5", "0912271451", "Calle 28 #77, Ambato", "Casado", new DateTime(1984, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3444), new DateTime(2018, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3444) });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 1", "0964765493", "Calle 86 #105, Ambato", new DateTime(1978, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3462), new DateTime(2019, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3461), "Magíster", "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico" },
                values: new object[] { "Docente Titular 4", "0968672844", "Calle 70 #148, Ambato", "Soltero", new DateTime(1995, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3470), new DateTime(2017, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3470), "Ingeniero" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "0935504592", "Calle 23 #198, Ambato", "Casado", new DateTime(1972, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3477), new DateTime(2021, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3477), "Ingeniero", "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico" },
                values: new object[] { "Docente Titular 1", "0966274061", "Calle 4 #165, Ambato", "Casado", new DateTime(1968, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3484), new DateTime(2015, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3484), "Magíster" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento" },
                values: new object[] { "Docente Titular 2", "0927383473", "Calle 90 #137, Ambato", "Casado", new DateTime(1995, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3492), new DateTime(2021, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3491) });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 5", "0987801229", "Calle 47 #8, Ambato", "Casado", new DateTime(1974, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3499), new DateTime(2015, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3499), "PhD", "Contrato a Término Fijo" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 5", "0988257737", "Calle 64 #192, Ambato", "Soltero", new DateTime(1983, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3507), new DateTime(2019, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3507), "Magíster", "Contrato a Término Fijo" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 2", "0992456988", "Calle 91 #58, Ambato", "Soltero", new DateTime(1980, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3514), new DateTime(2018, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3513), "Licenciado", "Contrato Indefinido" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 4", "0974819902", "Calle 79 #4, Ambato", "Casado", new DateTime(1976, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3520), new DateTime(2014, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3519), "Magíster", "Contrato Indefinido" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 5", "0944345609", "Calle 13 #25, Ambato", new DateTime(1976, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3527), new DateTime(2016, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3526), "PhD", "Contrato Indefinido" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 5", "0932235307", "Calle 4 #85, Ambato", "Casado", new DateTime(1993, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3540), new DateTime(2015, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3540), "PhD", "Contrato a Término Fijo" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 4", "0983946330", "Calle 68 #38, Ambato", new DateTime(1991, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3546), new DateTime(2015, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3546), "Licenciado", "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico" },
                values: new object[] { "Docente Titular 2", "0942012641", "Calle 54 #48, Ambato", "Soltero", new DateTime(1988, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3554), new DateTime(2023, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3554), "Magíster" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico" },
                values: new object[] { "Docente Titular 3", "0921680437", "Calle 18 #17, Ambato", "Soltero", new DateTime(1976, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3560), new DateTime(2023, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3560), "Magíster" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 2", "0979754806", "Calle 75 #98, Ambato", new DateTime(1977, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3565), new DateTime(2015, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3565), "Ingeniero", "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "0944204163", "Calle 22 #138, Ambato", "Casado", new DateTime(1977, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3573), new DateTime(2023, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3572), "Magíster", "Contrato a Término Fijo" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 1", "0992044176", "Calle 31 #70, Ambato", "Casado", new DateTime(1966, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3579), new DateTime(2020, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3578), "PhD", "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "TipoContrato" },
                values: new object[] { "Docente Titular 3", "0983027630", "Calle 87 #82, Ambato", new DateTime(1993, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3584), new DateTime(2017, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3584), "Contrato a Término Fijo" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "TipoContrato" },
                values: new object[] { "Docente Titular 2", "0948542440", "Calle 23 #98, Ambato", "Soltero", new DateTime(1977, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3590), new DateTime(2014, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3590), "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento" },
                values: new object[] { "Docente Titular 4", "0910595046", "Calle 52 #149, Ambato", "Casado", new DateTime(1995, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3596), new DateTime(2016, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3595) });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 5", "0952741165", "Calle 58 #177, Ambato", new DateTime(1992, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3602), new DateTime(2020, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3601), "Ingeniero", "Contrato a Término Fijo" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "TipoContrato" },
                values: new object[] { "Docente Titular 5", "0991659752", "Calle 44 #115, Ambato", "Casado", new DateTime(1985, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3608), new DateTime(2018, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3607), "Contrato Indefinido" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 4", "0931598352", "Calle 56 #197, Ambato", new DateTime(1967, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3614), new DateTime(2016, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3614), "Ingeniero", "Contrato a Término Fijo" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 2", "0955527189", "Calle 51 #113, Ambato", new DateTime(1970, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3626), new DateTime(2021, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3626), "Licenciado", "Contrato Indefinido" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 2", "0983040555", "Calle 88 #176, Ambato", "Casado", new DateTime(1971, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3633), new DateTime(2020, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3632), "Magíster", "Contrato a Término Fijo" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico" },
                values: new object[] { "0951982302", "Calle 96 #61, Ambato", new DateTime(1973, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3639), new DateTime(2022, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3639), "Licenciado" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 3", "0970615057", "Calle 58 #186, Ambato", "Soltero", new DateTime(1975, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3645), new DateTime(2015, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3645), "Licenciado", "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "TipoContrato" },
                values: new object[] { "Docente Titular 4", "0995909615", "Calle 60 #27, Ambato", "Soltero", new DateTime(1971, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3651), new DateTime(2018, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3651), "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento" },
                values: new object[] { "Docente Titular 4", "0984524143", "Calle 59 #191, Ambato", new DateTime(1982, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3658), new DateTime(2022, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3657) });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento" },
                values: new object[] { "0989867810", "Calle 63 #191, Ambato", "Soltero", new DateTime(1967, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3663), new DateTime(2021, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3663) });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "0967794420", "Calle 75 #174, Ambato", "Casado", new DateTime(1980, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3670), new DateTime(2015, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3670), "PhD", "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 2", "0957227719", "Calle 17 #60, Ambato", new DateTime(1988, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3676), new DateTime(2019, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3676), "PhD", "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "TipoContrato" },
                values: new object[] { "0970909628", "Calle 45 #145, Ambato", new DateTime(1994, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3683), new DateTime(2019, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3682), "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 1", "0945798468", "Calle 97 #166, Ambato", "Casado", new DateTime(1977, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3689), new DateTime(2016, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3688), "PhD", "Contrato Indefinido" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento" },
                values: new object[] { "Docente Titular 3", "0994143186", "Calle 31 #173, Ambato", "Soltero", new DateTime(1967, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3695), new DateTime(2015, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3694) });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 2", "0917947065", "Calle 97 #6, Ambato", "Casado", new DateTime(1970, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3701), new DateTime(2021, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3701), "PhD", "Contrato Indefinido" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "TipoContrato" },
                values: new object[] { "Docente Titular 2", "0981932049", "Calle 57 #44, Ambato", "Casado", new DateTime(1969, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3713), new DateTime(2018, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3713), "Contrato Indefinido" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico" },
                values: new object[] { "Docente Titular 3", "0992137207", "Calle 79 #29, Ambato", "Soltero", new DateTime(1978, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3719), new DateTime(2014, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3718), "Ingeniero" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 4", "0976856093", "Calle 39 #2, Ambato", new DateTime(1980, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3725), new DateTime(2022, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3724), "PhD", "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "TipoContrato" },
                values: new object[] { "Docente Titular 1", "0949899043", "Calle 39 #149, Ambato", "Casado", new DateTime(1967, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3732), new DateTime(2017, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3731), "Contrato a Término Fijo" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico" },
                values: new object[] { "Docente Titular 4", "0956809065", "Calle 68 #101, Ambato", new DateTime(1986, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3737), new DateTime(2014, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3737), "PhD" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "0914324516", "Calle 17 #184, Ambato", "Casado", new DateTime(1981, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3744), new DateTime(2016, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3744), "PhD", "Contrato Indefinido" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 2", "0969070575", "Calle 8 #95, Ambato", "Soltero", new DateTime(1979, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3750), new DateTime(2016, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3750), "Magíster", "Contrato Indefinido" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 5", "0949740114", "Calle 55 #50, Ambato", new DateTime(1978, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3758), new DateTime(2014, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3757), "Ingeniero", "Contrato a Término Fijo" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "0939157366", "Calle 62 #145, Ambato", "Casado", new DateTime(1980, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3765), new DateTime(2015, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3764), "Magíster", "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 4", "0980450306", "Calle 17 #181, Ambato", new DateTime(1991, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3771), new DateTime(2016, 6, 25, 21, 51, 7, 746, DateTimeKind.Utc).AddTicks(3771), "Licenciado", "Nombramiento" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CargoActual", "FechaNacimiento", "FechaNombramiento", "NivelAcademico" },
                values: new object[] { "Docente Titular 5", new DateTime(1983, 6, 25, 21, 50, 26, 254, DateTimeKind.Utc).AddTicks(22), new DateTime(2020, 6, 25, 21, 50, 26, 254, DateTimeKind.Utc).AddTicks(373), "Magíster" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico" },
                values: new object[] { "Docente Titular 1", "0927754724", "Calle 54 #31, Ambato", "Casado", new DateTime(1990, 6, 25, 21, 50, 26, 254, DateTimeKind.Utc).AddTicks(3157), new DateTime(2021, 6, 25, 21, 50, 26, 254, DateTimeKind.Utc).AddTicks(3153), "Magíster" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 3", "0982926281", "Calle 61 #15, Ambato", "Casado", new DateTime(1972, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(6930), new DateTime(2021, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(6926), "Licenciado", "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 4", "0975968013", "Calle 69 #100, Ambato", "Soltero", new DateTime(1976, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(6952), new DateTime(2014, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(6951), "Ingeniero", "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento" },
                values: new object[] { "Docente Titular 3", "0947771663", "Calle 19 #10, Ambato", "Soltero", new DateTime(1970, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(6982), new DateTime(2015, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(6982) });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 3", "0957331794", "Calle 92 #53, Ambato", new DateTime(1974, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7002), new DateTime(2020, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7002), "PhD", "Contrato Indefinido" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico" },
                values: new object[] { "Docente Titular 1", "0961812327", "Calle 6 #113, Ambato", "Casado", new DateTime(1968, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7011), new DateTime(2020, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7011), "PhD" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "0914965753", "Calle 78 #4, Ambato", "Soltero", new DateTime(1994, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7019), new DateTime(2021, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7018), "Licenciado", "Contrato a Término Fijo" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico" },
                values: new object[] { "Docente Titular 5", "0994060414", "Calle 82 #59, Ambato", "Soltero", new DateTime(1978, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7026), new DateTime(2019, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7025), "PhD" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento" },
                values: new object[] { "Docente Titular 1", "0963731743", "Calle 80 #85, Ambato", "Soltero", new DateTime(1966, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7034), new DateTime(2021, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7034) });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 4", "0974475477", "Calle 17 #195, Ambato", "Soltero", new DateTime(1992, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7042), new DateTime(2023, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7042), "Ingeniero", "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 2", "0975833180", "Calle 40 #95, Ambato", "Casado", new DateTime(1975, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7051), new DateTime(2019, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7051), "PhD", "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 5", "0992586029", "Calle 64 #157, Ambato", "Casado", new DateTime(1990, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7075), new DateTime(2014, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7074), "Ingeniero", "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 5", "0985353322", "Calle 8 #116, Ambato", "Soltero", new DateTime(1975, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7081), new DateTime(2014, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7081), "PhD", "Contrato a Término Fijo" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 2", "0989853353", "Calle 61 #193, Ambato", new DateTime(1967, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7089), new DateTime(2023, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7089), "Ingeniero", "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 4", "0980890589", "Calle 67 #185, Ambato", "Soltero", new DateTime(1982, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7095), new DateTime(2023, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7094), "Ingeniero", "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 2", "0937015961", "Calle 65 #154, Ambato", new DateTime(1971, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7101), new DateTime(2020, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7101), "Ingeniero", "Contrato Indefinido" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico" },
                values: new object[] { "Docente Titular 3", "0995778409", "Calle 35 #99, Ambato", "Casado", new DateTime(1969, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7110), new DateTime(2020, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7110), "Ingeniero" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico" },
                values: new object[] { "Docente Titular 4", "0937164923", "Calle 3 #156, Ambato", "Casado", new DateTime(1969, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7116), new DateTime(2017, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7116), "PhD" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 3", "0919935902", "Calle 92 #16, Ambato", new DateTime(1976, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7123), new DateTime(2017, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7122), "Magíster", "Contrato a Término Fijo" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "0934494937", "Calle 78 #46, Ambato", "Soltero", new DateTime(1966, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7130), new DateTime(2021, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7130), "PhD", "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 5", "0978731601", "Calle 80 #32, Ambato", "Soltero", new DateTime(1992, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7137), new DateTime(2015, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7136), "Magíster", "Contrato a Término Fijo" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "TipoContrato" },
                values: new object[] { "Docente Titular 1", "0945768909", "Calle 56 #23, Ambato", new DateTime(1991, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7142), new DateTime(2015, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7142), "Contrato Indefinido" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "TipoContrato" },
                values: new object[] { "Docente Titular 4", "0930399222", "Calle 7 #108, Ambato", "Casado", new DateTime(1976, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7148), new DateTime(2018, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7148), "Contrato a Término Fijo" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento" },
                values: new object[] { "Docente Titular 5", "0942505123", "Calle 20 #93, Ambato", "Soltero", new DateTime(1980, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7154), new DateTime(2016, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7154) });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 4", "0976203441", "Calle 78 #120, Ambato", new DateTime(1994, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7167), new DateTime(2019, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7167), "Licenciado", "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "TipoContrato" },
                values: new object[] { "Docente Titular 1", "0996112540", "Calle 96 #158, Ambato", "Soltero", new DateTime(1975, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7174), new DateTime(2022, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7174), "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 3", "0959339959", "Calle 45 #40, Ambato", new DateTime(1992, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7181), new DateTime(2023, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7181), "Licenciado", "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 4", "0917261075", "Calle 62 #78, Ambato", new DateTime(1984, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7187), new DateTime(2021, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7187), "Magíster", "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 4", "0911884136", "Calle 99 #104, Ambato", "Soltero", new DateTime(1977, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7194), new DateTime(2017, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7194), "Ingeniero", "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico" },
                values: new object[] { "0944325810", "Calle 90 #90, Ambato", new DateTime(1967, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7201), new DateTime(2017, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7201), "Magíster" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 2", "0925409521", "Calle 17 #44, Ambato", "Casado", new DateTime(1973, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7207), new DateTime(2016, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7207), "PhD", "Contrato Indefinido" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "TipoContrato" },
                values: new object[] { "Docente Titular 3", "0933066735", "Calle 99 #121, Ambato", "Casado", new DateTime(1982, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7213), new DateTime(2018, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7212), "Contrato a Término Fijo" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento" },
                values: new object[] { "Docente Titular 5", "0986004324", "Calle 7 #108, Ambato", new DateTime(1971, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7221), new DateTime(2020, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7220) });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento" },
                values: new object[] { "0993709577", "Calle 72 #75, Ambato", "Casado", new DateTime(1983, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7226), new DateTime(2016, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7226) });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "0995103138", "Calle 25 #127, Ambato", "Soltero", new DateTime(1987, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7234), new DateTime(2018, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7234), "Ingeniero", "Contrato a Término Fijo" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 5", "0987586983", "Calle 54 #189, Ambato", new DateTime(1982, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7241), new DateTime(2017, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7241), "Licenciado", "Contrato Indefinido" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "TipoContrato" },
                values: new object[] { "0967399149", "Calle 70 #131, Ambato", new DateTime(1985, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7247), new DateTime(2019, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7246), "Contrato a Término Fijo" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 2", "0958261948", "Calle 75 #135, Ambato", "Soltero", new DateTime(1985, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7261), new DateTime(2014, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7261), "Licenciado", "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento" },
                values: new object[] { "Docente Titular 2", "0998115381", "Calle 19 #23, Ambato", "Casado", new DateTime(1969, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7267), new DateTime(2015, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7267) });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 5", "0916246004", "Calle 15 #145, Ambato", "Soltero", new DateTime(1978, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7274), new DateTime(2017, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7274), "Ingeniero", "Contrato a Término Fijo" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "TipoContrato" },
                values: new object[] { "Docente Titular 4", "0951343862", "Calle 63 #156, Ambato", "Soltero", new DateTime(1973, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7281), new DateTime(2023, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7280), "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico" },
                values: new object[] { "Docente Titular 2", "0940381829", "Calle 22 #75, Ambato", "Casado", new DateTime(1978, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7286), new DateTime(2019, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7286), "Magíster" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 3", "0933834820", "Calle 57 #55, Ambato", new DateTime(1990, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7292), new DateTime(2022, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7291), "Magíster", "Contrato Indefinido" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "TipoContrato" },
                values: new object[] { "Docente Titular 5", "0978967508", "Calle 99 #187, Ambato", "Soltero", new DateTime(1966, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7299), new DateTime(2018, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7298), "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico" },
                values: new object[] { "Docente Titular 5", "0963901393", "Calle 4 #20, Ambato", new DateTime(1967, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7305), new DateTime(2020, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7304), "Magíster" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "0949642194", "Calle 91 #90, Ambato", "Soltero", new DateTime(1969, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7312), new DateTime(2019, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7311), "Magíster", "Contrato a Término Fijo" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "CargoActual", "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 3", "0933724367", "Calle 75 #161, Ambato", "Casado", new DateTime(1981, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7318), new DateTime(2018, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7318), "Licenciado", "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 3", "0925845622", "Calle 67 #111, Ambato", new DateTime(1968, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7325), new DateTime(2018, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7325), "PhD", "Nombramiento" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "Celular", "Direccion", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "0936032080", "Calle 7 #35, Ambato", "Soltero", new DateTime(1985, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7333), new DateTime(2015, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7332), "PhD", "Contrato a Término Fijo" });

            migrationBuilder.UpdateData(
                table: "EmpleadosTTHH",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "CargoActual", "Celular", "Direccion", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "TipoContrato" },
                values: new object[] { "Docente Titular 3", "0980816429", "Calle 1 #56, Ambato", new DateTime(1990, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7340), new DateTime(2015, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7339), "Ingeniero", "Contrato a Término Fijo" });
        }
    }
}
