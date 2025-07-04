using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Application.DTOs;
using SGA.Application.Interfaces;
using System.Security.Claims;

namespace SGA.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador,Admin")]
    public class ReportesAdminController : ControllerBase
    {
        private readonly IReporteAdminService _reporteAdminService;
        private readonly ILogger<ReportesAdminController> _logger;

        public ReportesAdminController(
            IReporteAdminService reporteAdminService,
            ILogger<ReportesAdminController> logger)
        {
            _reporteAdminService = reporteAdminService;
            _logger = logger;
        }

        #region Endpoints PDF

        [HttpPost("procesos-por-estado")]
        public async Task<IActionResult> GenerarReporteProcesosPorEstado([FromBody] FiltroReporteAdminDTO filtro)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                _logger.LogInformation("Admin {Email} generando reporte de procesos por estado", email);

                var pdfBytes = await _reporteAdminService.GenerarReporteProcesosPorEstadoAsync(filtro);
                
                return File(pdfBytes, "application/pdf", 
                    $"reporte_procesos_estado_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando reporte de procesos por estado");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("ascensos-por-facultad")]
        public async Task<IActionResult> GenerarReporteAscensosPorFacultad([FromBody] FiltroReporteAdminDTO filtro)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                _logger.LogInformation("Admin {Email} generando reporte de ascensos por facultad", email);

                var pdfBytes = await _reporteAdminService.GenerarReporteAscensosPorFacultadAsync(filtro);
                
                return File(pdfBytes, "application/pdf", 
                    $"reporte_ascensos_facultad_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando reporte de ascensos por facultad");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("tiempo-resolucion")]
        public async Task<IActionResult> GenerarReporteTiempoResolucion([FromBody] FiltroReporteAdminDTO filtro)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                _logger.LogInformation("Admin {Email} generando reporte de tiempo de resolución", email);

                var pdfBytes = await _reporteAdminService.GenerarReporteTiempoResolucionAsync(filtro);
                
                return File(pdfBytes, "application/pdf", 
                    $"reporte_tiempo_resolucion_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando reporte de tiempo de resolución");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("distribucion-docentes")]
        public async Task<IActionResult> GenerarReporteDistribucionDocentes([FromBody] FiltroReporteAdminDTO filtro)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                _logger.LogInformation("Admin {Email} generando reporte de distribución de docentes", email);

                var pdfBytes = await _reporteAdminService.GenerarReporteDistribucionDocentesAsync(filtro);
                
                return File(pdfBytes, "application/pdf", 
                    $"reporte_distribucion_docentes_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando reporte de distribución de docentes");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("actividad-periodo")]
        public async Task<IActionResult> GenerarReporteActividadPeriodo([FromBody] FiltroReporteAdminDTO filtro)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                _logger.LogInformation("Admin {Email} generando reporte de actividad por período", email);

                var pdfBytes = await _reporteAdminService.GenerarReporteActividadPeriodoAsync(filtro);
                
                return File(pdfBytes, "application/pdf", 
                    $"reporte_actividad_periodo_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando reporte de actividad por período");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("consolidado-gestion")]
        public async Task<IActionResult> GenerarReporteConsolidadoGestion([FromBody] FiltroReporteAdminDTO filtro)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                _logger.LogInformation("Admin {Email} generando reporte consolidado de gestión", email);

                var pdfBytes = await _reporteAdminService.GenerarReporteConsolidadoGestionAsync(filtro);
                
                return File(pdfBytes, "application/pdf", 
                    $"reporte_consolidado_gestion_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando reporte consolidado de gestión");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        #endregion

        #region Endpoints Vista HTML

        [HttpGet("vista/procesos-por-estado")]
        public async Task<ActionResult<string>> VistaReporteProcesosPorEstado(
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null,
            [FromQuery] string? estado = null,
            [FromQuery] Guid? facultadId = null)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                _logger.LogInformation("Admin {Email} generando vista HTML de procesos por estado", email);

                var filtro = new FiltroReporteAdminDTO
                {
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    Estado = estado,
                    FacultadId = facultadId
                };

                var html = await _reporteAdminService.GenerarVistaReporteProcesosPorEstadoAsync(filtro);
                
                _logger.LogInformation("Vista HTML generada exitosamente, longitud: {Length} caracteres", html?.Length ?? 0);
                return Ok(html);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando vista HTML de procesos por estado");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("vista/ascensos-por-facultad")]
        public async Task<ActionResult<string>> VistaReporteAscensosPorFacultad(
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null,
            [FromQuery] Guid? facultadId = null)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                _logger.LogInformation("Admin {Email} generando vista HTML de ascensos por facultad", email);

                var filtro = new FiltroReporteAdminDTO
                {
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    FacultadId = facultadId
                };

                var html = await _reporteAdminService.GenerarVistaReporteAscensosPorFacultadAsync(filtro);
                
                _logger.LogInformation("Vista HTML generada exitosamente, longitud: {Length} caracteres", html?.Length ?? 0);
                return Ok(html);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando vista HTML de ascensos por facultad");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("vista/tiempo-resolucion")]
        public async Task<ActionResult<string>> VistaReporteTiempoResolucion(
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null,
            [FromQuery] Guid? facultadId = null)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                _logger.LogInformation("Admin {Email} generando vista HTML de tiempo de resolución", email);

                var filtro = new FiltroReporteAdminDTO
                {
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    FacultadId = facultadId
                };

                var html = await _reporteAdminService.GenerarVistaReporteTiempoResolucionAsync(filtro);
                
                _logger.LogInformation("Vista HTML generada exitosamente, longitud: {Length} caracteres", html?.Length ?? 0);
                return Ok(html);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando vista HTML de tiempo de resolución");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("vista/distribucion-docentes")]
        public async Task<ActionResult<string>> VistaReporteDistribucionDocentes(
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null,
            [FromQuery] Guid? facultadId = null)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                _logger.LogInformation("Admin {Email} generando vista HTML de distribución de docentes", email);

                var filtro = new FiltroReporteAdminDTO
                {
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    FacultadId = facultadId
                };

                var html = await _reporteAdminService.GenerarVistaReporteDistribucionDocentesAsync(filtro);
                
                _logger.LogInformation("Vista HTML generada exitosamente, longitud: {Length} caracteres", html?.Length ?? 0);
                return Ok(html);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando vista HTML de distribución de docentes");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("vista/actividad-periodo")]
        public async Task<ActionResult<string>> VistaReporteActividadPeriodo(
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null,
            [FromQuery] string? periodo = null)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                _logger.LogInformation("Admin {Email} generando vista HTML de actividad por período", email);

                var filtro = new FiltroReporteAdminDTO
                {
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    Periodo = periodo
                };

                var html = await _reporteAdminService.GenerarVistaReporteActividadPeriodoAsync(filtro);
                
                _logger.LogInformation("Vista HTML generada exitosamente, longitud: {Length} caracteres", html?.Length ?? 0);
                return Ok(html);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando vista HTML de actividad por período");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("vista/consolidado-gestion")]
        public async Task<ActionResult<string>> VistaReporteConsolidadoGestion(
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null,
            [FromQuery] Guid? facultadId = null)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                _logger.LogInformation("Admin {Email} generando vista HTML consolidada de gestión", email);

                var filtro = new FiltroReporteAdminDTO
                {
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    FacultadId = facultadId
                };

                var html = await _reporteAdminService.GenerarVistaReporteConsolidadoGestionAsync(filtro);
                
                _logger.LogInformation("Vista HTML generada exitosamente, longitud: {Length} caracteres", html?.Length ?? 0);
                return Ok(html);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando vista HTML consolidada de gestión");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        #endregion
    }
}
