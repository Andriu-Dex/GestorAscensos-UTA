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
                e.Activo
            FROM EmpleadosTTHH e 
            WHERE e.Cedula = @Cedula", new { Cedula = cedula });

        if (empleado == null)
            return null;
            
        // Ahora consultamos acciones de personal para determinar el historial de promociones
        var accionesPersonal = await connection.QueryAsync<dynamic>(@"
            SELECT 
                ap.Tipo,
                ap.Fecha,
                ap.Detalle,
                ap.NivelAnterior,
                ap.NivelNuevo
            FROM AccionesPersonalTTHH ap
            WHERE ap.Cedula = @Cedula 
                AND ap.Tipo = 'PROMOCION'
            ORDER BY ap.Fecha DESC", new { Cedula = cedula });
            
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
        
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        // Consultar evaluaciones de los últimos 4 períodos
        var evaluaciones = await connection.QueryAsync<dynamic>(@"
            SELECT TOP 4
                e.PuntajeTotal as Puntaje,
                p.Nombre as PeriodoAcademico,
                e.FechaEvaluacion
            FROM EvaluacionesDocenteDAC e 
            INNER JOIN PeriodosAcademicosDAC p ON e.PeriodoId = p.Id
            WHERE e.Cedula = @Cedula 
            ORDER BY e.FechaEvaluacion DESC", new { Cedula = cedula });

        if (!evaluaciones.Any())
            return null;

        var promedio = evaluaciones.Average(e => (decimal)e.Puntaje);
        var ultimaEvaluacion = evaluaciones.First();

        return new DTOs.ExternalData.DatosDACDto
        {
            PromedioEvaluaciones = promedio,
            PeriodosEvaluados = evaluaciones.Count(),
            FechaUltimaEvaluacion = ultimaEvaluacion.FechaEvaluacion,
            Evaluaciones = evaluaciones.Select(e => new DTOs.ExternalData.EvaluacionPeriodoDto
            {
                Periodo = e.PeriodoAcademico,
                Calificacion = e.Puntaje,
                Fecha = e.FechaEvaluacion
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
}
