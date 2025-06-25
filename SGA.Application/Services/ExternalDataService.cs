using SGA.Application.DTOs.Docentes;
using SGA.Application.DTOs.ExternalData;
using SGA.Application.Interfaces;

namespace SGA.Application.Services;

public class ExternalDataService : IExternalDataService
{
    // Implementación simulada para demostración
    // En un sistema real, aquí se inyectarían las interfaces de los repositorios externos

    public async Task<ImportarDatosResponse> ImportarDesdeTTHHAsync(string cedula)
    {
        await Task.Delay(500); // Simular latencia de red

        // Simular datos reales basados en la cédula
        var añosAntiguedad = GetSimulatedAntiguedad(cedula);
        
        var datosSimulados = new Dictionary<string, object?>
        {
            ["FechaNombramiento"] = DateTime.UtcNow.AddYears(-añosAntiguedad),
            ["CargoActual"] = $"Docente Titular {GetSimulatedNivel(cedula)}",
            ["Departamento"] = "Facultad de Ingeniería"
        };

        return new ImportarDatosResponse
        {
            Exitoso = true,
            Mensaje = "Datos importados exitosamente desde TTHH",
            DatosImportados = datosSimulados
        };
    }

    public async Task<ImportarDatosResponse> ImportarDesdeDADACAsync(string cedula)
    {
        await Task.Delay(500);

        // Simular evaluaciones basadas en la cédula
        var promedio = GetSimulatedPromedio(cedula);

        var datosSimulados = new Dictionary<string, object?>
        {
            ["PromedioEvaluaciones"] = promedio,
            ["NumeroEvaluaciones"] = 8,
            ["UltimaEvaluacion"] = DateTime.UtcNow.AddMonths(-3)
        };

        return new ImportarDatosResponse
        {
            Exitoso = true,
            Mensaje = "Datos importados exitosamente desde DAC",
            DatosImportados = datosSimulados
        };
    }

    public async Task<ImportarDatosResponse> ImportarDesdeDITICAsync(string cedula)
    {
        await Task.Delay(500);

        // Simular horas de capacitación basadas en la cédula
        var horas = GetSimulatedHoras(cedula);

        var datosSimulados = new Dictionary<string, object?>
        {
            ["HorasCapacitacion"] = horas,
            ["NumeroCursos"] = horas / 20,
            ["UltimoCurso"] = DateTime.UtcNow.AddMonths(-2)
        };

        return new ImportarDatosResponse
        {
            Exitoso = true,
            Mensaje = "Datos importados exitosamente desde DITIC",
            DatosImportados = datosSimulados
        };
    }

    public async Task<ImportarDatosResponse> ImportarDesdeDIRINVAsync(string cedula)
    {
        await Task.Delay(500);

        // Simular obras académicas y tiempo de investigación basados en la cédula
        var obras = GetSimulatedObras(cedula);
        var meses = GetSimulatedMesesInvestigacion(cedula);

        var datosSimulados = new Dictionary<string, object?>
        {
            ["NumeroObrasAcademicas"] = obras,
            ["MesesInvestigacion"] = meses,
            ["UltimaPublicacion"] = DateTime.UtcNow.AddMonths(-6),
            ["ProyectosActivos"] = obras > 2 ? 1 : 0
        };

        return new ImportarDatosResponse
        {
            Exitoso = true,
            Mensaje = "Datos importados exitosamente desde DIRINV",
            DatosImportados = datosSimulados
        };
    }

    public async Task<DocenteTTHHDto?> ObtenerDatosDocenteTTHHAsync(string identificacion)
    {
        await Task.Delay(300); // Simular latencia
        
        // Simulación de datos de Talento Humano
        return new DocenteTTHHDto
        {
            Identificacion = identificacion,
            Nombres = "Nombre Ejemplo",
            Apellidos = "Apellido Ejemplo",
            Email = $"{identificacion}@uta.edu.ec",
            Telefono = "099999999",
            Departamento = "Facultad de Ingeniería en Sistemas",
            Cargo = "Docente Titular",
            FechaIngreso = DateTime.Now.AddYears(-GetSimulatedAntiguedad(identificacion)),
            EstaActivo = true,
            TituloAcademico = "PhD en Ciencias de la Computación",
            AniosExperiencia = GetSimulatedAntiguedad(identificacion)
        };
    }

    public async Task<bool> ValidarDocenteEnTTHHAsync(string identificacion)
    {
        await Task.Delay(200); // Simular latencia
        
        // Simulación de validación en sistema de Talento Humano
        return true;
    }

    public async Task<IEnumerable<DocenteTTHHDto>> ObtenerTodosDocentesTTHHAsync()
    {
        await Task.Delay(500); // Simular latencia
        
        // Simulación de obtención de todos los docentes
        var docentes = new List<DocenteTTHHDto>();
        
        for (int i = 1; i <= 10; i++)
        {
            var cedula = $"18001800{i}";
            docentes.Add(new DocenteTTHHDto
            {
                Identificacion = cedula,
                Nombres = $"Nombre{i}",
                Apellidos = $"Apellido{i}",
                Email = $"docente{i}@uta.edu.ec",
                Telefono = $"09999999{i}",
                Departamento = "Facultad de Ingeniería en Sistemas",
                Cargo = "Docente Titular",
                FechaIngreso = DateTime.Now.AddYears(-GetSimulatedAntiguedad(cedula)),
                EstaActivo = true,
                TituloAcademico = "PhD en Ciencias de la Computación",
                AniosExperiencia = GetSimulatedAntiguedad(cedula)
            });
        }
        
        return docentes;
    }

    public async Task SincronizarDatosDocentesAsync()
    {
        await Task.Delay(1000); // Simular proceso de sincronización
        // Implementación real sincronizaría todos los docentes con la base de datos
    }

    // Métodos auxiliares para generar datos consistentes basados en la cédula
    private int GetSimulatedAntiguedad(string cedula)
    {
        var hash = cedula.GetHashCode();
        return Math.Abs(hash % 10) + 2; // Entre 2 y 11 años
    }

    private int GetSimulatedNivel(string cedula)
    {
        var hash = cedula.GetHashCode();
        return Math.Abs(hash % 4) + 1; // Entre 1 y 4
    }

    private decimal GetSimulatedPromedio(string cedula)
    {
        var hash = cedula.GetHashCode();
        var base_score = 70 + (Math.Abs(hash % 30)); // Entre 70 y 99
        return Math.Round((decimal)base_score, 2);
    }

    private int GetSimulatedHoras(string cedula)
    {
        var hash = cedula.GetHashCode();
        return Math.Abs(hash % 150) + 50; // Entre 50 y 199 horas
    }

    private int GetSimulatedObras(string cedula)
    {
        var hash = cedula.GetHashCode();
        return Math.Abs(hash % 6); // Entre 0 y 5 obras
    }

    private int GetSimulatedMesesInvestigacion(string cedula)
    {
        var hash = cedula.GetHashCode();
        return Math.Abs(hash % 30) + 6; // Entre 6 y 35 meses
    }
}
