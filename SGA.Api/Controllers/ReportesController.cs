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

    [HttpGet("estado-requisitos")]
    public async Task<ActionResult> GenerarReporteEstadoRequisitos()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var pdfBytes = await _reporteService.GenerarReporteEstadoRequisitosPorNivelAsync(docente.Id);
            return File(pdfBytes, "application/pdf", $"EstadoRequisitos_{docente.Cedula}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar reporte de estado de requisitos");
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("historial-ascensos")]
    public async Task<ActionResult> GenerarReporteHistorialAscensos()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var pdfBytes = await _reporteService.GenerarReporteHistorialAscensosAsync(docente.Id);
            return File(pdfBytes, "application/pdf", $"HistorialAscensos_{docente.Cedula}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar reporte de historial de ascensos");
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("capacitaciones")]
    public async Task<ActionResult> GenerarReporteCapacitaciones()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var pdfBytes = await _reporteService.GenerarReporteCapacitacionesAsync(docente.Id);
            return File(pdfBytes, "application/pdf", $"Capacitaciones_{docente.Cedula}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar reporte de capacitaciones");
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("obras-academicas")]
    public async Task<ActionResult> GenerarReporteObrasAcademicas()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var pdfBytes = await _reporteService.GenerarReporteObrasAcademicasAsync(docente.Id);
            return File(pdfBytes, "application/pdf", $"ObrasAcademicas_{docente.Cedula}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar reporte de obras académicas");
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("completo-ascenso")]
    public async Task<ActionResult> GenerarReporteCompletoAscenso()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var pdfBytes = await _reporteService.GenerarReporteCompletoAscensoAsync(docente.Id);
            return File(pdfBytes, "application/pdf", $"ReporteCompleto_{docente.Cedula}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar reporte completo de ascenso");
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("certificado-estado")]
    public async Task<ActionResult> GenerarCertificadoEstado()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var pdfBytes = await _reporteService.GenerarCertificadoEstadoDocenteAsync(docente.Id);
            return File(pdfBytes, "application/pdf", $"CertificadoEstado_{docente.Cedula}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar certificado de estado");
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // Endpoints para vista previa (HTML para modales)
    [HttpGet("vista/hoja-vida")]
    public async Task<ActionResult<string>> GenerarVistaHojaVida()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Usuario no autenticado - email no encontrado");
                return Unauthorized();
            }
            
            _logger.LogDebug("Email del usuario autenticado: {Email}", email);

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
            {
                _logger.LogWarning("Docente no encontrado para email: {Email}", email);
                return NotFound("Docente no encontrado");
            }
            
            _logger.LogInformation("Generando vista hoja de vida para docente autenticado: {DocenteId}", docente.Id);

            var htmlContent = await _reporteService.GenerarVistaHojaVidaAsync(docente.Id);
            
            _logger.LogInformation("Vista HTML generada exitosamente, longitud: {Length} caracteres", htmlContent?.Length ?? 0);
            return Ok(htmlContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar vista de hoja de vida");
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // Endpoint alternativo para administradores que necesiten ver información de otros docentes
    [HttpGet("vista/hoja-vida/{docenteId}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<string>> GenerarVistaHojaVidaAdmin(Guid docenteId)
    {
        try
        {
            _logger.LogInformation("Administrador solicitando vista hoja de vida para docente: {DocenteId}", docenteId);

            var htmlContent = await _reporteService.GenerarVistaHojaVidaAsync(docenteId);
            
            _logger.LogInformation("Vista HTML generada exitosamente para administrador, longitud: {Length} caracteres", htmlContent?.Length ?? 0);
            return Ok(htmlContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar vista de hoja de vida para administrador - docente: {DocenteId}", docenteId);
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("vista/solicitud/{solicitudId}")]
    public async Task<ActionResult<string>> GenerarVistaSolicitud(Guid solicitudId)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var esAdmin = User.IsInRole("Administrador");
            var htmlContent = await _reporteService.GenerarVistaSolicitudAsync(solicitudId, docente.Id, esAdmin);
            return Ok(htmlContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar vista de solicitud");
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("vista/estado-requisitos")]
    public async Task<ActionResult<string>> GenerarVistaEstadoRequisitos()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var htmlContent = await _reporteService.GenerarVistaEstadoRequisitosAsync(docente.Id);
            return Ok(htmlContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar vista de estado de requisitos");
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("vista/estado-requisitos/{docenteId}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<string>> GenerarVistaEstadoRequisitosAdmin(Guid docenteId)
    {
        try
        {
            var htmlContent = await _reporteService.GenerarVistaEstadoRequisitosAsync(docenteId);
            return Ok(htmlContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar vista de estado de requisitos para administrador");
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("vista/historial-ascensos")]
    public async Task<ActionResult<string>> GenerarVistaHistorialAscensos()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var htmlContent = await _reporteService.GenerarVistaHistorialAscensosAsync(docente.Id);
            return Ok(htmlContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar vista de historial de ascensos");
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("vista/historial-ascensos/{docenteId}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<string>> GenerarVistaHistorialAscensosAdmin(Guid docenteId)
    {
        try
        {
            var htmlContent = await _reporteService.GenerarVistaHistorialAscensosAsync(docenteId);
            return Ok(htmlContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar vista de historial de ascensos para administrador");
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("vista/capacitaciones")]
    public async Task<ActionResult<string>> GenerarVistaCapacitaciones()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var htmlContent = await _reporteService.GenerarVistaCapacitacionesAsync(docente.Id);
            return Ok(htmlContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar vista de capacitaciones");
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("vista/capacitaciones/{docenteId}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<string>> GenerarVistaCapacitacionesAdmin(Guid docenteId)
    {
        try
        {
            var htmlContent = await _reporteService.GenerarVistaCapacitacionesAsync(docenteId);
            return Ok(htmlContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar vista de capacitaciones para administrador");
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("vista/obras-academicas")]
    public async Task<ActionResult<string>> GenerarVistaObrasAcademicas()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var htmlContent = await _reporteService.GenerarVistaObrasAcademicasAsync(docente.Id);
            return Ok(htmlContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar vista de obras académicas");
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("vista/obras-academicas/{docenteId}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<string>> GenerarVistaObrasAcademicasAdmin(Guid docenteId)
    {
        try
        {
            var htmlContent = await _reporteService.GenerarVistaObrasAcademicasAsync(docenteId);
            return Ok(htmlContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar vista de obras académicas para administrador");
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("vista/reporte-completo")]
    public async Task<ActionResult<string>> GenerarVistaReporteCompleto()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var htmlContent = await _reporteService.GenerarVistaReporteCompletoAsync(docente.Id);
            return Ok(htmlContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar vista de reporte completo");
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("vista/reporte-completo/{docenteId}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<string>> GenerarVistaReporteCompletoAdmin(Guid docenteId)
    {
        try
        {
            var htmlContent = await _reporteService.GenerarVistaReporteCompletoAsync(docenteId);
            return Ok(htmlContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar vista de reporte completo para administrador");
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("vista/certificado-estado")]
    public async Task<ActionResult<string>> GenerarVistaCertificadoEstado()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var htmlContent = await _reporteService.GenerarVistaCertificadoEstadoAsync(docente.Id);
            return Ok(htmlContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar vista de certificado de estado");
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("vista/certificado-estado/{docenteId}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<string>> GenerarVistaCertificadoEstadoAdmin(Guid docenteId)
    {
        try
        {
            var htmlContent = await _reporteService.GenerarVistaCertificadoEstadoAsync(docenteId);
            return Ok(htmlContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar vista de certificado de estado para administrador");
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
