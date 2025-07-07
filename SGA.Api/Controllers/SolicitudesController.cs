using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Application.DTOs.Solicitudes;
using SGA.Application.DTOs.Documentos;
using SGA.Application.Interfaces;
using System.Security.Claims;

namespace SGA.Api.Controllers;

[ApiController]
[Route("api/solicitudascenso")]
[Authorize]
public class SolicitudesController : ControllerBase
{
    private readonly ISolicitudService _solicitudService;
    private readonly IDocenteService _docenteService;
    private readonly IDocumentoUtilizacionService _documentoUtilizacionService;

    public SolicitudesController(ISolicitudService solicitudService, IDocenteService docenteService, IDocumentoUtilizacionService documentoUtilizacionService)
    {
        _solicitudService = solicitudService;
        _docenteService = docenteService;
        _documentoUtilizacionService = documentoUtilizacionService;
    }

    [HttpPost]
    public async Task<ActionResult<SolicitudAscensoDto>> CrearSolicitud([FromBody] CrearSolicitudRequest request)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var solicitud = await _solicitudService.CrearSolicitudAsync(docente.Id, request);
            return Ok(solicitud);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("mis-solicitudes")]
    public async Task<ActionResult<List<SolicitudAscensoDto>>> GetMisSolicitudes()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var solicitudes = await _solicitudService.GetSolicitudesByDocenteAsync(docente.Id);
            
            return Ok(solicitudes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<List<SolicitudAscensoDto>>> GetTodasLasSolicitudes()
    {
        try
        {
            var solicitudes = await _solicitudService.GetTodasLasSolicitudesAsync();
            return Ok(solicitudes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SolicitudAscensoDto>> GetSolicitud(Guid id)
    {
        try
        {
            var solicitud = await _solicitudService.GetSolicitudByIdAsync(id);
            if (solicitud == null)
            {
                return NotFound("Solicitud no encontrada");
            }

            // Verificar permisos: solo el docente propietario o administrador pueden ver la solicitud
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "Administrador")
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var docente = await _docenteService.GetDocenteByEmailAsync(email!);
                if (docente == null || docente.Id != solicitud.DocenteId)
                    return Forbid();
            }

            return Ok(solicitud);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("{id}/procesar")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult> ProcesarSolicitud(Guid id, [FromBody] ProcesarSolicitudRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var adminId))
                return Unauthorized();

            request.SolicitudId = id;
            var resultado = await _solicitudService.ProcesarSolicitudAsync(id, request, adminId);
            
            if (!resultado)
                return BadRequest("No se pudo procesar la solicitud");

            return Ok(new { message = request.Aprobar ? "Solicitud aprobada" : "Solicitud rechazada" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("tiene-solicitud-activa")]
    public async Task<ActionResult<bool>> TieneSolicitudActiva()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var tieneSolicitudActiva = await _solicitudService.TieneDocumenteSolicitudActivaAsync(docente.Id);
            return Ok(tieneSolicitudActiva);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("documento/{documentoId}")]
    public async Task<ActionResult> DescargarDocumento(Guid documentoId)
    {
        try
        {
            var contenido = await _solicitudService.DescargarDocumentoAsync(documentoId);
            if (contenido == null)
                return NotFound("Documento no encontrado");

            return File(contenido, "application/pdf");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> CancelarSolicitud(Guid id)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var solicitud = await _solicitudService.GetSolicitudByIdAsync(id);
            if (solicitud == null)
                return NotFound("Solicitud no encontrada");

            // Verificar que la solicitud pertenece al docente
            if (solicitud.DocenteId != docente.Id)
                return Forbid("No tiene permisos para cancelar esta solicitud");

            // Solo se pueden cancelar solicitudes en estado Pendiente
            if (solicitud.Estado != "Pendiente")
                return BadRequest("Solo se pueden cancelar solicitudes en estado Pendiente");

            // Cancelar la solicitud directamente usando el nuevo método
            var resultado = await _solicitudService.CancelarSolicitudAsync(id, docente.Id);
            
            if (!resultado)
                return BadRequest("No se pudo cancelar la solicitud");

            return Ok(new { message = "Solicitud cancelada exitosamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("{id}/documentos")]
    public async Task<ActionResult<List<DocumentoDto>>> GetDocumentosSolicitud(Guid id)
    {
        try
        {
            var solicitud = await _solicitudService.GetSolicitudByIdAsync(id);
            if (solicitud == null)
            {
                return NotFound("Solicitud no encontrada");
            }

            // Verificar permisos: solo el docente propietario o administrador pueden ver los documentos
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "Administrador")
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var docente = await _docenteService.GetDocenteByEmailAsync(email!);
                if (docente == null || docente.Id != solicitud.DocenteId)
                {
                    return Forbid();
                }
            }

            return Ok(solicitud.Documentos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPut("{id}/reenviar")]
    public async Task<ActionResult> ReenviarSolicitud(Guid id)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var solicitud = await _solicitudService.GetSolicitudByIdAsync(id);
            if (solicitud == null)
                return NotFound("Solicitud no encontrada");

            // Verificar que la solicitud pertenece al docente
            if (solicitud.DocenteId != docente.Id)
                return Forbid("No tiene permisos para reenviar esta solicitud");

            // Solo se pueden reenviar solicitudes rechazadas
            if (solicitud.Estado != "Rechazada")
                return BadRequest("Solo se pueden reenviar solicitudes rechazadas");

            var resultado = await _solicitudService.ReenviarSolicitudAsync(id, docente.Id);
            
            if (!resultado)
                return BadRequest("No se pudo reenviar la solicitud");

            return Ok(new { message = "Solicitud reenviada exitosamente para nueva revisión" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPut("{id}/reenviar-con-documentos")]
    public async Task<ActionResult> ReenviarSolicitudConDocumentos(Guid id, [FromBody] ReenviarConDocumentosRequest request)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var solicitud = await _solicitudService.GetSolicitudByIdAsync(id);
            if (solicitud == null)
                return NotFound("Solicitud no encontrada");

            // Verificar que la solicitud pertenece al docente
            if (solicitud.DocenteId != docente.Id)
                return Forbid("No tiene permisos para reenviar esta solicitud");

            // Solo se pueden reenviar solicitudes rechazadas
            if (solicitud.Estado != "Rechazada")
                return BadRequest("Solo se pueden reenviar solicitudes rechazadas");

            var resultado = await _solicitudService.ReenviarSolicitudConDocumentosAsync(id, docente.Id, request.DocumentosSeleccionados);
            
            if (!resultado)
                return BadRequest("No se pudo reenviar la solicitud con los documentos seleccionados");

            return Ok(new { message = "Solicitud reenviada exitosamente con documentos actualizados" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("{id}/debug-documentos")]
    public async Task<ActionResult> DebugDocumentosSolicitud(Guid id)
    {
        try
        {
            // Verificar si la solicitud existe
            var solicitud = await _solicitudService.GetSolicitudByIdAsync(id);
            if (solicitud == null)
            {
                return NotFound(new { message = "Solicitud no encontrada", solicitudId = id });
            }

            // Verificar permisos básicos
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            
            var debugInfo = new
            {
                SolicitudId = id,
                SolicitudExiste = solicitud != null,
                DocenteId = solicitud?.DocenteId,
                UserRole = userRole,
                UserEmail = userEmail,
                DocumentosCount = solicitud?.Documentos?.Count ?? 0,
                Documentos = solicitud?.Documentos?.Select(d => new {
                    d.Id,
                    d.NombreArchivo,
                    d.TipoDocumento,
                    d.TamanoArchivo,
                    d.FechaCreacion
                }) ?? Enumerable.Empty<object>(),
                Timestamp = DateTime.Now
            };
            
            return Ok(debugInfo);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    /// <summary>
    /// Obtiene los documentos disponibles (no utilizados en solicitudes aprobadas) para el docente actual
    /// </summary>
    [HttpGet("documentos-disponibles")]
    public async Task<ActionResult<List<DocumentoDto>>> GetDocumentosDisponibles()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            // Obtener documentos disponibles usando el servicio
            var documentosDisponibles = await _documentoUtilizacionService.ObtenerDocumentosDisponiblesAsync(docente.Id);

            // Convertir a DTOs
            var documentosDto = documentosDisponibles.Select(d => new DocumentoDto
            {
                Id = d.Id,
                NombreArchivo = d.NombreArchivo,
                Nombre = d.NombreArchivo,
                TamanoArchivo = d.TamanoArchivo,
                TipoDocumento = d.TipoDocumento.ToString(),
                FechaCreacion = d.FechaCreacion
            }).ToList();

            return Ok(documentosDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Verifica el estado de un documento específico (disponible o ya utilizado)
    /// </summary>
    [HttpGet("documento-estado/{documentoId}")]
    public async Task<ActionResult<object>> GetEstadoDocumento(Guid documentoId)
    {
        try
        {
            var (estaDisponible, motivoNoDisponible, fechaUtilizacion) = 
                await _documentoUtilizacionService.ObtenerEstadoDocumentoAsync(documentoId);

            return Ok(new 
            {
                EstaDisponible = estaDisponible,
                MotivoNoDisponible = motivoNoDisponible,
                FechaUtilizacion = fechaUtilizacion
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Verifica si una obra académica está disponible para usar en una nueva solicitud
    /// </summary>
    [HttpGet("verificar-disponibilidad-obra/{solicitudId}")]
    public async Task<ActionResult<object>> VerificarDisponibilidadObra(Guid solicitudId)
    {
        try
        {
            // Verificar que el usuario tenga permisos para esta verificación
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var (estaDisponible, motivoNoDisponible, fechaUtilizacion) = 
                await _documentoUtilizacionService.VerificarDisponibilidadObraAsync(solicitudId);

            return Ok(new 
            {
                EstaDisponible = estaDisponible,
                MotivoNoDisponible = motivoNoDisponible,
                FechaUtilizacion = fechaUtilizacion
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Verifica si un certificado de capacitación está disponible para usar en una nueva solicitud
    /// </summary>
    [HttpGet("verificar-disponibilidad-certificado/{certificadoId}")]
    public async Task<ActionResult<object>> VerificarDisponibilidadCertificado(Guid certificadoId)
    {
        try
        {
            // Verificar que el usuario tenga permisos para esta verificación
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var (estaDisponible, motivoNoDisponible, fechaUtilizacion) = 
                await _documentoUtilizacionService.VerificarDisponibilidadCertificadoAsync(certificadoId);

            return Ok(new 
            {
                EstaDisponible = estaDisponible,
                MotivoNoDisponible = motivoNoDisponible,
                FechaUtilizacion = fechaUtilizacion
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Verifica si una evidencia de investigación está disponible para usar en una nueva solicitud
    /// </summary>
    [HttpGet("verificar-disponibilidad-evidencia/{evidenciaId}")]
    public async Task<ActionResult<object>> VerificarDisponibilidadEvidencia(Guid evidenciaId)
    {
        try
        {
            // Verificar que el usuario tenga permisos para esta verificación
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            var (estaDisponible, motivoNoDisponible, fechaUtilizacion) = 
                await _documentoUtilizacionService.VerificarDisponibilidadEvidenciaAsync(evidenciaId);

            return Ok(new 
            {
                EstaDisponible = estaDisponible,
                MotivoNoDisponible = motivoNoDisponible,
                FechaUtilizacion = fechaUtilizacion
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
