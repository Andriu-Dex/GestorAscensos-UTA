using Microsoft.Data.SqlClient;
using Dapper;

namespace VerificarDB;

public class Program
{
    public static async Task Main()
    {
        var connectionString = "Server=.\\SQLEXPRESS;Database=TTHH;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true";
        
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        Console.WriteLine("📊 ESTRUCTURA DE LA BASE DE DATOS TTHH");
        Console.WriteLine("=====================================");

        // Verificar qué tablas existen
        var tablas = await connection.QueryAsync<dynamic>(@"
            SELECT TABLE_NAME 
            FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_TYPE = 'BASE TABLE'
            ORDER BY TABLE_NAME");

        Console.WriteLine("\n🔍 Tablas disponibles:");
        Console.WriteLine("─────────────────────");
        foreach (var tabla in tablas)
        {
            Console.WriteLine($"  • {tabla.TABLE_NAME}");
        }

        // Verificar columnas de la tabla EmpleadosTTHH
        var columnas = await connection.QueryAsync<dynamic>(@"
            SELECT 
                COLUMN_NAME,
                DATA_TYPE,
                IS_NULLABLE,
                CHARACTER_MAXIMUM_LENGTH
            FROM INFORMATION_SCHEMA.COLUMNS 
            WHERE TABLE_NAME = 'EmpleadosTTHH'
            ORDER BY ORDINAL_POSITION");

        Console.WriteLine("\n🔍 Columnas en la tabla EmpleadosTTHH:");
        Console.WriteLine("─────────────────────────────────────");
        foreach (var columna in columnas)
        {
            var longitud = columna.CHARACTER_MAXIMUM_LENGTH != null ? $"({columna.CHARACTER_MAXIMUM_LENGTH})" : "";
            var nullable = columna.IS_NULLABLE == "YES" ? "NULL" : "NOT NULL";
            Console.WriteLine($"  • {columna.COLUMN_NAME} {columna.DATA_TYPE}{longitud} {nullable}");
        }

        // Verificar columnas de la tabla AccionesPersonalTTHH
        var columnasAcciones = await connection.QueryAsync<dynamic>(@"
            SELECT 
                COLUMN_NAME,
                DATA_TYPE,
                IS_NULLABLE,
                CHARACTER_MAXIMUM_LENGTH
            FROM INFORMATION_SCHEMA.COLUMNS 
            WHERE TABLE_NAME = 'AccionesPersonalTTHH'
            ORDER BY ORDINAL_POSITION");

        Console.WriteLine("\n🔍 Columnas en la tabla AccionesPersonalTTHH:");
        Console.WriteLine("───────────────────────────────────────────");
        foreach (var columna in columnasAcciones)
        {
            var longitud = columna.CHARACTER_MAXIMUM_LENGTH != null ? $"({columna.CHARACTER_MAXIMUM_LENGTH})" : "";
            var nullable = columna.IS_NULLABLE == "YES" ? "NULL" : "NOT NULL";
            Console.WriteLine($"  • {columna.COLUMN_NAME} {columna.DATA_TYPE}{longitud} {nullable}");
        }

        // Verificar algunos registros de AccionesPersonalTTHH
        Console.WriteLine("\n📋 Registros de ejemplo de AccionesPersonalTTHH:");
        Console.WriteLine("─────────────────────────────────────────────");
        var registrosAcciones = await connection.QueryAsync<dynamic>(@"
            SELECT TOP 3 * FROM AccionesPersonalTTHH");

        foreach (var registro in registrosAcciones)
        {
            Console.WriteLine($"\n📄 Acción:");
            foreach (var property in ((IDictionary<string, object>)registro))
            {
                Console.WriteLine($"  {property.Key}: {property.Value}");
            }
        }
        Console.WriteLine("\n📋 Registros de ejemplo:");
        Console.WriteLine("─────────────────────────");
        var registros = await connection.QueryAsync<dynamic>(@"
            SELECT TOP 3 * FROM EmpleadosTTHH");

        foreach (var registro in registros)
        {
            Console.WriteLine($"\n📄 Registro:");
            foreach (var property in ((IDictionary<string, object>)registro))
            {
                Console.WriteLine($"  {property.Key}: {property.Value}");
            }
        }

        Console.WriteLine("\n✅ Verificación completada");
    }
}
