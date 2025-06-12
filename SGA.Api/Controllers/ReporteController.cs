using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Application.Services;
using System.Security.Claims;

namespace SGA.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReporteController : ControllerBase
    {
        private readonly IReporteService _reporteService;
        private readonly ILogger<ReporteController> _logger;

        public ReporteController(IReporteService reporteService, ILogger<ReporteController> logger)
        {
            _reporteService = reporteService;
            _logger = logger;
        }

        [HttpGet("docente/{docenteId}")]
        public async Task<IActionResult> GenerarReporteDocente(int docenteId)
        {
            try
            {
                // Verificar permisos: solo el propio docente o un administrador puede ver su reporte
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int usuarioId))
                {
                    return Unauthorized(new { message = "Token no válido" });
                }

                if (usuarioId != docenteId && !User.IsInRole("Administrador"))
                {
                    return Forbid("No tiene permisos para acceder a este reporte");
                }

                var reporteBytes = await _reporteService.GenerarReporteDocenteAsync(docenteId);

                return File(reporteBytes, "application/pdf", $"ReporteDocente_{docenteId}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al generar reporte de docente con ID {docenteId}: {ex.Message}");
                return StatusCode(500, new { message = "Error al generar el reporte" });
            }
        }

        [HttpGet("solicitud/{solicitudId}")]
        public async Task<IActionResult> GenerarReporteSolicitud(int solicitudId)
        {
            try
            {
                // La verificación de permisos se realiza en el servicio de solicitudes
                var reporteBytes = await _reporteService.GenerarReporteSolicitudAsync(solicitudId);

                return File(reporteBytes, "application/pdf", $"ReporteSolicitud_{solicitudId}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al generar reporte de solicitud con ID {solicitudId}: {ex.Message}");
                return StatusCode(500, new { message = "Error al generar el reporte" });
            }
        }

        [HttpGet("mi-reporte")]
        public async Task<IActionResult> GenerarMiReporte()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int docenteId))
                {
                    return Unauthorized(new { message = "Token no válido" });
                }

                var reporteBytes = await _reporteService.GenerarReporteDocenteAsync(docenteId);

                return File(reporteBytes, "application/pdf", $"MiReporte.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte del docente actual: {Message}", ex.Message);
                return StatusCode(500, new { message = "Error al generar el reporte" });
            }
        }
    }
}
