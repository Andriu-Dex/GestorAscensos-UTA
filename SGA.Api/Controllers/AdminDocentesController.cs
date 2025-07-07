using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Application.DTOs.Admin;
using SGA.Application.Interfaces;
using System.Security.Claims;

namespace SGA.Api.Controllers;

[ApiController]
[Route("api/admin/docentes")]
[Authorize(Roles = "Administrador,Admin")]
public class AdminDocentesController : ControllerBase
{
    private readonly IAdminDocenteService _adminDocenteService;
    private readonly ILogger<AdminDocentesController> _logger;

    public AdminDocentesController(
        IAdminDocenteService adminDocenteService,
        ILogger<AdminDocentesController> logger)
    {
        _adminDocenteService = adminDocenteService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los docentes para la vista de administración
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<DocenteAdminDto>>> GetAllDocentes()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("Admin {Email} consultando lista de docentes", email);

            var docentes = await _adminDocenteService.GetAllDocentesAsync();
            return Ok(docentes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener lista de docentes");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene un docente específico por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<DocenteDetalleAdminDto>> GetDocente(Guid id)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("Admin {Email} consultando docente {DocenteId}", email, id);

            var docente = await _adminDocenteService.GetDocenteDetalleAsync(id);
            if (docente == null)
                return NotFound(new { message = "Docente no encontrado" });

            return Ok(docente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener docente {DocenteId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene las solicitudes de un docente específico
    /// </summary>
    [HttpGet("{id}/solicitudes")]
    public async Task<ActionResult<List<SolicitudResumenDto>>> GetSolicitudesDocente(Guid id)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("Admin {Email} consultando solicitudes del docente {DocenteId}", email, id);

            var solicitudes = await _adminDocenteService.GetSolicitudesDocenteAsync(id);
            return Ok(solicitudes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener solicitudes del docente {DocenteId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Genera un reporte PDF de un docente específico
    /// </summary>
    [HttpGet("{id}/reporte")]
    public async Task<IActionResult> GenerarReporteDocente(Guid id)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("Admin {Email} generando reporte del docente {DocenteId}", email, id);

            var pdfBytes = await _adminDocenteService.GenerarReporteDocenteAsync(id);
            if (pdfBytes == null || pdfBytes.Length == 0)
                return NotFound(new { message = "No se pudo generar el reporte" });

            return File(pdfBytes, "application/pdf", 
                $"reporte_docente_{id}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar reporte del docente {DocenteId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza el nivel de un docente
    /// </summary>
    [HttpPut("{id}/nivel")]
    public async Task<ActionResult> ActualizarNivelDocente(Guid id, [FromBody] ActualizarNivelDocenteDto dto)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("Admin {Email} actualizando nivel del docente {DocenteId} a {NuevoNivel}", 
                email, id, dto.NuevoNivel);

            var resultado = await _adminDocenteService.ActualizarNivelDocenteAsync(id, dto, email);
            if (!resultado)
                return BadRequest(new { message = "No se pudo actualizar el nivel del docente" });

            return Ok(new { message = "Nivel actualizado correctamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar nivel del docente {DocenteId}", id);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene estadísticas de docentes
    /// </summary>
    [HttpGet("estadisticas")]
    public async Task<ActionResult<EstadisticasDocentesDto>> GetEstadisticasDocentes()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("Admin {Email} consultando estadísticas de docentes", email);

            var estadisticas = await _adminDocenteService.GetEstadisticasDocentesAsync();
            return Ok(estadisticas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas de docentes");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene la lista de facultades
    /// </summary>
    [HttpGet("facultades")]
    public async Task<ActionResult<List<string>>> GetFacultades()
    {
        try
        {
            var facultades = await _adminDocenteService.GetFacultadesAsync();
            return Ok(facultades);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener facultades");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}
