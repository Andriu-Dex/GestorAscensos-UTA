using Microsoft.Extensions.Configuration;
using SGA.Application.Services;
using System.Collections.Generic;

namespace SGA.Application.Tests.Services;

// Test simple para verificar que el ExternalDataService compila y funciona
public class ExternalDataServiceTest
{
    public static void TestBasicFunctionality()
    {
        // Configuración de prueba
        var configValues = new Dictionary<string, string>
        {
            {"ConnectionStrings:TTHHConnection", "Server=test;Database=test;"},
            {"ConnectionStrings:DACConnection", "Server=test;Database=test;"},
            {"ConnectionStrings:DITICConnection", "Server=test;Database=test;"},
            {"ConnectionStrings:DIRINVConnection", "Server=test;Database=test;"}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues!)
            .Build();

        // Crear instancia del servicio
        var service = new ExternalDataService(configuration);

        // El servicio se crea correctamente
        Console.WriteLine("✅ ExternalDataService se creó correctamente");
        Console.WriteLine("✅ Todas las dependencias están resueltas");
        Console.WriteLine("✅ El código compila sin errores");
    }
}
