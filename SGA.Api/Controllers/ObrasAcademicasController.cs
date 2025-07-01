using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Application.DTOs;
using SGA.Application.Interfaces;
using System.Security.Claims;

namespace SGA.Api.Controllers;

[ApiController]
[Route("api/obraacademicas")]
[Authorize]
public class ObrasAcademicasController : ControllerBase
{
    private readonly IObrasAcademicasService _obrasService;
    private readonly ILogger<ObrasAcademicasController> _logger;

    public ObrasAcademicasController(IObrasAcademicasService obrasService, ILogger<ObrasAcademicasController> logger)
    {
        _obrasService = obrasService;
        _logger = logger;
    }

    [HttpGet("mis-obras")]
    public async Task<ActionResult<ResponseObrasAcademicasDto>> GetMisObras()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            // Intentar obtener la cédula del claim primero
            var cedula = User.FindFirst("cedula")?.Value;
            
            // Si no está en los claims, buscar por email en la base de datos
            if (string.IsNullOrEmpty(cedula))
            {
                var docenteService = HttpContext.RequestServices.GetRequiredService<Application.Interfaces.IDocenteService>();
                var docente = await docenteService.GetDocenteByEmailAsync(email);
                
                if (docente == null)
                {
                    return BadRequest("No se pudo encontrar el docente asociado con este usuario");
                }
                
                cedula = docente.Cedula;
            }

            var response = await _obrasService.GetObrasDocenteAsync(cedula);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("solicitar-nuevas")]
    public async Task<ActionResult<ResponseObrasAcademicasDto>> SolicitarNuevasObras([FromBody] SolicitudObrasAcademicasDto solicitud)
    {
        try
        {
            _logger.LogInformation("Iniciando solicitud de nuevas obras académicas");
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo de solicitud inválido");
                return BadRequest(ModelState);
            }

            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("No se encontró email en los claims del usuario");
                return Unauthorized();
            }

            _logger.LogInformation($"Usuario identificado: {email}");

            // Intentar obtener la cédula del claim primero
            var cedula = User.FindFirst("cedula")?.Value;
            
            // Si no está en los claims, buscar por email en la base de datos
            if (string.IsNullOrEmpty(cedula))
            {
                _logger.LogInformation("Cédula no encontrada en claims, buscando por email en base de datos");
                var docenteService = HttpContext.RequestServices.GetRequiredService<Application.Interfaces.IDocenteService>();
                var docente = await docenteService.GetDocenteByEmailAsync(email);
                
                if (docente == null)
                {
                    _logger.LogWarning($"No se encontró docente para email: {email}");
                    return BadRequest("No se pudo encontrar el docente asociado con este usuario");
                }
                
                cedula = docente.Cedula;
                _logger.LogInformation($"Cédula obtenida de base de datos: {cedula}");
            }
            else
            {
                _logger.LogInformation($"Cédula obtenida de claims: {cedula}");
            }

            _logger.LogInformation($"Enviando solicitud de {solicitud.Obras.Count} obras al servicio");
            var response = await _obrasService.SolicitarNuevasObrasAsync(cedula, solicitud);
            
            _logger.LogInformation($"Respuesta del servicio: Exitoso={response.Exitoso}, Mensaje='{response.Mensaje}'");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar solicitud de obras académicas");
            
            // Log más detallado incluyendo la excepción interna
            var innerMessage = ex.InnerException?.Message ?? "No hay excepción interna";
            var stackTrace = ex.StackTrace ?? "No hay stack trace";
            
            _logger.LogError("Detalles del error - Mensaje: {Mensaje}, Inner Exception: {InnerMessage}, Stack Trace: {StackTrace}", 
                ex.Message, innerMessage, stackTrace);
            
            return StatusCode(500, new { 
                message = ex.Message,
                innerException = innerMessage,
                stackTrace = stackTrace
            });
        }
    }

    [HttpGet("solicitudes-pendientes")]
    public async Task<ActionResult<ResponseObrasAcademicasDto>> GetSolicitudesPendientes()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            // Intentar obtener la cédula del claim primero
            var cedula = User.FindFirst("cedula")?.Value;
            
            // Si no está en los claims, buscar por email en la base de datos
            if (string.IsNullOrEmpty(cedula))
            {
                var docenteService = HttpContext.RequestServices.GetRequiredService<Application.Interfaces.IDocenteService>();
                var docente = await docenteService.GetDocenteByEmailAsync(email);
                
                if (docente == null)
                {
                    return BadRequest("No se pudo encontrar el docente asociado con este usuario");
                }
                
                cedula = docente.Cedula;
            }

            var response = await _obrasService.GetSolicitudesPendientesAsync(cedula);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("importar-externo")]
    public async Task<ActionResult> ImportarDesdeExterno()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var cedula = User.FindFirst("cedula")?.Value;  
            if (string.IsNullOrEmpty(cedula))
            {
                return BadRequest("No se pudo obtener la cédula del docente");
            }

            var response = await _obrasService.ImportarObrasDesdeExternoAsync(cedula);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("mis-solicitudes")]
    public async Task<ActionResult<ResponseObrasAcademicasDto>> GetMisSolicitudes()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            // Intentar obtener la cédula del claim primero
            var cedula = User.FindFirst("cedula")?.Value;
            
            // Si no está en los claims, buscar por email en la base de datos
            if (string.IsNullOrEmpty(cedula))
            {
                var docenteService = HttpContext.RequestServices.GetRequiredService<Application.Interfaces.IDocenteService>();
                var docente = await docenteService.GetDocenteByEmailAsync(email);
                
                if (docente == null)
                {
                    return BadRequest("No se pudo encontrar el docente asociado con este usuario");
                }
                
                cedula = docente.Cedula;
            }

            var response = await _obrasService.GetTodasSolicitudesDocenteAsync(cedula);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // Endpoints para administradores
    [HttpGet("admin/solicitudes-por-revisar")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ResponseObrasAcademicasDto>> GetSolicitudesPorRevisar()
    {
        try
        {
            var response = await _obrasService.GetSolicitudesPorRevisarAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("admin/aprobar/{solicitudId}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ResponseObrasAcademicasDto>> AprobarSolicitud(Guid solicitudId, [FromBody] string comentarios = "")
    {
        try
        {
            var response = await _obrasService.AprobarSolicitudAsync(solicitudId, comentarios);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("admin/rechazar/{solicitudId}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ResponseObrasAcademicasDto>> RechazarSolicitud(Guid solicitudId, [FromBody] string motivo)
    {
        try
        {
            if (string.IsNullOrEmpty(motivo))
                return BadRequest("El motivo de rechazo es requerido");

            var response = await _obrasService.RechazarSolicitudAsync(solicitudId, motivo);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("admin/solicitudes")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ResponseSolicitudesAdminDto>> GetSolicitudesAdmin()
    {
        try
        {
            var response = await _obrasService.GetTodasSolicitudesAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("admin/revisar")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ResponseGenericoDto>> RevisarSolicitud([FromBody] RevisionSolicitudDto revision)
    {
        try
        {
            var adminEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(adminEmail))
                return Unauthorized();

            var response = await _obrasService.RevisarSolicitudAsync(revision, adminEmail);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("admin/descargar-archivo/{solicitudId}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult> DescargarArchivoAdmin(Guid solicitudId, [FromQuery] string token)
    {
        try
        {
            var archivo = await _obrasService.GetArchivoSolicitudAsync(solicitudId);
            if (archivo == null)
                return NotFound();

            return File(archivo, "application/pdf", $"obra-admin-{solicitudId}.pdf");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
