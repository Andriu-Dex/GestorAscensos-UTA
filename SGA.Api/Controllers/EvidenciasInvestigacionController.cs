using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Application.DTOs.EvidenciasInvestigacion;
using SGA.Application.Interfaces;
using System.Security.Claims;

namespace SGA.Api.Controllers;

[ApiController]
[Route("api/evidencias-investigacion")]
[Authorize]
public class EvidenciasInvestigacionController : ControllerBase
{
    private readonly IEvidenciasInvestigacionService _evidenciasService;
    private readonly ILogger<EvidenciasInvestigacionController> _logger;

    public EvidenciasInvestigacionController(
        IEvidenciasInvestigacionService evidenciasService,
        ILogger<EvidenciasInvestigacionController> logger)
    {
        _evidenciasService = evidenciasService;
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
    [HttpGet("mis-evidencias")]
    public async Task<ActionResult<ResponseEvidenciasInvestigacionDto>> GetMisEvidencias()
    {
        try
        {
            var cedula = GetCedulaFromClaims();
            if (string.IsNullOrEmpty(cedula))
            {
                return Unauthorized("No se pudo obtener la cédula del usuario");
            }

            var response = await _evidenciasService.GetMisEvidenciasAsync(cedula);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener evidencias del docente");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    [HttpPost("solicitar")]
    public async Task<ActionResult<ResponseEvidenciasInvestigacionDto>> SolicitarEvidencias([FromBody] SolicitarEvidenciasInvestigacionDto solicitud)
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

            var response = await _evidenciasService.SolicitarNuevasEvidenciasAsync(cedula, solicitud);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al solicitar evidencias de investigación");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    // Endpoints para gestión de documentos del usuario
    [HttpDelete("eliminar/{evidenciaId}")]
    public async Task<ActionResult<ResponseGenericoEvidenciaDto>> EliminarEvidencia(Guid evidenciaId)
    {
        try
        {
            var cedula = GetCedulaFromClaims();
            if (string.IsNullOrEmpty(cedula))
            {
                return Unauthorized("No se pudo obtener la cédula del usuario");
            }

            var response = await _evidenciasService.EliminarSolicitudEvidenciaAsync(evidenciaId, cedula);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar evidencia de investigación {EvidenciaId}", evidenciaId);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    [HttpPut("editar-metadatos/{evidenciaId}")]
    public async Task<ActionResult<ResponseGenericoEvidenciaDto>> EditarMetadatos(Guid evidenciaId, [FromBody] EditarMetadatosEvidenciaDto metadatos)
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

            var response = await _evidenciasService.EditarMetadatosEvidenciaAsync(evidenciaId, cedula, metadatos);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al editar metadatos de evidencia {EvidenciaId}", evidenciaId);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    [HttpPut("reemplazar-archivo/{evidenciaId}")]
    public async Task<ActionResult<ResponseGenericoEvidenciaDto>> ReemplazarArchivo(Guid evidenciaId, [FromForm] ReemplazarArchivoEvidenciaDto archivo)
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

            var response = await _evidenciasService.ReemplazarArchivoEvidenciaAsync(evidenciaId, cedula, archivo);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al reemplazar archivo de evidencia {EvidenciaId}", evidenciaId);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    [HttpGet("visualizar-archivo/{evidenciaId}")]
    public async Task<IActionResult> VisualizarArchivo(Guid evidenciaId)
    {
        try
        {
            // Si es administrador, permite acceso directo
            if (IsAdmin())
            {
                var archivo = await _evidenciasService.GetArchivoEvidenciaSolicitudAsync(evidenciaId);
                if (archivo == null)
                {
                    return NotFound("Archivo no encontrado");
                }
                return File(archivo, "application/pdf");
            }

            // Para docentes, verificar que pertenezca al usuario
            var cedula = GetCedulaFromClaims();
            if (string.IsNullOrEmpty(cedula))
            {
                return Unauthorized("No se pudo obtener la cédula del usuario");
            }

            var archivoDocente = await _evidenciasService.GetArchivoEvidenciaAsync(evidenciaId, cedula);
            if (archivoDocente == null)
            {
                return NotFound("Archivo no encontrado");
            }

            return File(archivoDocente, "application/pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al visualizar archivo de evidencia {EvidenciaId}", evidenciaId);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    // Ruta alternativa para administradores con ID de solicitud
    [HttpGet("{evidenciaId}/archivo")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ObtenerArchivoAdmin(Guid evidenciaId)
    {
        try
        {
            var archivo = await _evidenciasService.GetArchivoEvidenciaSolicitudAsync(evidenciaId);
            if (archivo == null)
            {
                return NotFound("Archivo no encontrado");
            }

            return File(archivo, "application/pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener archivo de evidencia como admin {EvidenciaId}", evidenciaId);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    // Endpoints de administración
    [HttpGet("admin/todas")]
    [Authorize(Roles = "Administrador")]
    [Produces("application/json")]
    public async Task<ActionResult> GetTodasEvidenciasAdmin()
    {
        try
        {
            var response = await _evidenciasService.GetTodasSolicitudesEvidenciasAsync();
            
            // Mapear al formato que espera el frontend
            var frontendResponse = new
            {
                Exitoso = response.Exitoso,
                Mensaje = response.Mensaje,
                Evidencias = response.Solicitudes.Select(s => new
                {
                    Id = s.Id,
                    DocenteCedula = s.DocenteCedula,
                    DocenteNombre = s.DocenteNombre,
                    TipoEvidencia = s.TipoEvidencia,
                    TituloProyecto = s.TituloProyecto,
                    InstitucionFinanciadora = s.InstitucionFinanciadora,
                    RolInvestigador = s.RolInvestigador,
                    FechaInicio = s.FechaInicio,
                    FechaFin = s.FechaFin,
                    MesesDuracion = s.MesesDuracion,
                    CodigoProyecto = s.CodigoProyecto,
                    AreaTematica = s.AreaTematica,
                    Descripcion = s.Descripcion,
                    ArchivoNombre = s.ArchivoNombre,
                    TieneArchivo = !string.IsNullOrEmpty(s.ArchivoNombre),
                    Estado = s.Estado,
                    MotivoRechazo = s.MotivoRechazo,
                    ComentariosRevision = s.ComentariosRevision,
                    FechaCreacion = s.FechaCreacion,
                    FechaRevision = s.FechaRevision
                }).ToList(),
                TotalEvidencias = response.Solicitudes.Count
            };
            
            return Ok(frontendResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las evidencias de investigación (admin)");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    [HttpGet("admin/por-revisar")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ResponseSolicitudesEvidenciasAdminDto>> GetEvidenciasPorRevisar()
    {
        try
        {
            var response = await _evidenciasService.GetSolicitudesEvidenciasPorRevisarAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener evidencias por revisar");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    [HttpPost("admin/revisar")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult> RevisarEvidencia([FromBody] RevisionSolicitudEvidenciaDto revision)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adminEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "";
            var response = await _evidenciasService.RevisarSolicitudEvidenciaAsync(revision, adminEmail);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al revisar evidencia de investigación {EvidenciaId}", revision.SolicitudId);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    [HttpGet("admin/visualizar-archivo/{solicitudId}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> VisualizarArchivoAdmin(Guid solicitudId)
    {
        try
        {
            var archivo = await _evidenciasService.GetArchivoEvidenciaSolicitudAsync(solicitudId);
            if (archivo == null)
            {
                return NotFound("Archivo no encontrado");
            }

            return File(archivo, "application/pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al visualizar archivo de evidencia como admin {SolicitudId}", solicitudId);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }
}
