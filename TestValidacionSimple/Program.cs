using Microsoft.Extensions.Configuration;
using SGA.Application.Services;
using SGA.Domain.Enums;
using SGA.Application.DTOs.Docentes;

namespace TestValidacionSimple;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("\n🔍 PRUEBA SIMPLIFICADA - IDENTIFICAR PROBLEMA DE VALIDACIÓN");
        Console.WriteLine("===========================================================");

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

        // Crear un caso MANUAL que sabemos que debería fallar
        Console.WriteLine("🧪 PRUEBA CON DATOS SIMULADOS - Docente que NO debe poder ascender");
        Console.WriteLine("──────────────────────────────────────────────────────────────");

        // Crear un DocenteDto manualmente con datos que NO cumplen requisitos
        var docenteQueNoDebeAscender = new DocenteDto
        {
            Cedula = "TEST001",
            Nombres = "Docente",
            Apellidos = "Prueba",
            NivelActual = "Titular1",
            FechaInicioNivelActual = DateTime.Now.AddMonths(-6), // Solo 6 meses (necesita 48)
            FechaNombramiento = DateTime.Now.AddMonths(-6),
            PromedioEvaluaciones = 60m, // Solo 60% (necesita 75%)
            NumeroObrasAcademicas = 0,  // 0 obras (necesita 1)
            HorasCapacitacion = 20,     // Solo 20 horas (necesita 96)
            MesesInvestigacion = 0      // 0 meses (no necesario para Titular 2)
        };

        Console.WriteLine($"📊 Datos del docente de prueba:");
        Console.WriteLine($"   • Tiempo en nivel: 6 meses (necesita 48 meses)");
        Console.WriteLine($"   • Evaluación: 60% (necesita 75%)");
        Console.WriteLine($"   • Obras: 0 (necesita 1)");
        Console.WriteLine($"   • Capacitación: 20 horas (necesita 96)");
        Console.WriteLine();

        var resultado = validacionService.ValidarRequisitos(docenteQueNoDebeAscender);

        Console.WriteLine($"🎯 RESULTADO: {(resultado.PuedeAscender ? "✅ PUEDE ASCENDER" : "❌ NO PUEDE ASCENDER")}");
        Console.WriteLine();
        
        Console.WriteLine("📋 DETALLE DE VALIDACIONES:");
        Console.WriteLine($"   Antigüedad: {(resultado.CumpleAntiguedad ? "✅" : "❌")} ({resultado.AñosActuales} de {resultado.AñosRequeridos} años)");
        Console.WriteLine($"   Obras: {(resultado.CumpleObras ? "✅" : "❌")} ({resultado.ObrasActuales} de {resultado.ObrasRequeridas} obras)");
        Console.WriteLine($"   Evaluación: {(resultado.CumpleEvaluacion ? "✅" : "❌")} ({resultado.PromedioActual}% de {resultado.PromedioRequerido}% req.)");
        Console.WriteLine($"   Capacitación: {(resultado.CumpleCapacitacion ? "✅" : "❌")} ({resultado.HorasActuales} de {resultado.HorasRequeridas} horas)");
        Console.WriteLine($"   Investigación: {(resultado.CumpleInvestigacion ? "✅" : "❌")} (requerido: {resultado.MesesRequeridos} meses)");

        // ANÁLISIS CRÍTICO
        Console.WriteLine("\n🔍 ANÁLISIS DEL PROBLEMA:");
        if (resultado.PuedeAscender)
        {
            Console.WriteLine("🚨 ¡PROBLEMA DETECTADO! El sistema dice que puede ascender cuando NO debería");
            Console.WriteLine("   Este docente claramente NO cumple varios requisitos básicos");
            
            if (resultado.CumpleAntiguedad)
                Console.WriteLine("   • ❌ Error: Dice que cumple antigüedad (6 meses < 4 años)");
            if (resultado.CumpleEvaluacion)
                Console.WriteLine("   • ❌ Error: Dice que cumple evaluación (60% < 75%)");
            if (resultado.CumpleObras)
                Console.WriteLine("   • ❌ Error: Dice que cumple obras (0 < 1)");
            if (resultado.CumpleCapacitacion)
                Console.WriteLine("   • ❌ Error: Dice que cumple capacitación (20 < 96 horas)");
        }
        else
        {
            Console.WriteLine("✅ El sistema funciona correctamente - rechaza ascenso como debe ser");
        }

        Console.WriteLine("\n🔚 FIN DE PRUEBA SIMPLIFICADA");
        Console.WriteLine("Presiona cualquier tecla para continuar...");
        Console.ReadKey();
    }
}
