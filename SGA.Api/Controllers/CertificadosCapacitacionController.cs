using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Application.DTOs;
using SGA.Application.Interfaces;
using System.Security.Claims;

namespace SGA.Api.Controllers;

[ApiController]
[Route("api/certificados-capacitacion")]
[Authorize]
public class CertificadosCapacitacionController : ControllerBase
{
    private readonly ICertificadosCapacitacionService _certificadosService;
    private readonly ILogger<CertificadosCapacitacionController> _logger;

    public CertificadosCapacitacionController(
        ICertificadosCapacitacionService certificadosService,
        ILogger<CertificadosCapacitacionController> logger)
    {
        _certificadosService = certificadosService;
        _logger = logger;
    }

    private string GetCedulaFromClaims()
    {
        return User.FindFirst("cedula")?.Value ?? string.Empty;
    }

    private bool IsAdmin()
    {
        return User.IsInRole("Administrador");
    }

    // Endpoints para docentes
    [HttpGet("mis-certificados")]
    public async Task<ActionResult<ResponseCertificadosCapacitacionDto>> GetMisCertificados()
    {
        try
        {
            var cedula = GetCedulaFromClaims();
            if (string.IsNullOrEmpty(cedula))
            {
                return Unauthorized("No se pudo obtener la cédula del usuario");
            }

            var response = await _certificadosService.GetCertificadosDocenteAsync(cedula);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener certificados del docente");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    [HttpPost("solicitar")]
    public async Task<ActionResult<ResponseCertificadosCapacitacionDto>> SolicitarCertificados([FromBody] SolicitarCertificadosCapacitacionDto solicitud)
    {
        try
        {
            var cedula = GetCedulaFromClaims();
            if (string.IsNullOrEmpty(cedula))
            {
                return Unauthorized("No se pudo obtener la cédula del usuario");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _certificadosService.SolicitarNuevosCertificadosAsync(cedula, solicitud);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al solicitar certificados");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    [HttpGet("mis-solicitudes")]
    public async Task<ActionResult<ResponseCertificadosCapacitacionDto>> GetMisSolicitudes()
    {
        try
        {
            var cedula = GetCedulaFromClaims();
            if (string.IsNullOrEmpty(cedula))
            {
                return Unauthorized("No se pudo obtener la cédula del usuario");
            }

            var response = await _certificadosService.GetTodasSolicitudesDocenteAsync(cedula);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener solicitudes del docente");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    [HttpGet("pendientes")]
    public async Task<ActionResult<ResponseCertificadosCapacitacionDto>> GetSolicitudesPendientes()
    {
        try
        {
            var cedula = GetCedulaFromClaims();
            if (string.IsNullOrEmpty(cedula))
            {
                return Unauthorized("No se pudo obtener la cédula del usuario");
            }

            var response = await _certificadosService.GetSolicitudesPendientesAsync(cedula);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener solicitudes pendientes");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    // Endpoints para gestión de documentos del usuario
    [HttpDelete("eliminar/{solicitudId}")]
    public async Task<ActionResult<ResponseGenericoCertificadoDto>> EliminarSolicitud(Guid solicitudId)
    {
        try
        {
            var cedula = GetCedulaFromClaims();
            if (string.IsNullOrEmpty(cedula))
            {
                return Unauthorized("No se pudo obtener la cédula del usuario");
            }

            var response = await _certificadosService.EliminarSolicitudCertificadoAsync(solicitudId, cedula);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar solicitud de certificado {SolicitudId}", solicitudId);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    [HttpPut("editar-metadatos/{solicitudId}")]
    public async Task<ActionResult<ResponseGenericoCertificadoDto>> EditarMetadatos(Guid solicitudId, [FromBody] EditarMetadatosCertificadoDto metadatos)
    {
        try
        {
            var cedula = GetCedulaFromClaims();
            if (string.IsNullOrEmpty(cedula))
            {
                return Unauthorized("No se pudo obtener la cédula del usuario");
            }

            var response = await _certificadosService.EditarMetadatosCertificadoAsync(solicitudId, cedula, metadatos);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al editar metadatos de certificado {SolicitudId}", solicitudId);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    [HttpPut("reemplazar-archivo/{solicitudId}")]
    public async Task<ActionResult<ResponseGenericoCertificadoDto>> ReemplazarArchivo(Guid solicitudId, [FromBody] ReemplazarArchivoCertificadoDto archivo)
    {
        try
        {
            var cedula = GetCedulaFromClaims();
            if (string.IsNullOrEmpty(cedula))
            {
                return Unauthorized("No se pudo obtener la cédula del usuario");
            }

            var response = await _certificadosService.ReemplazarArchivoCertificadoAsync(solicitudId, cedula, archivo);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al reemplazar archivo de certificado {SolicitudId}", solicitudId);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    [HttpPost("agregar-comentario/{solicitudId}")]
    public async Task<ActionResult<ResponseGenericoCertificadoDto>> AgregarComentario(Guid solicitudId, [FromBody] string comentario)
    {
        try
        {
            var cedula = GetCedulaFromClaims();
            if (string.IsNullOrEmpty(cedula))
            {
                return Unauthorized("No se pudo obtener la cédula del usuario");
            }

            var response = await _certificadosService.AgregarComentarioCertificadoAsync(solicitudId, cedula, comentario);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al agregar comentario a certificado {SolicitudId}", solicitudId);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    [HttpPut("reenviar/{solicitudId}")]
    public async Task<ActionResult<ResponseGenericoCertificadoDto>> ReenviarSolicitud(Guid solicitudId)
    {
        try
        {
            var cedula = GetCedulaFromClaims();
            if (string.IsNullOrEmpty(cedula))
            {
                return Unauthorized("No se pudo obtener la cédula del usuario");
            }

            var response = await _certificadosService.ReenviarSolicitudCertificadoAsync(solicitudId, cedula);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al reenviar solicitud de certificado {SolicitudId}", solicitudId);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    [HttpGet("visualizar-archivo/{solicitudId}")]
    public async Task<IActionResult> VisualizarArchivo(Guid solicitudId)
    {
        try
        {
            var cedula = GetCedulaFromClaims();
            if (string.IsNullOrEmpty(cedula))
            {
                return Unauthorized("No se pudo obtener la cédula del usuario");
            }

            var archivo = await _certificadosService.VisualizarArchivoCertificadoAsync(solicitudId, cedula);
            if (archivo == null)
            {
                return NotFound("Archivo no encontrado");
            }

            return File(archivo, "application/pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al visualizar archivo de certificado {SolicitudId}", solicitudId);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    [HttpGet("descargar-archivo/{solicitudId}")]
    public async Task<IActionResult> DescargarArchivo(Guid solicitudId)
    {
        try
        {
            var archivo = await _certificadosService.DescargarArchivoCertificadoAsync(solicitudId);
            if (archivo == null)
            {
                return NotFound("Archivo no encontrado o no aprobado");
            }

            return File(archivo, "application/pdf", $"certificado_{solicitudId}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al descargar archivo de certificado {SolicitudId}", solicitudId);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    // Endpoints para administradores
    [HttpGet("admin/todas-solicitudes")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ResponseSolicitudesCertificadosAdminDto>> GetTodasSolicitudesAdmin()
    {
        try
        {
            var response = await _certificadosService.GetTodasSolicitudesCertificadosAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las solicitudes de certificados (admin)");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    [HttpGet("admin/por-revisar")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ResponseSolicitudesCertificadosAdminDto>> GetSolicitudesPorRevisar()
    {
        try
        {
            var response = await _certificadosService.GetSolicitudesCertificadosPorRevisarAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener solicitudes por revisar");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    [HttpPost("admin/aprobar/{solicitudId}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ResponseCertificadosCapacitacionDto>> AprobarSolicitud(Guid solicitudId, [FromBody] string comentarios)
    {
        try
        {
            var response = await _certificadosService.AprobarSolicitudCertificadoAsync(solicitudId, comentarios ?? "");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al aprobar solicitud de certificado {SolicitudId}", solicitudId);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    [HttpPost("admin/rechazar/{solicitudId}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ResponseCertificadosCapacitacionDto>> RechazarSolicitud(Guid solicitudId, [FromBody] string motivo)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(motivo))
            {
                return BadRequest("El motivo del rechazo es requerido");
            }

            var response = await _certificadosService.RechazarSolicitudCertificadoAsync(solicitudId, motivo);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al rechazar solicitud de certificado {SolicitudId}", solicitudId);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    [HttpPost("admin/revisar/{solicitudId}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult> RevisarSolicitud(Guid solicitudId, [FromBody] RevisionSolicitudCertificadoDto revision)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adminEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(adminEmail))
                return Unauthorized();

            var response = await _certificadosService.RevisarSolicitudCertificadoAsync(revision, adminEmail);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al revisar solicitud de certificado {SolicitudId}", solicitudId);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    [HttpGet("admin/descargar/{solicitudId}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult> DescargarArchivoAdmin(Guid solicitudId)
    {
        try
        {
            var archivo = await _certificadosService.GetArchivoCertificadoSolicitudAsync(solicitudId);
            if (archivo == null)
            {
                return NotFound("Archivo no encontrado");
            }

            return File(archivo, "application/pdf", $"certificado_{solicitudId}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al descargar archivo admin para solicitud {SolicitudId}", solicitudId);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    [HttpGet("admin/ver/{solicitudId}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult> VisualizarArchivoAdmin(Guid solicitudId)
    {
        try
        {
            var archivo = await _certificadosService.GetArchivoCertificadoSolicitudAsync(solicitudId);
            if (archivo == null)
            {
                return NotFound("Archivo no encontrado");
            }

            return File(archivo, "application/pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al visualizar archivo admin para solicitud {SolicitudId}", solicitudId);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }
}
