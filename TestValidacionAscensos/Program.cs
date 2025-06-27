using Microsoft.Extensions.Configuration;
using SGA.Application.Services;
using SGA.Domain.Enums;

namespace TestValidacionAscensos;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("\n🧪 PRUEBA DE VALIDACIÓN DE REQUISITOS DE ASCENSO");
        Console.WriteLine("================================================");

        // Configuración
        var configValues = new Dictionary<string, string>
        {
            {"ConnectionStrings:DACConnection", "Server=.\\SQLEXPRESS;Database=DAC;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"},
            {"ConnectionStrings:TTHHConnection", "Server=.\\SQLEXPRESS;Database=TTHH;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"},
            {"ConnectionStrings:DITICConnection", "Server=.\\SQLEXPRESS;Database=DITIC;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"},
            {"ConnectionStrings:DIRINVConnection", "Server=.\\SQLEXPRESS;Database=DIRINV;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues!)
            .Build();

        var externalDataService = new ExternalDataService(configuration);
        var validacionService = new ValidacionAscensoService(externalDataService);

        // Casos de prueba específicos usando cédulas reales
        var casosPrueba = new[]
        {
            new { Cedula = "1805123456", Descripcion = "Steven Alexander Paredes - Docente Titular 2" },
            new { Cedula = "1801000000", Descripcion = "Andriu Dex - Docente Titular" },
            new { Cedula = "1805012341", Descripcion = "Erick Aguilar - Docente Titular" },
        };

        foreach (var caso in casosPrueba)
        {
            try
            {
                Console.WriteLine($"\n🔍 CASO: {caso.Descripcion}");
                Console.WriteLine($"   Cédula: {caso.Cedula}");
                Console.WriteLine("   " + new string('─', 50));

                var resultado = await validacionService.ValidarRequisitosConDatosExternosAsync(caso.Cedula);

                Console.WriteLine($"   🎯 RESULTADO GENERAL: {(resultado.PuedeAscender ? "✅ PUEDE ASCENDER" : "❌ NO PUEDE ASCENDER")}");
                Console.WriteLine($"   📊 Nivel actual: {resultado.NivelActual} → {resultado.NivelDestino}");
                Console.WriteLine();

                // Detallar cada requisito
                Console.WriteLine("   📋 DETALLE DE REQUISITOS:");
                Console.WriteLine($"      ⏰ Antigüedad: {(resultado.CumpleAntiguedad ? "✅" : "❌")} ({resultado.AñosActuales} de {resultado.AñosRequeridos} años)");
                Console.WriteLine($"      📚 Obras académicas: {(resultado.CumpleObras ? "✅" : "❌")} ({resultado.ObrasActuales} de {resultado.ObrasRequeridas} obras)");
                Console.WriteLine($"      📊 Evaluación docente: {(resultado.CumpleEvaluacion ? "✅" : "❌")} ({resultado.PromedioActual:F1}% de {resultado.PromedioRequerido}% requerido)");
                Console.WriteLine($"      🎓 Capacitación: {(resultado.CumpleCapacitacion ? "✅" : "❌")} ({resultado.HorasActuales} de {resultado.HorasRequeridas} horas)");
                
                if (resultado.MesesRequeridos > 0)
                {
                    Console.WriteLine($"      🔬 Investigación: {(resultado.CumpleInvestigacion ? "✅" : "❌")} ({resultado.MesesActuales} de {resultado.MesesRequeridos} meses)");
                }

                // Mostrar observaciones si las hay
                if (!string.IsNullOrEmpty(resultado.Observaciones))
                {
                    Console.WriteLine($"   📝 Observaciones: {resultado.Observaciones}");
                }

                // Mostrar requisitos faltantes
                if (resultado.RequisitosFaltantes.Any())
                {
                    Console.WriteLine($"   ⚠️  REQUISITOS FALTANTES:");
                    foreach (var faltante in resultado.RequisitosFaltantes)
                    {
                        Console.WriteLine($"      • {faltante}");
                    }
                }

                // ANÁLISIS CRÍTICO - Identificar problemas de validación
                if (resultado.PuedeAscender)
                {
                    Console.WriteLine($"   ✅ Este docente puede ascender según el sistema");
                }
                else
                {
                    Console.WriteLine($"   ❌ Este docente NO puede ascender - Validación correcta");
                }
                
                // Verificar si hay inconsistencias
                if (!resultado.CumpleEvaluacion && resultado.PuedeAscender)
                {
                    Console.WriteLine($"   🚨 ¡PROBLEMA DETECTADO! El docente NO cumple evaluación pero puede ascender");
                }
                
                if (!resultado.CumpleAntiguedad && resultado.PuedeAscender)
                {
                    Console.WriteLine($"   🚨 ¡PROBLEMA DETECTADO! El docente NO cumple antigüedad pero puede ascender");
                }
                
                if (!resultado.CumpleObras && resultado.PuedeAscender)
                {
                    Console.WriteLine($"   🚨 ¡PROBLEMA DETECTADO! El docente NO cumple obras pero puede ascender");
                }
                
                if (!resultado.CumpleCapacitacion && resultado.PuedeAscender)
                {
                    Console.WriteLine($"   🚨 ¡PROBLEMA DETECTADO! El docente NO cumple capacitación pero puede ascender");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ ERROR: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"      Inner: {ex.InnerException.Message}");
                }
            }

            Console.WriteLine();
        }

        Console.WriteLine("\n🔚 FIN DE PRUEBAS DE VALIDACIÓN");
        Console.WriteLine("Presiona cualquier tecla para continuar...");
        Console.ReadKey();
    }
}
