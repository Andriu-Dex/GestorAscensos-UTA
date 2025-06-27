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

    public ReportesController(IDocenteService docenteService, IReporteService reporteService)
    {
        _docenteService = docenteService;
        _reporteService = reporteService;
    }

    [HttpGet("hoja-vida")]
    public async Task<ActionResult> GenerarHojaVida()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var pdfBytes = await _reporteService.GenerarHojaVidaAsync(docente.Id);
            return File(pdfBytes, "application/pdf", $"HojaVida_{docente.Cedula}.pdf");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("solicitud/{solicitudId}")]
    public async Task<ActionResult> GenerarReporteSolicitud(Guid solicitudId)
    {
        try
        {
            // Verificar permisos
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var esAdmin = User.IsInRole("Administrador");
            
            // Generar reporte
            var pdfBytes = await _reporteService.GenerarReporteSolicitudAsync(solicitudId, docente.Id, esAdmin);
            return File(pdfBytes, "application/pdf", $"Solicitud_{solicitudId}.pdf");
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
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
