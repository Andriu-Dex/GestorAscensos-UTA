using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Application.DTOs;
using SGA.Application.Services;
using SGA.Domain.Entities;
using System.Security.Claims;

namespace SGA.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SolicitudAscensoController : ControllerBase
    {
        private readonly ISolicitudAscensoService _solicitudService;
        private readonly ILogger<SolicitudAscensoController> _logger;

        public SolicitudAscensoController(ISolicitudAscensoService solicitudService, ILogger<SolicitudAscensoController> logger)
        {
            _solicitudService = solicitudService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetMisSolicitudes()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int docenteId))
                {
                    return Unauthorized(new { message = "Token no válido" });
                }

                var solicitudes = await _solicitudService.GetSolicitudesByDocenteIdAsync(docenteId);
                var solicitudesDto = solicitudes.Select(s => new SolicitudAscensoResponseDto
                {
                    Id = s.Id,
                    DocenteId = s.DocenteId,
                    FechaSolicitud = s.FechaSolicitud,
                    NivelActual = s.NivelActual,
                    NivelSolicitado = s.NivelSolicitado,
                    EstadoSolicitudId = s.EstadoSolicitudId,
                    MotivoRechazo = s.MotivoRechazo,
                    FechaRevision = s.FechaRevision,
                    RevisorId = s.RevisorId,
                    TiempoEnRol = s.TiempoEnRol,
                    NumeroObras = s.NumeroObras,
                    PuntajeEvaluacion = s.PuntajeEvaluacion,
                    HorasCapacitacion = s.HorasCapacitacion,
                    TiempoInvestigacion = s.TiempoInvestigacion,
                    CumpleTiempo = s.CumpleTiempo,
                    CumpleObras = s.CumpleObras,
                    CumpleEvaluacion = s.CumpleEvaluacion,
                    CumpleCapacitacion = s.CumpleCapacitacion,
                    CumpleInvestigacion = s.CumpleInvestigacion,
                    ObservacionesRevisor = s.ObservacionesRevisor,
                    FechaCreacion = s.FechaCreacion,
                    FechaActualizacion = s.FechaActualizacion
                });

                return Ok(solicitudesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener solicitudes del docente: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSolicitud(int id)
        {
            try
            {
                var solicitud = await _solicitudService.GetSolicitudByIdAsync(id);
                if (solicitud == null)
                {
                    return NotFound(new { message = "Solicitud no encontrada" });
                }

                // Verificar que el usuario tiene acceso a esta solicitud
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int docenteId))
                {
                    return Unauthorized(new { message = "Token no válido" });
                }

                if (solicitud.DocenteId != docenteId && !User.IsInRole("Administrador"))
                {
                    return Forbid("No tiene permisos para ver esta solicitud");
                }

                var solicitudDto = new SolicitudAscensoResponseDto
                {
                    Id = solicitud.Id,
                    DocenteId = solicitud.DocenteId,
                    FechaSolicitud = solicitud.FechaSolicitud,
                    NivelActual = solicitud.NivelActual,
                    NivelSolicitado = solicitud.NivelSolicitado,
                    EstadoSolicitudId = solicitud.EstadoSolicitudId,
                    MotivoRechazo = solicitud.MotivoRechazo,
                    FechaRevision = solicitud.FechaRevision,
                    RevisorId = solicitud.RevisorId,
                    TiempoEnRol = solicitud.TiempoEnRol,
                    NumeroObras = solicitud.NumeroObras,
                    PuntajeEvaluacion = solicitud.PuntajeEvaluacion,
                    HorasCapacitacion = solicitud.HorasCapacitacion,
                    TiempoInvestigacion = solicitud.TiempoInvestigacion,
                    CumpleTiempo = solicitud.CumpleTiempo,
                    CumpleObras = solicitud.CumpleObras,
                    CumpleEvaluacion = solicitud.CumpleEvaluacion,
                    CumpleCapacitacion = solicitud.CumpleCapacitacion,
                    CumpleInvestigacion = solicitud.CumpleInvestigacion,
                    ObservacionesRevisor = solicitud.ObservacionesRevisor,
                    FechaCreacion = solicitud.FechaCreacion,
                    FechaActualizacion = solicitud.FechaActualizacion
                };

                return Ok(solicitudDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener solicitud con ID {id}: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CrearSolicitud([FromBody] CrearSolicitudAscensoDto crearDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int docenteId))
                {
                    return Unauthorized(new { message = "Token no válido" });
                }

                var solicitud = await _solicitudService.CrearSolicitudAscensoAsync(docenteId, crearDto.DocumentosIds);

                var solicitudDto = new SolicitudAscensoResponseDto
                {
                    Id = solicitud.Id,
                    DocenteId = solicitud.DocenteId,
                    FechaSolicitud = solicitud.FechaSolicitud,
                    NivelActual = solicitud.NivelActual,
                    NivelSolicitado = solicitud.NivelSolicitado,
                    EstadoSolicitudId = solicitud.EstadoSolicitudId,
                    TiempoEnRol = solicitud.TiempoEnRol,
                    NumeroObras = solicitud.NumeroObras,
                    PuntajeEvaluacion = solicitud.PuntajeEvaluacion,
                    HorasCapacitacion = solicitud.HorasCapacitacion,
                    TiempoInvestigacion = solicitud.TiempoInvestigacion,
                    CumpleTiempo = solicitud.CumpleTiempo,
                    CumpleObras = solicitud.CumpleObras,
                    CumpleEvaluacion = solicitud.CumpleEvaluacion,
                    CumpleCapacitacion = solicitud.CumpleCapacitacion,
                    CumpleInvestigacion = solicitud.CumpleInvestigacion,
                    FechaCreacion = solicitud.FechaCreacion,
                    FechaActualizacion = solicitud.FechaActualizacion
                };

                return CreatedAtAction(nameof(GetSolicitud), new { id = solicitud.Id }, solicitudDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al crear solicitud de ascenso: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("pendientes")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GetSolicitudesPendientes()
        {
            try
            {
                var solicitudes = await _solicitudService.GetSolicitudesPendientesAsync();
                var solicitudesDto = solicitudes.Select(s => new SolicitudAscensoResponseDto
                {
                    Id = s.Id,
                    DocenteId = s.DocenteId,
                    FechaSolicitud = s.FechaSolicitud,
                    NivelActual = s.NivelActual,
                    NivelSolicitado = s.NivelSolicitado,
                    EstadoSolicitudId = s.EstadoSolicitudId,
                    MotivoRechazo = s.MotivoRechazo,
                    FechaRevision = s.FechaRevision,
                    RevisorId = s.RevisorId,
                    TiempoEnRol = s.TiempoEnRol,
                    NumeroObras = s.NumeroObras,
                    PuntajeEvaluacion = s.PuntajeEvaluacion,
                    HorasCapacitacion = s.HorasCapacitacion,
                    TiempoInvestigacion = s.TiempoInvestigacion,
                    CumpleTiempo = s.CumpleTiempo,
                    CumpleObras = s.CumpleObras,
                    CumpleEvaluacion = s.CumpleEvaluacion,
                    CumpleCapacitacion = s.CumpleCapacitacion,
                    CumpleInvestigacion = s.CumpleInvestigacion,
                    ObservacionesRevisor = s.ObservacionesRevisor,
                    FechaCreacion = s.FechaCreacion,
                    FechaActualizacion = s.FechaActualizacion
                });

                return Ok(solicitudesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener solicitudes pendientes: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("{id}/revisar")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> RevisarSolicitud(int id, [FromBody] RevisarSolicitudDto revisarDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int revisorId))
                {
                    return Unauthorized(new { message = "Token no válido" });
                }

                await _solicitudService.RevisarSolicitudAsync(id, revisorId, revisarDto.EstadoSolicitudId, revisarDto.MotivoRechazo, revisarDto.Observaciones);

                return Ok(new { message = "Solicitud revisada exitosamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al revisar solicitud con ID {id}: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
