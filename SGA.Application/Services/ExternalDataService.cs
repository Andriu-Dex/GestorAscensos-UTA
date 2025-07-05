using SGA.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Dapper;
using Microsoft.Data.SqlClient;
using SGA.Application.DTOs.ExternalData;
using System.Collections.Generic;

namespace SGA.Application.Services;

public class ExternalDataService : IExternalDataService
{
    private readonly IConfiguration _configuration;

    public ExternalDataService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<DTOs.ExternalData.DatosTTHHDto?> ImportarDatosTTHHAsync(string cedula)
    {
        var connectionString = _configuration.GetConnectionString("TTHHConnection");
        
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        // Primero obtenemos datos completos del empleado
        var empleado = await connection.QueryFirstOrDefaultAsync<dynamic>(@"
            SELECT 
                e.Cedula,
                e.Nombres,
                e.Apellidos,
                e.CargoActual,
                e.FechaNombramiento,
                e.NivelAcademico,
                e.Facultad,
                e.Departamento,
                e.CorreoInstitucional as Email,
                e.EstaActivo as Activo
            FROM EmpleadosTTHH e 
            WHERE e.Cedula = @Cedula", new { Cedula = cedula });

        if (empleado == null)
            return null;
            
        // Ahora consultamos acciones de personal para determinar el historial de promociones
        var accionesPersonal = await connection.QueryAsync<dynamic>(@"
            SELECT 
                ap.TipoAccion as Tipo,
                ap.FechaAccion as Fecha,
                ap.Observaciones as Detalle,
                ap.CargoAnterior as NivelAnterior,
                ap.CargoNuevo as NivelNuevo
            FROM AccionesPersonalTTHH ap
            WHERE ap.Cedula = @Cedula 
                AND ap.TipoAccion = 'PROMOCION'
            ORDER BY ap.FechaAccion DESC", new { Cedula = cedula });
            
        // Determinar la fecha de ingreso al nivel actual usando la acción de personal más reciente
        // o la fecha de nombramiento si no hay acciones de promoción
        DateTime fechaIngresoNivelActual;
        string? nivelActual = null;
        
        var ultimaPromocion = accionesPersonal.FirstOrDefault();
        if (ultimaPromocion != null)
        {
            fechaIngresoNivelActual = ultimaPromocion.Fecha;
            nivelActual = ultimaPromocion.NivelNuevo;
        }
        else
        {
            fechaIngresoNivelActual = empleado.FechaNombramiento;
            
            // Si no hay registro de promociones, intentamos obtener el nivel actual desde otra fuente
            var nivelInfo = await connection.QueryFirstOrDefaultAsync<dynamic>(@"
                SELECT NivelActual
                FROM InformacionAcademicaTTHH
                WHERE Cedula = @Cedula", new { Cedula = cedula });
                
            if (nivelInfo != null)
            {
                nivelActual = nivelInfo.NivelActual;
            }
        }
        
        // Calculamos tiempo en nivel actual
        var tiempoEnNivel = DateTime.Now - fechaIngresoNivelActual;
        var diasEnNivel = (int)tiempoEnNivel.TotalDays;
        
        // Consultamos historial laboral para información adicional
        var historialLaboral = await connection.QueryAsync<dynamic>(@"
            SELECT 
                hl.Cargo,
                hl.Facultad,
                hl.Departamento,
                hl.FechaInicio,
                hl.FechaFin
            FROM HistorialLaboralTTHH hl
            WHERE hl.Cedula = @Cedula 
            ORDER BY hl.FechaInicio DESC", new { Cedula = cedula });
            
        // Convertimos todas las acciones de personal en objetos PromocionDto
        var historialPromociones = accionesPersonal.Select(ap => new DTOs.ExternalData.PromocionDto
        {
            Fecha = ap.Fecha,
            NivelAnterior = ap.NivelAnterior,
            NivelNuevo = ap.NivelNuevo,
            Detalle = ap.Detalle
        }).ToList();
        
        // Construimos y retornamos el DTO completo
        return new DTOs.ExternalData.DatosTTHHDto
        {
            Cedula = empleado.Cedula,
            Nombres = empleado.Nombres,
            Apellidos = empleado.Apellidos,
            FechaNombramiento = empleado.FechaNombramiento,
            CargoActual = empleado.CargoActual,
            FechaInicioCargoActual = fechaIngresoNivelActual,
            FechaIngresoNivelActual = fechaIngresoNivelActual,
            DiasEnNivelActual = diasEnNivel,
            Facultad = empleado.Facultad,
            Departamento = empleado.Departamento ?? string.Empty,
            Email = empleado.Email,
            Activo = empleado.Activo,
            NivelAcademico = empleado.NivelAcademico,
            NivelActual = nivelActual ?? string.Empty,
            HistorialPromociones = historialPromociones
        };
    }

    public async Task<DTOs.ExternalData.DatosDACDto?> ImportarDatosDACAsync(string cedula)
    {
        var connectionString = _configuration.GetConnectionString("DACConnection");
        Console.WriteLine($"[DAC DEBUG] Importando datos para cédula: {cedula}");
        Console.WriteLine($"[DAC DEBUG] Connection string: {connectionString}");
        
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        // Primero verificar si el docente existe
        var docenteExiste = await connection.QueryFirstOrDefaultAsync<int?>(@"
            SELECT COUNT(*) 
            FROM Evaluaciones 
            WHERE Cedula = @Cedula", new { Cedula = cedula });

        Console.WriteLine($"[DAC DEBUG] Evaluaciones encontradas para cédula {cedula}: {docenteExiste}");

        if (docenteExiste == 0)
            return null;

        // Obtener fecha de inicio del nivel actual desde la base de datos principal (SGA)
        DateTime fechaInicioNivel = DateTime.Now.AddYears(-5); // Valor por defecto
        
        var sgaConnectionString = _configuration.GetConnectionString("DefaultConnection");
        using var sgaConnection = new SqlConnection(sgaConnectionString);
        await sgaConnection.OpenAsync();
        
        var fechaInicio = await sgaConnection.QueryFirstOrDefaultAsync<DateTime?>(@"
            SELECT FechaInicioNivelActual 
            FROM Docentes 
            WHERE Cedula = @Cedula AND EstaActivo = 1", 
            new { Cedula = cedula });
            
        if (fechaInicio.HasValue)
        {
            fechaInicioNivel = fechaInicio.Value;
            Console.WriteLine($"[DAC DEBUG] Fecha inicio nivel actual desde SGA: {fechaInicioNivel:yyyy-MM-dd}");
        }
        else
        {
            Console.WriteLine($"[DAC DEBUG] No se encontró fecha de inicio de nivel actual, usando fecha por defecto");
        }

        // Consultar SOLO evaluaciones posteriores a la fecha de inicio en el nivel actual
        var evaluaciones = await connection.QueryAsync<dynamic>(@"
            SELECT 
                e.PuntajeTotal,
                e.PuntajeMaximo,
                (e.PuntajeTotal / e.PuntajeMaximo * 100) as Porcentaje,
                p.Nombre as PeriodoAcademico,
                e.FechaEvaluacion,
                p.FechaInicio,
                p.FechaFin
            FROM Evaluaciones e 
            INNER JOIN Periodos p ON e.PeriodoId = p.Id
            WHERE e.Cedula = @Cedula 
                AND e.FechaEvaluacion >= @FechaInicio
            ORDER BY e.FechaEvaluacion DESC", 
            new { 
                Cedula = cedula, 
                FechaInicio = fechaInicioNivel 
            });

        Console.WriteLine($"[DAC DEBUG] Evaluaciones filtradas por fecha posterior al inicio del cargo: {evaluaciones.Count()}");

        // Si no hay evaluaciones después de la fecha de inicio del cargo, retornar null
        // No usar fallback a evaluaciones anteriores
        if (!evaluaciones.Any())
        {
            Console.WriteLine($"[DAC DEBUG] No se encontraron evaluaciones posteriores a la fecha de inicio del cargo actual ({fechaInicioNivel:yyyy-MM-dd}) para la cédula {cedula}");
            return null;
        }

        var promedioEvaluacion = Math.Round(evaluaciones.Average(e => (decimal)e.Porcentaje), 2);
        var totalPeriodos = evaluaciones.Count();
        var primeraEvaluacion = evaluaciones.Last();
        var ultimaEvaluacion = evaluaciones.First();
        
        Console.WriteLine($"[DAC DEBUG] Promedio calculado: {promedioEvaluacion:F2}%");
        Console.WriteLine($"[DAC DEBUG] Total períodos: {totalPeriodos}");
        
        // Determinar si cumple el requisito (75% mínimo)
        var cumpleRequisito = promedioEvaluacion >= 75.0m;
        var requisitoMinimo = 75.0m;

        Console.WriteLine($"[DAC DEBUG] Cumple requisito: {cumpleRequisito}");

        return new DTOs.ExternalData.DatosDACDto
        {
            PromedioEvaluaciones = promedioEvaluacion,
            PeriodosEvaluados = totalPeriodos,
            FechaUltimaEvaluacion = ultimaEvaluacion.FechaEvaluacion,
            CumpleRequisito = cumpleRequisito,
            RequisitoMinimo = requisitoMinimo,
            PeriodoEvaluado = $"Desde {primeraEvaluacion.FechaEvaluacion:dd/MM/yyyy} hasta {ultimaEvaluacion.FechaEvaluacion:dd/MM/yyyy} (posterior al inicio del cargo actual: {fechaInicioNivel:dd/MM/yyyy})",
            Mensaje = $"Promedio de {totalPeriodos} evaluaciones posteriores al inicio del cargo actual: {promedioEvaluacion:F2}% " + 
                     (cumpleRequisito ? "(Cumple requisito mínimo)" : "(NO cumple requisito mínimo)"),
            Evaluaciones = evaluaciones.Select(e => new DTOs.ExternalData.EvaluacionPeriodoDto
            {
                Periodo = e.PeriodoAcademico,
                Calificacion = e.PuntajeTotal,
                Porcentaje = e.Porcentaje,
                Fecha = e.FechaEvaluacion,
                EstudiantesEvaluaron = 45 // Este valor se puede hacer dinámico si tienes esa información
            }).ToList()
        };
    }

    public async Task<DTOs.ExternalData.DatosDITICDto?> ImportarDatosDITICAsync(string cedula)
    {
        var connectionString = _configuration.GetConnectionString("DITICConnection");
        
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        // Consultar participaciones en cursos de los últimos 3 años
        var fechaCorte = DateTime.Now.AddYears(-3);
        
        var participaciones = await connection.QueryAsync<dynamic>(@"
            SELECT 
                c.Nombre as NombreCurso,
                c.HorasDuracion,
                p.FechaFinalizacion,
                p.Aprobado,
                p.NotaFinal,
                cert.FechaEmision as FechaCertificacion
            FROM ParticipacionesCursoDITIC p
            INNER JOIN CursosDITIC c ON p.CursoId = c.Id
            LEFT JOIN CertificacionesDITIC cert ON p.Cedula = cert.Cedula AND c.Nombre = cert.NombreCertificacion
            WHERE p.Cedula = @Cedula 
                AND p.FechaFinalizacion >= @FechaCorte 
                AND p.Aprobado = 1
            ORDER BY p.FechaFinalizacion DESC", 
            new { Cedula = cedula, FechaCorte = fechaCorte });

        var totalHoras = participaciones.Sum(p => (int)p.HorasDuracion);

        return new DTOs.ExternalData.DatosDITICDto
        {
            HorasCapacitacion = totalHoras,
            CursosCompletados = participaciones.Count(),
            FechaUltimoCurso = participaciones.Any() ? participaciones.First().FechaFinalizacion : null,
            Cursos = participaciones.Select(p => new DTOs.ExternalData.CursoDto
            {
                Nombre = p.NombreCurso,
                Horas = p.HorasDuracion,
                FechaInicio = p.FechaFinalizacion?.AddDays(-p.HorasDuracion) ?? DateTime.MinValue, // Estimación
                FechaFin = p.FechaFinalizacion,
                Completado = p.Aprobado
            }).ToList()
        };
    }

    public async Task<DTOs.ExternalData.DatosDirInvDto?> ImportarDatosDirInvAsync(string cedula)
    {
        var connectionString = _configuration.GetConnectionString("DIRINVConnection");
        
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        // Consultar obras académicas
        var obras = await connection.QueryAsync<dynamic>(@"
            SELECT 
                Titulo,
                TipoObra,
                FechaPublicacion,
                Revista,
                EsIndexada,
                IndiceIndexacion
            FROM ObrasAcademicasDIRINV 
            WHERE Cedula = @Cedula 
            ORDER BY FechaPublicacion DESC", new { Cedula = cedula });

        // Consultar participación en proyectos de investigación
        var proyectos = await connection.QueryAsync<dynamic>(@"
            SELECT 
                p.Titulo,
                p.FechaInicio,
                p.FechaFin,
                part.RolEnProyecto,
                part.HorasSemanales
            FROM ParticipacionesProyectoDIRINV part
            INNER JOIN ProyectosInvestigacionDIRINV p ON part.ProyectoId = p.Id
            WHERE part.Cedula = @Cedula 
            ORDER BY p.FechaInicio DESC", new { Cedula = cedula });

        // Calcular meses en investigación
        var mesesInvestigacion = 0;
        foreach (var proyecto in proyectos)
        {
            var fechaInicio = (DateTime)proyecto.FechaInicio;
            var fechaFin = proyecto.FechaFin != null ? (DateTime)proyecto.FechaFin : DateTime.Now;
            mesesInvestigacion += (int)((fechaFin - fechaInicio).TotalDays / 30.44);
        }

        return new DTOs.ExternalData.DatosDirInvDto
        {
            NumeroObrasAcademicas = obras.Count(),
            NumeroObras = obras.Count(), // Para compatibilidad
            MesesInvestigacion = mesesInvestigacion,
            ProyectosActivos = proyectos.Count(p => p.FechaFin == null),
            FechaUltimaPublicacion = obras.Any() ? obras.First().FechaPublicacion : null,
            Obras = obras.Select(o => new DTOs.ExternalData.ObraAcademicaDto
            {
                Titulo = o.Titulo,
                Tipo = o.TipoObra,
                FechaPublicacion = o.FechaPublicacion,
                Revista = o.Revista ?? string.Empty,
                Autores = string.Empty // Este campo no está en la consulta, se puede agregar después
            }).ToList(),
            Proyectos = proyectos.Select(p => new DTOs.ExternalData.ProyectoInvestigacionDto
            {
                Titulo = p.Titulo,
                FechaInicio = p.FechaInicio,
                FechaFin = p.FechaFin,
                Estado = p.FechaFin == null ? "Activo" : "Finalizado",
                Rol = p.RolEnProyecto
            }).ToList()
        };
    }

    public async Task<List<DTOs.ExternalData.ObraAcademicaConPdfDto>> ObtenerObrasAcademicasConPdfAsync(string cedula)
    {
        var connectionString = _configuration.GetConnectionString("DIRINVConnection");
        
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        // Consultar obras académicas con PDF
        var obras = await connection.QueryAsync<dynamic>(@"
            SELECT 
                Id,
                Titulo,
                TipoObra,
                FechaPublicacion,
                Revista,
                EsIndexada,
                IndiceIndexacion,
                Autores,
                ArchivoPdf,
                NombreArchivo,
                TamañoOriginal
            FROM ObrasAcademicasDIRINV 
            WHERE Cedula = @Cedula 
            AND ArchivoPdf IS NOT NULL
            ORDER BY FechaPublicacion DESC", new { Cedula = cedula });

        return obras.Select(o => new DTOs.ExternalData.ObraAcademicaConPdfDto
        {
            Titulo = o.Titulo ?? string.Empty,
            Tipo = o.TipoObra ?? string.Empty,
            FechaPublicacion = o.FechaPublicacion,
            Revista = o.Revista ?? string.Empty,
            Autores = o.Autores ?? string.Empty,
            PdfComprimido = o.ArchivoPdf as byte[],
            NombreArchivo = o.NombreArchivo ?? $"{o.Titulo}.pdf",
            TamañoOriginal = o.TamañoOriginal
        }).ToList();
    }
}
