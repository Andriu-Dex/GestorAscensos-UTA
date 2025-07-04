using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SGA.Application.Interfaces;
using SGA.Domain.Enums;
using System.Security.Claims;

namespace SGA.Api.Controllers;

/// <summary>
/// Controlador para probar el sistema de notificaciones en tiempo real
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificacionesPruebaController : ControllerBase
{
    private readonly INotificacionTiempoRealService _notificacionService;
    private readonly ILogger<NotificacionesPruebaController> _logger;

    public NotificacionesPruebaController(
        INotificacionTiempoRealService notificacionService,
        ILogger<NotificacionesPruebaController> logger)
    {
        _notificacionService = notificacionService;
        _logger = logger;
    }

    /// <summary>
    /// Enviar una notificación de prueba al usuario actual
    /// </summary>
    [HttpPost("enviar-prueba")]
    public async Task<IActionResult> EnviarNotificacionPrueba([FromBody] NotificacionPruebaRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
            
            _logger.LogInformation("🧪 TEST: Enviando notificación de prueba");
            _logger.LogInformation("🧪 TEST: Usuario ID Claim: {UserIdClaim}", userIdClaim);
            _logger.LogInformation("🧪 TEST: Email Claim: {EmailClaim}", emailClaim);
            
            if (string.IsNullOrEmpty(userIdClaim))
            {
                _logger.LogWarning("🧪 TEST: No se encontró claim de usuario ID");
                return BadRequest("No se pudo identificar al usuario");
            }

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("🧪 TEST: El claim del usuario no es un GUID válido: {UserIdClaim}", userIdClaim);
                return BadRequest("ID de usuario inválido");
            }

            var titulo = request.Titulo ?? "Notificación de Prueba";
            var mensaje = request.Mensaje ?? $"Esta es una notificación de prueba enviada a las {DateTime.Now:HH:mm:ss}";
            var tipo = request.Tipo ?? TipoNotificacion.Info;

            _logger.LogInformation("🧪 TEST: Enviando notificación - Título: {Titulo}, Usuario: {UserId}", titulo, userId);

            await _notificacionService.EnviarNotificacionAsync(userId, titulo, mensaje, tipo);

            return Ok(new { 
                Mensaje = "Notificación de prueba enviada exitosamente",
                UsuarioId = userId,
                Titulo = titulo,
                Tipo = tipo.ToString(),
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🧪 TEST: Error al enviar notificación de prueba");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Enviar una notificación de prueba a todos los administradores
    /// </summary>
    [HttpPost("enviar-prueba-admins")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> EnviarNotificacionPruebaAdmins([FromBody] NotificacionPruebaRequest request)
    {
        try
        {
            _logger.LogInformation("🧪 TEST: Enviando notificación de prueba a administradores");

            var titulo = request.Titulo ?? "Notificación de Prueba para Administradores";
            var mensaje = request.Mensaje ?? $"Esta es una notificación de prueba para administradores enviada a las {DateTime.Now:HH:mm:ss}";
            var tipo = request.Tipo ?? TipoNotificacion.NuevaSolicitud;

            await _notificacionService.EnviarNotificacionAdministradoresAsync(titulo, mensaje, tipo);

            return Ok(new { 
                Mensaje = "Notificación de prueba enviada a administradores exitosamente",
                Titulo = titulo,
                Tipo = tipo.ToString(),
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🧪 TEST: Error al enviar notificación de prueba a administradores");
            return StatusCode(500, "Error interno del servidor");
        }
    }
}

public class NotificacionPruebaRequest
{
    public string? Titulo { get; set; }
    public string? Mensaje { get; set; }
    public TipoNotificacion? Tipo { get; set; }
}
