using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGA.Application.DTOs.Admin;
using SGA.Application.Interfaces;
using SGA.Domain.Enums;
using System.Globalization;

namespace SGA.Application.Services
{
    public class EstadisticasService : IEstadisticasService
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<EstadisticasService> _logger;

        public EstadisticasService(IApplicationDbContext context, ILogger<EstadisticasService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<EstadisticasCompletasDto> GetEstadisticasCompletasAsync()
        {
            try
            {
                _logger.LogInformation("Iniciando obtención de estadísticas completas");
                
                var fechaActual = DateTime.UtcNow;
                var inicioMes = new DateTime(fechaActual.Year, fechaActual.Month, 1);
                var inicioAnio = new DateTime(fechaActual.Year, 1, 1);

                // Obtener estadísticas de docentes
                _logger.LogDebug("Obteniendo estadísticas de docentes");
                var totalDocentes = await GetTotalDocentesAsync();
                var docentesPorNivel = await GetDocentesPorNivelAsync();

                // Obtener estadísticas de solicitudes
                _logger.LogDebug("Obteniendo estadísticas de solicitudes");
                var totalSolicitudes = await GetTotalSolicitudesAsync();
                var solicitudesPorEstado = await GetSolicitudesPorEstadoAsync();
                var solicitudesEsteMes = await GetSolicitudesEsteMesAsync(inicioMes);
                var ascensosEsteAnio = await GetAscensosEsteAnioAsync(inicioAnio);

                _logger.LogInformation("Estadísticas completas obtenidas exitosamente");

                return new EstadisticasCompletasDto
                {
                    TotalDocentes = totalDocentes,
                    TotalSolicitudes = totalSolicitudes,
                    SolicitudesPendientes = solicitudesPorEstado.GetValueOrDefault(EstadoSolicitud.Pendiente, 0),
                    SolicitudesEnProceso = solicitudesPorEstado.GetValueOrDefault(EstadoSolicitud.EnProceso, 0),
                    SolicitudesAprobadas = solicitudesPorEstado.GetValueOrDefault(EstadoSolicitud.Aprobada, 0),
                    SolicitudesRechazadas = solicitudesPorEstado.GetValueOrDefault(EstadoSolicitud.Rechazada, 0),
                    SolicitudesEsteMes = solicitudesEsteMes,
                    AscensosEsteAnio = ascensosEsteAnio,
                    DocentesPorNivel = docentesPorNivel,
                    FechaActualizacion = fechaActual
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas completas: {Message}", ex.Message);
                
                // En caso de error, devolver estadísticas vacías pero válidas
                return new EstadisticasCompletasDto
                {
                    TotalDocentes = 0,
                    TotalSolicitudes = 0,
                    SolicitudesPendientes = 0,
                    SolicitudesEnProceso = 0,
                    SolicitudesAprobadas = 0,
                    SolicitudesRechazadas = 0,
                    SolicitudesEsteMes = 0,
                    AscensosEsteAnio = 0,
                    DocentesPorNivel = new Dictionary<int, int>(),
                    FechaActualizacion = DateTime.UtcNow
                };
            }
        }

        private async Task<int> GetTotalDocentesAsync()
        {
            try
            {
                var count = await _context.Docentes
                    .Include(d => d.Usuario)
                    .Where(d => d.Usuario != null && d.Usuario.Rol == RolUsuario.Docente)
                    .CountAsync();
                _logger.LogDebug("Total docentes: {Count}", count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener total de docentes");
                return 0; // Return 0 instead of throwing to make the service more resilient
            }
        }

        private async Task<Dictionary<int, int>> GetDocentesPorNivelAsync()
        {
            try
            {
                var result = await _context.Docentes
                    .Include(d => d.Usuario)
                    .Where(d => d.Usuario != null && d.Usuario.Rol == RolUsuario.Docente)
                    .GroupBy(d => d.NivelActual)
                    .Select(g => new { Nivel = (int)g.Key, Cantidad = g.Count() })
                    .ToDictionaryAsync(x => x.Nivel, x => x.Cantidad);
                _logger.LogDebug("Docentes por nivel: {Count} niveles", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener docentes por nivel");
                return new Dictionary<int, int>(); // Return empty dictionary instead of throwing
            }
        }

        private async Task<int> GetTotalSolicitudesAsync()
        {
            try
            {
                var count = await _context.SolicitudesAscenso.CountAsync();
                _logger.LogDebug("Total solicitudes: {Count}", count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener total de solicitudes");
                return 0; // Return 0 instead of throwing
            }
        }

        private async Task<Dictionary<EstadoSolicitud, int>> GetSolicitudesPorEstadoAsync()
        {
            try
            {
                var result = await _context.SolicitudesAscenso
                    .GroupBy(s => s.Estado)
                    .Select(g => new { Estado = g.Key, Cantidad = g.Count() })
                    .ToDictionaryAsync(x => x.Estado, x => x.Cantidad);
                _logger.LogDebug("Solicitudes por estado: {Count} estados", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener solicitudes por estado");
                return new Dictionary<EstadoSolicitud, int>(); // Return empty dictionary instead of throwing
            }
        }

        private async Task<int> GetSolicitudesEsteMesAsync(DateTime inicioMes)
        {
            try
            {
                var count = await _context.SolicitudesAscenso
                    .Where(s => s.FechaSolicitud >= inicioMes)
                    .CountAsync();
                _logger.LogDebug("Solicitudes este mes: {Count}", count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener solicitudes este mes");
                return 0; // Return 0 instead of throwing
            }
        }

        private async Task<int> GetAscensosEsteAnioAsync(DateTime inicioAnio)
        {
            try
            {
                var count = await _context.SolicitudesAscenso
                    .Where(s => s.Estado == EstadoSolicitud.Aprobada && 
                               s.FechaAprobacion.HasValue && s.FechaAprobacion.Value >= inicioAnio)
                    .CountAsync();
                _logger.LogDebug("Ascensos este año: {Count}", count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ascensos este año");
                return 0; // Return 0 instead of throwing
            }
        }

        public async Task<EstadisticasGeneralesDto> GetEstadisticasGeneralesAsync()
        {
            var fechaActual = DateTime.UtcNow;

            var totalDocentes = await _context.Docentes
                .Include(d => d.Usuario)
                .Where(d => d.Usuario != null && d.Usuario.Rol == RolUsuario.Docente)
                .CountAsync();

            var solicitudesPorEstado = await _context.SolicitudesAscenso
                .GroupBy(s => s.Estado)
                .Select(g => new { Estado = g.Key, Cantidad = g.Count() })
                .ToDictionaryAsync(x => x.Estado, x => x.Cantidad);

            return new EstadisticasGeneralesDto
            {
                TotalDocentes = totalDocentes,
                SolicitudesPendientes = solicitudesPorEstado.ContainsKey(EstadoSolicitud.Pendiente) ? solicitudesPorEstado[EstadoSolicitud.Pendiente] : 0,
                SolicitudesAprobadas = solicitudesPorEstado.ContainsKey(EstadoSolicitud.Aprobada) ? solicitudesPorEstado[EstadoSolicitud.Aprobada] : 0,
                SolicitudesRechazadas = solicitudesPorEstado.ContainsKey(EstadoSolicitud.Rechazada) ? solicitudesPorEstado[EstadoSolicitud.Rechazada] : 0,
                FechaActualizacion = fechaActual
            };
        }

        public Task<List<EstadisticasFacultadDto>> GetEstadisticasPorFacultadAsync()
        {
            // Por ahora retornar lista vacía hasta que se configure correctamente la relación facultad
            return Task.FromResult(new List<EstadisticasFacultadDto>());
        }

        public async Task<List<EstadisticasNivelDto>> GetEstadisticasPorNivelAsync()
        {
            var estadisticasNivel = new List<EstadisticasNivelDto>();
            var totalDocentes = await _context.Docentes
                .Include(d => d.Usuario)
                .Where(d => d.Usuario != null && d.Usuario.Rol == RolUsuario.Docente)
                .CountAsync();

            foreach (NivelTitular nivel in Enum.GetValues<NivelTitular>())
            {
                var docentesNivel = await _context.Docentes
                    .Include(d => d.Usuario)
                    .Where(d => d.Usuario != null && d.Usuario.Rol == RolUsuario.Docente && d.NivelActual == nivel)
                    .CountAsync();

                var solicitudesNivel = await _context.SolicitudesAscenso
                    .Where(s => s.NivelActual == nivel)
                    .GroupBy(s => s.Estado)
                    .Select(g => new { Estado = g.Key, Cantidad = g.Count() })
                    .ToDictionaryAsync(x => x.Estado, x => x.Cantidad);

                var porcentajeDistribucion = totalDocentes > 0 ? (double)docentesNivel / totalDocentes * 100 : 0;

                estadisticasNivel.Add(new EstadisticasNivelDto
                {
                    Nivel = (int)nivel,
                    NombreNivel = $"Titular {(int)nivel}",
                    TotalDocentes = docentesNivel,
                    SolicitudesPendientes = solicitudesNivel.ContainsKey(EstadoSolicitud.Pendiente) ? solicitudesNivel[EstadoSolicitud.Pendiente] : 0,
                    SolicitudesAprobadas = solicitudesNivel.ContainsKey(EstadoSolicitud.Aprobada) ? solicitudesNivel[EstadoSolicitud.Aprobada] : 0,
                    SolicitudesRechazadas = solicitudesNivel.ContainsKey(EstadoSolicitud.Rechazada) ? solicitudesNivel[EstadoSolicitud.Rechazada] : 0,
                    PorcentajeDistribucion = Math.Round(porcentajeDistribucion, 2)
                });
            }

            return estadisticasNivel;
        }

        public async Task<List<EstadisticasActividadMensualDto>> GetEstadisticasActividadMensualAsync()
        {
            var estadisticasMensuales = new List<EstadisticasActividadMensualDto>();
            var fechaActual = DateTime.UtcNow;
            var cultura = new CultureInfo("es-ES");

            // Obtener estadísticas de los últimos 12 meses
            for (int i = 11; i >= 0; i--)
            {
                var mesReferencia = fechaActual.AddMonths(-i);
                var inicioMes = new DateTime(mesReferencia.Year, mesReferencia.Month, 1);
                var finMes = inicioMes.AddMonths(1).AddDays(-1);

                var solicitudesCreadas = await _context.SolicitudesAscenso
                    .Where(s => s.FechaSolicitud >= inicioMes && s.FechaSolicitud <= finMes)
                    .CountAsync();

                var solicitudesAprobadas = await _context.SolicitudesAscenso
                    .Where(s => s.Estado == EstadoSolicitud.Aprobada && 
                               s.FechaAprobacion.HasValue && s.FechaAprobacion >= inicioMes && s.FechaAprobacion <= finMes)
                    .CountAsync();

                var solicitudesRechazadas = await _context.SolicitudesAscenso
                    .Where(s => s.Estado == EstadoSolicitud.Rechazada && 
                               s.FechaSolicitud >= inicioMes && s.FechaSolicitud <= finMes)
                    .CountAsync();

                estadisticasMensuales.Add(new EstadisticasActividadMensualDto
                {
                    Mes = mesReferencia.Month,
                    Anio = mesReferencia.Year,
                    NombreMes = cultura.DateTimeFormat.GetMonthName(mesReferencia.Month),
                    SolicitudesCreadas = solicitudesCreadas,
                    SolicitudesAprobadas = solicitudesAprobadas,
                    SolicitudesRechazadas = solicitudesRechazadas,
                    AscensosRealizados = solicitudesAprobadas
                });
            }

            return estadisticasMensuales;
        }

        public Task<List<string>> GetFacultadesAsync()
        {
            // Por ahora retornar lista vacía hasta que se configure correctamente la relación facultad
            return Task.FromResult(new List<string>());
        }
    }
}
