using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Application.DTOs.Solicitudes;
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

    public SolicitudesController(ISolicitudService solicitudService, IDocenteService docenteService)
    {
        _solicitudService = solicitudService;
        _docenteService = docenteService;
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
                return NotFound("Solicitud no encontrada");

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
}
