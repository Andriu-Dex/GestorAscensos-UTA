using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Application.Interfaces;
using SGA.Domain.Enums;
using System.Security.Claims;

namespace SGA.Api.Controllers;

/// <summary>
/// Controlador para gestionar notificaciones en tiempo real
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificacionesController : ControllerBase
{
    private readonly INotificacionTiempoRealService _notificacionService;
    private readonly ILogger<NotificacionesController> _logger;

    public NotificacionesController(
        INotificacionTiempoRealService notificacionService,
        ILogger<NotificacionesController> logger)
    {
        _notificacionService = notificacionService;
        _logger = logger;
    }

    /// <summary>
    /// Obtener notificaciones del usuario actual
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetNotificaciones([FromQuery] int limit = 20)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("Usuario no identificado");
            }

            var notificaciones = await _notificacionService.ObtenerNotificacionesUsuarioAsync(userId, limit);
            
            // Convertir a DTO para asegurar serialización correcta
            var notificacionesDto = notificaciones.Select(n => new NotificacionDto
            {
                Id = n.Id,
                Titulo = n.Titulo,
                Mensaje = n.Mensaje,
                Tipo = n.Tipo.ToString(), // Convertir enum a string
                UrlAccion = n.UrlAccion,
                FechaCreacion = n.FechaCreacion,
                Leida = n.Leida,
                Icono = ObtenerIconoPorTipo(n.Tipo),
                Color = ObtenerColorPorTipo(n.Tipo)
            }).ToList();
            
            return Ok(notificacionesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener notificaciones");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtener notificaciones no leídas del usuario actual
    /// </summary>
    [HttpGet("no-leidas")]
    public async Task<IActionResult> GetNotificacionesNoLeidas()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("Usuario no identificado");
            }

            var notificaciones = await _notificacionService.ObtenerNotificacionesNoLeidasAsync(userId);
            
            // Convertir a DTO para asegurar serialización correcta
            var notificacionesDto = notificaciones.Select(n => new NotificacionDto
            {
                Id = n.Id,
                Titulo = n.Titulo,
                Mensaje = n.Mensaje,
                Tipo = n.Tipo.ToString(), // Convertir enum a string
                UrlAccion = n.UrlAccion,
                FechaCreacion = n.FechaCreacion,
                Leida = n.Leida,
                Icono = ObtenerIconoPorTipo(n.Tipo),
                Color = ObtenerColorPorTipo(n.Tipo)
            }).ToList();
            
            return Ok(notificacionesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener notificaciones no leídas");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtener contador de notificaciones no leídas
    /// </summary>
    [HttpGet("contador-no-leidas")]
    public async Task<IActionResult> GetContadorNoLeidas()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("Usuario no identificado");
            }

            var contador = await _notificacionService.ObtenerContadorNoLeidasAsync(userId);
            return Ok(new { Contador = contador }); // Asegurar que la primera letra sea mayúscula
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener contador de notificaciones no leídas");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Marcar notificación como leída
    /// </summary>
    [HttpPut("{id}/marcar-leida")]
    public async Task<IActionResult> MarcarComoLeida(Guid id)
    {
        try
        {
            await _notificacionService.MarcarComoLeidaAsync(id);
            return Ok(new { mensaje = "Notificación marcada como leída" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al marcar notificación como leída");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Marcar todas las notificaciones como leídas
    /// </summary>
    [HttpPut("marcar-todas-leidas")]
    public async Task<IActionResult> MarcarTodasComoLeidas()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("Usuario no identificado");
            }

            await _notificacionService.MarcarTodasComoLeidasAsync(userId);
            return Ok(new { mensaje = "Todas las notificaciones marcadas como leídas" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al marcar todas las notificaciones como leídas");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Enviar notificación de prueba (solo para administradores)
    /// </summary>
    [HttpPost("prueba")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> EnviarNotificacionPrueba([FromBody] NotificacionPruebaDto notificacion)
    {
        try
        {
            if (notificacion.UsuarioId.HasValue)
            {
                await _notificacionService.EnviarNotificacionAsync(
                    notificacion.UsuarioId.Value,
                    notificacion.Titulo,
                    notificacion.Mensaje,
                    Domain.Enums.TipoNotificacion.Info,
                    notificacion.UrlAccion
                );
            }
            else
            {
                await _notificacionService.EnviarNotificacionAdministradoresAsync(
                    notificacion.Titulo,
                    notificacion.Mensaje,
                    Domain.Enums.TipoNotificacion.Info,
                    notificacion.UrlAccion
                );
            }

            return Ok(new { mensaje = "Notificación de prueba enviada" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar notificación de prueba");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    private static string ObtenerIconoPorTipo(TipoNotificacion tipo)
    {
        return tipo switch
        {
            TipoNotificacion.Exito or TipoNotificacion.SolicitudAprobada or TipoNotificacion.AscensoAprobado 
                or TipoNotificacion.CertificadoAprobado or TipoNotificacion.ObraAprobada or TipoNotificacion.EvidenciaAprobada => "bi-check-circle-fill",
            TipoNotificacion.Error or TipoNotificacion.SolicitudRechazada or TipoNotificacion.AscensoRechazado 
                or TipoNotificacion.CertificadoRechazado or TipoNotificacion.ObraRechazada or TipoNotificacion.EvidenciaRechazada => "bi-x-circle-fill",
            TipoNotificacion.Advertencia => "bi-exclamation-triangle-fill",
            TipoNotificacion.NuevaSolicitud => "bi-file-earmark-plus-fill",
            _ => "bi-info-circle-fill"
        };
    }

    private static string ObtenerColorPorTipo(TipoNotificacion tipo)
    {
        return tipo switch
        {
            TipoNotificacion.Exito or TipoNotificacion.SolicitudAprobada or TipoNotificacion.AscensoAprobado 
                or TipoNotificacion.CertificadoAprobado or TipoNotificacion.ObraAprobada or TipoNotificacion.EvidenciaAprobada => "success",
            TipoNotificacion.Error or TipoNotificacion.SolicitudRechazada or TipoNotificacion.AscensoRechazado 
                or TipoNotificacion.CertificadoRechazado or TipoNotificacion.ObraRechazada or TipoNotificacion.EvidenciaRechazada => "danger",
            TipoNotificacion.Advertencia => "warning",
            TipoNotificacion.NuevaSolicitud => "primary",
            _ => "info"
        };
    }
}

public class NotificacionDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string? Tipo { get; set; }
    public string? UrlAccion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string? Icono { get; set; }
    public string? Color { get; set; }
    public bool Leida { get; set; }
}

public class NotificacionPruebaDto
{
    public Guid? UsuarioId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string? UrlAccion { get; set; }
}
