using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Application.DTOs;
using SGA.Application.Interfaces;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

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
    public async Task<ActionResult> DescargarArchivoAdmin(Guid solicitudId)
    {
        try
        {
            var archivo = await _obrasService.GetArchivoSolicitudAsync(solicitudId);
            if (archivo == null)
                return NotFound("Archivo no encontrado");

            return File(archivo, "application/pdf", $"obra-{solicitudId}.pdf");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("admin/visualizar-archivo/{solicitudId}")]
    [AllowAnonymous] // Permitir acceso anónimo ya que validaremos el token manualmente
    public async Task<ActionResult> VisualizarArchivoAdmin(Guid solicitudId, [FromQuery] string token)
    {
        try
        {
            _logger.LogInformation("Intentando visualizar archivo para solicitud {SolicitudId} con token", solicitudId);
            
            // Validar el token JWT manualmente
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token no proporcionado para solicitud {SolicitudId}", solicitudId);
                return Unauthorized("Token requerido");
            }

            // Limpiar el token (remover comillas si las tiene)
            token = token.Trim('"');
            
            // Validar el token JWT
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            try
            {
                var jsonToken = tokenHandler.ReadJwtToken(token);
                var expClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == "exp");
                
                if (expClaim != null)
                {
                    var exp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim.Value));
                    if (exp < DateTimeOffset.UtcNow)
                    {
                        _logger.LogWarning("Token expirado para solicitud {SolicitudId}", solicitudId);
                        return Unauthorized("Token expirado");
                    }
                }

                // Verificar que el usuario tiene rol de administrador
                var roleClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
                if (roleClaim?.Value != "Administrador")
                {
                    _logger.LogWarning("Usuario sin permisos de administrador intentando acceder a solicitud {SolicitudId}", solicitudId);
                    return Forbid("Acceso denegado");
                }
            }
            catch (Exception tokenEx)
            {
                _logger.LogError(tokenEx, "Error al validar token para solicitud {SolicitudId}", solicitudId);
                return Unauthorized("Token inválido");
            }
            
            var archivo = await _obrasService.GetArchivoSolicitudAsync(solicitudId);
            if (archivo == null)
            {
                _logger.LogWarning("Archivo no encontrado para solicitud {SolicitudId}", solicitudId);
                return NotFound("Archivo no encontrado");
            }

            _logger.LogInformation("Archivo encontrado, tamaño: {Size} bytes", archivo.Length);
            
            Response.Headers["Content-Disposition"] = "inline";
            Response.Headers["Content-Type"] = "application/pdf";
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
            
            return File(archivo, "application/pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al visualizar archivo de solicitud {SolicitudId}", solicitudId);
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpDelete("eliminar/{solicitudId}")]
    public async Task<ActionResult<ResponseGenericoDto>> EliminarSolicitud(Guid solicitudId)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var cedula = User.FindFirst("cedula")?.Value;
            if (string.IsNullOrEmpty(cedula))
            {
                var docenteService = HttpContext.RequestServices.GetRequiredService<Application.Interfaces.IDocenteService>();
                var docente = await docenteService.GetDocenteByEmailAsync(email);
                if (docente == null)
                    return BadRequest("No se pudo encontrar el docente asociado con este usuario");
                cedula = docente.Cedula;
            }

            var response = await _obrasService.EliminarSolicitudAsync(solicitudId, cedula);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPut("editar-metadatos/{solicitudId}")]
    public async Task<ActionResult<ResponseGenericoDto>> EditarMetadatos(Guid solicitudId, [FromBody] EditarMetadatosSolicitudDto metadatos)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var cedula = User.FindFirst("cedula")?.Value;
            if (string.IsNullOrEmpty(cedula))
            {
                var docenteService = HttpContext.RequestServices.GetRequiredService<Application.Interfaces.IDocenteService>();
                var docente = await docenteService.GetDocenteByEmailAsync(email);
                if (docente == null)
                    return BadRequest("No se pudo encontrar el docente asociado con este usuario");
                cedula = docente.Cedula;
            }

            var response = await _obrasService.EditarMetadatosSolicitudAsync(solicitudId, cedula, metadatos);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPut("reemplazar-archivo/{solicitudId}")]
    public async Task<ActionResult<ResponseGenericoDto>> ReemplazarArchivo(Guid solicitudId, [FromBody] ReemplazarArchivoDto archivo)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var cedula = User.FindFirst("cedula")?.Value;
            if (string.IsNullOrEmpty(cedula))
            {
                var docenteService = HttpContext.RequestServices.GetRequiredService<Application.Interfaces.IDocenteService>();
                var docente = await docenteService.GetDocenteByEmailAsync(email);
                if (docente == null)
                    return BadRequest("No se pudo encontrar el docente asociado con este usuario");
                cedula = docente.Cedula;
            }

            var response = await _obrasService.ReemplazarArchivoSolicitudAsync(solicitudId, cedula, archivo);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("agregar-comentario/{solicitudId}")]
    public async Task<ActionResult<ResponseGenericoDto>> AgregarComentario(Guid solicitudId, [FromBody] string comentario)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var cedula = User.FindFirst("cedula")?.Value;
            if (string.IsNullOrEmpty(cedula))
            {
                var docenteService = HttpContext.RequestServices.GetRequiredService<Application.Interfaces.IDocenteService>();
                var docente = await docenteService.GetDocenteByEmailAsync(email);
                if (docente == null)
                    return BadRequest("No se pudo encontrar el docente asociado con este usuario");
                cedula = docente.Cedula;
            }

            if (string.IsNullOrWhiteSpace(comentario))
                return BadRequest("El comentario no puede estar vacío");

            var response = await _obrasService.AgregarComentarioSolicitudAsync(solicitudId, cedula, comentario);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPut("reenviar/{solicitudId}")]
    public async Task<ActionResult<ResponseGenericoDto>> ReenviarSolicitud(Guid solicitudId)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var cedula = User.FindFirst("cedula")?.Value;
            if (string.IsNullOrEmpty(cedula))
            {
                var docenteService = HttpContext.RequestServices.GetRequiredService<Application.Interfaces.IDocenteService>();
                var docente = await docenteService.GetDocenteByEmailAsync(email);
                if (docente == null)
                    return BadRequest("No se pudo encontrar el docente asociado con este usuario");
                cedula = docente.Cedula;
            }

            var response = await _obrasService.ReenviarSolicitudAsync(solicitudId, cedula);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("visualizar-archivo/{solicitudId}")]
    public async Task<ActionResult> VisualizarArchivo(Guid solicitudId)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var cedula = User.FindFirst("cedula")?.Value;
            if (string.IsNullOrEmpty(cedula))
            {
                var docenteService = HttpContext.RequestServices.GetRequiredService<Application.Interfaces.IDocenteService>();
                var docente = await docenteService.GetDocenteByEmailAsync(email);
                if (docente == null)
                    return BadRequest("No se pudo encontrar el docente asociado con este usuario");
                cedula = docente.Cedula;
            }

            var archivo = await _obrasService.VisualizarArchivoSolicitudAsync(solicitudId, cedula);
            if (archivo == null)
                return NotFound("Archivo no encontrado");

            Response.Headers["Content-Disposition"] = "inline";
            Response.Headers["Content-Type"] = "application/pdf";
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return File(archivo, "application/pdf");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("descargar-archivo/{solicitudId}")]
    public async Task<ActionResult> DescargarArchivo(Guid solicitudId)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var cedula = User.FindFirst("cedula")?.Value;
            if (string.IsNullOrEmpty(cedula))
            {
                var docenteService = HttpContext.RequestServices.GetRequiredService<Application.Interfaces.IDocenteService>();
                var docente = await docenteService.GetDocenteByEmailAsync(email);
                if (docente == null)
                    return BadRequest("No se pudo encontrar el docente asociado con este usuario");
                cedula = docente.Cedula;
            }

            var archivo = await _obrasService.VisualizarArchivoSolicitudAsync(solicitudId, cedula);
            if (archivo == null)
                return NotFound("Archivo no encontrado");

            return File(archivo, "application/pdf", $"documento-{solicitudId}.pdf");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
