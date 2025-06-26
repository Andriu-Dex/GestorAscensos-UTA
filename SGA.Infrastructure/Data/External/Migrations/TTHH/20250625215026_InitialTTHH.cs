using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGA.Infrastructure.Data.External.Migrations.TTHH
{
    /// <inheritdoc />
    public partial class InitialTTHH : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccionesPersonalTTHH",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TipoAccion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaAccion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CargoAnterior = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CargoNuevo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccionesPersonalTTHH", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CargosTTHH",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCargo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NivelTitular = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EstaActivo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargosTTHH", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmpleadosTTHH",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CorreoInstitucional = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Celular = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    FechaNombramiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CargoActual = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NivelAcademico = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstadoCivil = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TipoContrato = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EstaActivo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpleadosTTHH", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "CargosTTHH",
                columns: new[] { "Id", "Descripcion", "EstaActivo", "NivelTitular", "NombreCargo" },
                values: new object[,]
                {
                    { 1, "Nivel inicial", true, "Titular1", "Docente Titular 1" },
                    { 2, "Segundo nivel", true, "Titular2", "Docente Titular 2" },
                    { 3, "Tercer nivel", true, "Titular3", "Docente Titular 3" },
                    { 4, "Cuarto nivel", true, "Titular4", "Docente Titular 4" },
                    { 5, "Nivel máximo", true, "Titular5", "Docente Titular 5" }
                });

            migrationBuilder.InsertData(
                table: "EmpleadosTTHH",
                columns: new[] { "Id", "Apellidos", "CargoActual", "Cedula", "Celular", "CorreoInstitucional", "Direccion", "Email", "EstaActivo", "EstadoCivil", "FechaNacimiento", "FechaNombramiento", "NivelAcademico", "Nombres", "TipoContrato" },
                values: new object[,]
                {
                    { 1, "Paredes", "Docente Titular 5", "1805123456", "0987654321", "sparedes@uta.edu.ec", "Av. Los Shyris 123 y Amazonas, Ambato", "steven.paredes@gmail.com", true, "Soltero", new DateTime(1983, 6, 25, 21, 50, 26, 254, DateTimeKind.Utc).AddTicks(22), new DateTime(2020, 6, 25, 21, 50, 26, 254, DateTimeKind.Utc).AddTicks(373), "Magíster", "Steven Alexander", "Contrato Indefinido" },
                    { 2, "Rodríguez", "Docente Titular 1", "1800000001", "0927754724", "mrodríguez@uta.edu.ec", "Calle 54 #31, Ambato", "mrodríguez@gmail.com", true, "Casado", new DateTime(1990, 6, 25, 21, 50, 26, 254, DateTimeKind.Utc).AddTicks(3157), new DateTime(2021, 6, 25, 21, 50, 26, 254, DateTimeKind.Utc).AddTicks(3153), "Magíster", "María Elena", "Contrato a Término Fijo" },
                    { 3, "Morales", "Docente Titular 3", "1800000002", "0982926281", "cmorales@uta.edu.ec", "Calle 61 #15, Ambato", "cmorales@gmail.com", true, "Casado", new DateTime(1972, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(6930), new DateTime(2021, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(6926), "Licenciado", "Carlos Alberto", "Nombramiento" },
                    { 4, "Vásquez", "Docente Titular 4", "1800000003", "0975968013", "avásquez@uta.edu.ec", "Calle 69 #100, Ambato", "avásquez@gmail.com", true, "Soltero", new DateTime(1976, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(6952), new DateTime(2014, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(6951), "Ingeniero", "Ana Cristina", "Nombramiento" },
                    { 5, "Herrera", "Docente Titular 3", "1800000004", "0947771663", "lherrera@uta.edu.ec", "Calle 19 #10, Ambato", "lherrera@gmail.com", true, "Soltero", new DateTime(1970, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(6982), new DateTime(2015, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(6982), "Licenciado", "Luis Fernando", "Contrato a Término Fijo" },
                    { 6, "Gómez", "Docente Titular 3", "1800000005", "0957331794", "pgómez@uta.edu.ec", "Calle 92 #53, Ambato", "pgómez@gmail.com", true, "Casado", new DateTime(1974, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7002), new DateTime(2020, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7002), "PhD", "Patricia Isabel", "Contrato Indefinido" },
                    { 7, "Pérez", "Docente Titular 1", "1800000006", "0961812327", "jpérez@uta.edu.ec", "Calle 6 #113, Ambato", "jpérez@gmail.com", true, "Casado", new DateTime(1968, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7011), new DateTime(2020, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7011), "PhD", "Jorge Eduardo", "Contrato a Término Fijo" },
                    { 8, "Torres", "Docente Titular 4", "1800000007", "0914965753", "rtorres@uta.edu.ec", "Calle 78 #4, Ambato", "rtorres@gmail.com", true, "Soltero", new DateTime(1994, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7019), new DateTime(2021, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7018), "Licenciado", "Rosa Angélica", "Contrato a Término Fijo" },
                    { 9, "Castillo", "Docente Titular 5", "1800000008", "0994060414", "mcastillo@uta.edu.ec", "Calle 82 #59, Ambato", "mcastillo@gmail.com", true, "Soltero", new DateTime(1978, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7026), new DateTime(2019, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7025), "PhD", "Miguel Ángel", "Nombramiento" },
                    { 10, "Jiménez", "Docente Titular 1", "1800000009", "0963731743", "ljiménez@uta.edu.ec", "Calle 80 #85, Ambato", "ljiménez@gmail.com", true, "Soltero", new DateTime(1966, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7034), new DateTime(2021, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7034), "Magíster", "Lucía Mercedes", "Contrato a Término Fijo" },
                    { 11, "Ramírez", "Docente Titular 4", "1800000010", "0974475477", "framírez@uta.edu.ec", "Calle 17 #195, Ambato", "framírez@gmail.com", true, "Soltero", new DateTime(1992, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7042), new DateTime(2023, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7042), "Ingeniero", "Francisco Javier", "Nombramiento" },
                    { 12, "Vargas", "Docente Titular 2", "1800000011", "0975833180", "cvargas@uta.edu.ec", "Calle 40 #95, Ambato", "cvargas@gmail.com", true, "Casado", new DateTime(1975, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7051), new DateTime(2019, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7051), "PhD", "Carmen Dolores", "Nombramiento" },
                    { 13, "Mendoza", "Docente Titular 5", "1800000012", "0992586029", "rmendoza@uta.edu.ec", "Calle 64 #157, Ambato", "rmendoza@gmail.com", true, "Casado", new DateTime(1990, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7075), new DateTime(2014, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7074), "Ingeniero", "Roberto Carlos", "Nombramiento" },
                    { 14, "Sánchez", "Docente Titular 5", "1800000013", "0985353322", "gsánchez@uta.edu.ec", "Calle 8 #116, Ambato", "gsánchez@gmail.com", true, "Soltero", new DateTime(1975, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7081), new DateTime(2014, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7081), "PhD", "Gloria Patricia", "Contrato a Término Fijo" },
                    { 15, "Guerrero", "Docente Titular 2", "1800000014", "0989853353", "aguerrero@uta.edu.ec", "Calle 61 #193, Ambato", "aguerrero@gmail.com", true, "Casado", new DateTime(1967, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7089), new DateTime(2023, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7089), "Ingeniero", "Andrés Felipe", "Nombramiento" },
                    { 16, "Ortega", "Docente Titular 4", "1800000015", "0980890589", "bortega@uta.edu.ec", "Calle 67 #185, Ambato", "bortega@gmail.com", true, "Soltero", new DateTime(1982, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7095), new DateTime(2023, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7094), "Ingeniero", "Beatriz Elena", "Nombramiento" },
                    { 17, "Silva", "Docente Titular 2", "1800000016", "0937015961", "dsilva@uta.edu.ec", "Calle 65 #154, Ambato", "dsilva@gmail.com", true, "Soltero", new DateTime(1971, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7101), new DateTime(2020, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7101), "Ingeniero", "Daniel Alejandro", "Contrato Indefinido" },
                    { 18, "Ramos", "Docente Titular 3", "1800000017", "0995778409", "mramos@uta.edu.ec", "Calle 35 #99, Ambato", "mramos@gmail.com", true, "Casado", new DateTime(1969, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7110), new DateTime(2020, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7110), "Ingeniero", "Mónica Teresa", "Contrato Indefinido" },
                    { 19, "Paredes", "Docente Titular 4", "1800000018", "0937164923", "hparedes@uta.edu.ec", "Calle 3 #156, Ambato", "hparedes@gmail.com", true, "Casado", new DateTime(1969, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7116), new DateTime(2017, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7116), "PhD", "Héctor Manuel", "Contrato Indefinido" },
                    { 20, "Lozano", "Docente Titular 3", "1800000019", "0919935902", "elozano@uta.edu.ec", "Calle 92 #16, Ambato", "elozano@gmail.com", true, "Casado", new DateTime(1976, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7123), new DateTime(2017, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7122), "Magíster", "Esperanza del Carmen", "Contrato a Término Fijo" },
                    { 21, "Aguilar", "Docente Titular 1", "1800000020", "0934494937", "raguilar@uta.edu.ec", "Calle 78 #46, Ambato", "raguilar@gmail.com", true, "Soltero", new DateTime(1966, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7130), new DateTime(2021, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7130), "PhD", "Raúl Enrique", "Nombramiento" },
                    { 22, "Navarro", "Docente Titular 5", "1800000021", "0978731601", "snavarro@uta.edu.ec", "Calle 80 #32, Ambato", "snavarro@gmail.com", true, "Soltero", new DateTime(1992, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7137), new DateTime(2015, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7136), "Magíster", "Silvia Marcela", "Contrato a Término Fijo" },
                    { 23, "Cevallos", "Docente Titular 1", "1800000022", "0945768909", "pcevallos@uta.edu.ec", "Calle 56 #23, Ambato", "pcevallos@gmail.com", true, "Casado", new DateTime(1991, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7142), new DateTime(2015, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7142), "Licenciado", "Pablo Antonio", "Contrato Indefinido" },
                    { 24, "Vallejo", "Docente Titular 4", "1800000023", "0930399222", "vvallejo@uta.edu.ec", "Calle 7 #108, Ambato", "vvallejo@gmail.com", true, "Casado", new DateTime(1976, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7148), new DateTime(2018, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7148), "Licenciado", "Verónica Alejandra", "Contrato a Término Fijo" },
                    { 25, "Salazar", "Docente Titular 5", "1800000024", "0942505123", "gsalazar@uta.edu.ec", "Calle 20 #93, Ambato", "gsalazar@gmail.com", true, "Soltero", new DateTime(1980, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7154), new DateTime(2016, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7154), "Ingeniero", "Germán Patricio", "Contrato Indefinido" },
                    { 26, "Espinoza", "Docente Titular 4", "1800000025", "0976203441", "gespinoza@uta.edu.ec", "Calle 78 #120, Ambato", "gespinoza@gmail.com", true, "Casado", new DateTime(1994, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7167), new DateTime(2019, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7167), "Licenciado", "Gabriela Fernanda", "Nombramiento" },
                    { 27, "Maldonado", "Docente Titular 1", "1800000026", "0996112540", "jmaldonado@uta.edu.ec", "Calle 96 #158, Ambato", "jmaldonado@gmail.com", true, "Soltero", new DateTime(1975, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7174), new DateTime(2022, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7174), "Magíster", "Juan Carlos", "Nombramiento" },
                    { 28, "Figueroa", "Docente Titular 3", "1800000027", "0959339959", "sfigueroa@uta.edu.ec", "Calle 45 #40, Ambato", "sfigueroa@gmail.com", true, "Casado", new DateTime(1992, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7181), new DateTime(2023, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7181), "Licenciado", "Sandra Milena", "Nombramiento" },
                    { 29, "Cordero", "Docente Titular 4", "1800000028", "0917261075", "fcordero@uta.edu.ec", "Calle 62 #78, Ambato", "fcordero@gmail.com", true, "Soltero", new DateTime(1984, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7187), new DateTime(2021, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7187), "Magíster", "Fernando José", "Nombramiento" },
                    { 30, "Villacís", "Docente Titular 4", "1800000029", "0911884136", "mvillacís@uta.edu.ec", "Calle 99 #104, Ambato", "mvillacís@gmail.com", true, "Soltero", new DateTime(1977, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7194), new DateTime(2017, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7194), "Ingeniero", "Martha Lucía", "Nombramiento" },
                    { 31, "Mantilla", "Docente Titular 4", "1800000030", "0944325810", "rmantilla@uta.edu.ec", "Calle 90 #90, Ambato", "rmantilla@gmail.com", true, "Casado", new DateTime(1967, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7201), new DateTime(2017, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7201), "Magíster", "Rodrigo Esteban", "Nombramiento" },
                    { 32, "Zambrano", "Docente Titular 2", "1800000031", "0925409521", "dzambrano@uta.edu.ec", "Calle 17 #44, Ambato", "dzambrano@gmail.com", true, "Casado", new DateTime(1973, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7207), new DateTime(2016, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7207), "PhD", "Diana Carolina", "Contrato Indefinido" },
                    { 33, "Balarezo", "Docente Titular 3", "1800000032", "0933066735", "ibalarezo@uta.edu.ec", "Calle 99 #121, Ambato", "ibalarezo@gmail.com", true, "Casado", new DateTime(1982, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7213), new DateTime(2018, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7212), "Magíster", "Iván Patricio", "Contrato a Término Fijo" },
                    { 34, "Freire", "Docente Titular 5", "1800000033", "0986004324", "lfreire@uta.edu.ec", "Calle 7 #108, Ambato", "lfreire@gmail.com", true, "Casado", new DateTime(1971, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7221), new DateTime(2020, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7220), "PhD", "Lilian Esperanza", "Contrato a Término Fijo" },
                    { 35, "Santamaría", "Docente Titular 1", "1800000034", "0993709577", "nsantamaría@uta.edu.ec", "Calle 72 #75, Ambato", "nsantamaría@gmail.com", true, "Casado", new DateTime(1983, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7226), new DateTime(2016, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7226), "PhD", "Nelson Rodrigo", "Contrato a Término Fijo" },
                    { 36, "Montoya", "Docente Titular 2", "1800000035", "0995103138", "amontoya@uta.edu.ec", "Calle 25 #127, Ambato", "amontoya@gmail.com", true, "Soltero", new DateTime(1987, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7234), new DateTime(2018, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7234), "Ingeniero", "Alejandra Paola", "Contrato a Término Fijo" },
                    { 37, "Villalba", "Docente Titular 5", "1800000036", "0987586983", "mvillalba@uta.edu.ec", "Calle 54 #189, Ambato", "mvillalba@gmail.com", true, "Soltero", new DateTime(1982, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7241), new DateTime(2017, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7241), "Licenciado", "Mauricio Xavier", "Contrato Indefinido" },
                    { 38, "Ruiz", "Docente Titular 1", "1800000037", "0967399149", "eruiz@uta.edu.ec", "Calle 70 #131, Ambato", "eruiz@gmail.com", true, "Casado", new DateTime(1985, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7247), new DateTime(2019, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7246), "Ingeniero", "Elena Guadalupe", "Contrato a Término Fijo" },
                    { 39, "Carrasco", "Docente Titular 2", "1800000038", "0958261948", "jcarrasco@uta.edu.ec", "Calle 75 #135, Ambato", "jcarrasco@gmail.com", true, "Soltero", new DateTime(1985, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7261), new DateTime(2014, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7261), "Licenciado", "Jaime Ricardo", "Nombramiento" },
                    { 40, "Albán", "Docente Titular 2", "1800000039", "0998115381", "xalbán@uta.edu.ec", "Calle 19 #23, Ambato", "xalbán@gmail.com", true, "Casado", new DateTime(1969, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7267), new DateTime(2015, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7267), "Magíster", "Ximena del Rocío", "Contrato Indefinido" },
                    { 41, "Proaño", "Docente Titular 5", "1800000040", "0916246004", "oproaño@uta.edu.ec", "Calle 15 #145, Ambato", "oproaño@gmail.com", true, "Soltero", new DateTime(1978, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7274), new DateTime(2017, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7274), "Ingeniero", "Oscar Vinicio", "Contrato a Término Fijo" },
                    { 42, "Tapia", "Docente Titular 4", "1800000041", "0951343862", "ptapia@uta.edu.ec", "Calle 63 #156, Ambato", "ptapia@gmail.com", true, "Soltero", new DateTime(1973, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7281), new DateTime(2023, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7280), "Magíster", "Pilar Soledad", "Nombramiento" },
                    { 43, "Velasco", "Docente Titular 2", "1800000042", "0940381829", "evelasco@uta.edu.ec", "Calle 22 #75, Ambato", "evelasco@gmail.com", true, "Casado", new DateTime(1978, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7286), new DateTime(2019, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7286), "Magíster", "Edison Fabián", "Contrato a Término Fijo" },
                    { 44, "Cáceres", "Docente Titular 3", "1800000043", "0933834820", "mcáceres@uta.edu.ec", "Calle 57 #55, Ambato", "mcáceres@gmail.com", true, "Casado", new DateTime(1990, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7292), new DateTime(2022, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7291), "Magíster", "Margarita Isabel", "Contrato Indefinido" },
                    { 45, "Sandoval", "Docente Titular 5", "1800000044", "0978967508", "asandoval@uta.edu.ec", "Calle 99 #187, Ambato", "asandoval@gmail.com", true, "Soltero", new DateTime(1966, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7299), new DateTime(2018, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7298), "Licenciado", "Alberto Raúl", "Nombramiento" },
                    { 46, "Carrión", "Docente Titular 5", "1800000045", "0963901393", "rcarrión@uta.edu.ec", "Calle 4 #20, Ambato", "rcarrión@gmail.com", true, "Casado", new DateTime(1967, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7305), new DateTime(2020, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7304), "Magíster", "Rocío del Pilar", "Contrato a Término Fijo" },
                    { 47, "Mayorga", "Docente Titular 5", "1800000046", "0949642194", "wmayorga@uta.edu.ec", "Calle 91 #90, Ambato", "wmayorga@gmail.com", true, "Soltero", new DateTime(1969, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7312), new DateTime(2019, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7311), "Magíster", "Wilmer Patricio", "Contrato a Término Fijo" },
                    { 48, "Bermúdez", "Docente Titular 3", "1800000047", "0933724367", "nbermúdez@uta.edu.ec", "Calle 75 #161, Ambato", "nbermúdez@gmail.com", true, "Casado", new DateTime(1981, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7318), new DateTime(2018, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7318), "Licenciado", "Nelly Esperanza", "Nombramiento" },
                    { 49, "Solórzano", "Docente Titular 3", "1800000048", "0925845622", "vsolórzano@uta.edu.ec", "Calle 67 #111, Ambato", "vsolórzano@gmail.com", true, "Casado", new DateTime(1968, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7325), new DateTime(2018, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7325), "PhD", "Víctor Hugo", "Nombramiento" },
                    { 50, "Arévalo", "Docente Titular 4", "1800000049", "0936032080", "barévalo@uta.edu.ec", "Calle 7 #35, Ambato", "barévalo@gmail.com", true, "Soltero", new DateTime(1985, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7333), new DateTime(2015, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7332), "PhD", "Blanca Estela", "Contrato a Término Fijo" },
                    { 51, "Tixe", "Docente Titular 3", "1800000050", "0980816429", "mtixe@uta.edu.ec", "Calle 1 #56, Ambato", "mtixe@gmail.com", true, "Casado", new DateTime(1990, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7340), new DateTime(2015, 6, 25, 21, 50, 26, 263, DateTimeKind.Utc).AddTicks(7339), "Ingeniero", "Marco Vinicio", "Contrato a Término Fijo" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccionesPersonalTTHH_Cedula",
                table: "AccionesPersonalTTHH",
                column: "Cedula");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadosTTHH_Cedula",
                table: "EmpleadosTTHH",
                column: "Cedula");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadosTTHH_CorreoInstitucional",
                table: "EmpleadosTTHH",
                column: "CorreoInstitucional",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccionesPersonalTTHH");

            migrationBuilder.DropTable(
                name: "CargosTTHH");

            migrationBuilder.DropTable(
                name: "EmpleadosTTHH");
        }
    }
}
