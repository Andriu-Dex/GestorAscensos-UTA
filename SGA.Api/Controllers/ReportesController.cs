using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Application.Interfaces;
using System.Security.Claims;

namespace SGA.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportesController : ControllerBase
{
    private readonly IDocenteService _docenteService;
    private readonly IReporteService _reporteService;
    private readonly ILogger<ReportesController> _logger;

    public ReportesController(IDocenteService docenteService, IReporteService reporteService, ILogger<ReportesController> logger)
    {
        _docenteService = docenteService;
        _reporteService = reporteService;
        _logger = logger;
    }

    [HttpGet("hoja-vida")]
    public async Task<ActionResult> GenerarHojaVida()
    {
        try
        {
            _logger.LogInformation("Iniciando generación de hoja de vida");
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Usuario no autenticado - email no encontrado");
                return Unauthorized();
            }

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
            {
                _logger.LogWarning("Docente no encontrado para email: {Email}", email);
                return NotFound("Docente no encontrado");
            }

            _logger.LogInformation("Generando hoja de vida para docente: {DocenteId}", docente.Id);
            var pdfBytes = await _reporteService.GenerarHojaVidaAsync(docente.Id);
            
            _logger.LogInformation("Hoja de vida generada exitosamente para docente: {DocenteId}, tamaño: {Size} bytes", docente.Id, pdfBytes.Length);
            return File(pdfBytes, "application/pdf", $"HojaVida_{docente.Cedula}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar hoja de vida");
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("solicitud/{solicitudId}")]
    public async Task<ActionResult> GenerarReporteSolicitud(Guid solicitudId)
    {
        try
        {
            _logger.LogInformation("Iniciando generación de reporte para solicitud: {SolicitudId}", solicitudId);
            
            // Verificar permisos
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Usuario no autenticado - email no encontrado");
                return Unauthorized();
            }

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
            {
                _logger.LogWarning("Docente no encontrado para email: {Email}", email);
                return NotFound("Docente no encontrado");
            }

            var esAdmin = User.IsInRole("Administrador");
            _logger.LogInformation("Generando reporte para solicitud: {SolicitudId}, docente: {DocenteId}, esAdmin: {EsAdmin}", 
                solicitudId, docente.Id, esAdmin);
            
            // Generar reporte
            var pdfBytes = await _reporteService.GenerarReporteSolicitudAsync(solicitudId, docente.Id, esAdmin);
            
            _logger.LogInformation("Reporte de solicitud generado exitosamente: {SolicitudId}, tamaño: {Size} bytes", 
                solicitudId, pdfBytes.Length);
            return File(pdfBytes, "application/pdf", $"Solicitud_{solicitudId}.pdf");
        }
        catch (UnauthorizedAccessException)
        {
            _logger.LogWarning("Acceso no autorizado al reporte de solicitud: {SolicitudId}", solicitudId);
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Solicitud no encontrada: {SolicitudId}, Error: {Error}", solicitudId, ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar reporte de solicitud: {SolicitudId}", solicitudId);
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("estadisticas")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult> GenerarEstadisticas()
    {
        try
        {
            var pdfBytes = await _reporteService.GenerarEstadisticasAsync();
            return File(pdfBytes, "application/pdf", "Estadisticas.pdf");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
