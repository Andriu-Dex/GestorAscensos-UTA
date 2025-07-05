using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SGA.Application.Interfaces;
using SGA.Application.Interfaces.Repositories;
using SGA.Domain.Entities;
using SGA.Domain.Enums;
using System.Text.Json;

namespace SGA.Application.Services;

/// <summary>
/// Servicio para gestionar notificaciones en tiempo real usando SignalR
/// </summary>
public class NotificacionTiempoRealService : INotificacionTiempoRealService
{
    private readonly INotificacionRepository _notificacionRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IHubContext<Hub> _hubContext; // Usando Hub genérico
    private readonly ILogger<NotificacionTiempoRealService> _logger;

    public NotificacionTiempoRealService(
        INotificacionRepository notificacionRepository,
        IUsuarioRepository usuarioRepository,
        IHubContext<Hub> hubContext,
        ILogger<NotificacionTiempoRealService> logger)
    {
        _notificacionRepository = notificacionRepository;
        _usuarioRepository = usuarioRepository;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task EnviarNotificacionAsync(Guid usuarioId, string titulo, string mensaje, TipoNotificacion tipo, string? urlAccion = null)
    {
        try
        {
            _logger.LogInformation("Iniciando envío de notificación al usuario {UsuarioId}: {Titulo}", usuarioId, titulo);
            
            // Crear notificación en la base de datos
            var notificacion = new Notificacion
            {
                UsuarioId = usuarioId,
                Titulo = titulo,
                Mensaje = mensaje,
                Tipo = tipo,
                UrlAccion = urlAccion,
                FechaCreacion = DateTime.UtcNow
            };

            await _notificacionRepository.CreateAsync(notificacion);
            _logger.LogInformation("Notificación guardada en base de datos con ID: {NotificacionId}", notificacion.Id);

            // Enviar notificación en tiempo real via SignalR
            var notificacionDto = new
            {
                id = notificacion.Id,
                titulo = titulo,
                mensaje = mensaje,
                tipo = tipo.ToString(),
                urlAccion = urlAccion,
                fechaCreacion = notificacion.FechaCreacion,
                icono = ObtenerIconoPorTipo(tipo),
                color = ObtenerColorPorTipo(tipo)
            };

            _logger.LogInformation("Enviando notificación a través de SignalR al grupo: User_{UsuarioId}", usuarioId);
            await _hubContext.Clients.Group($"User_{usuarioId}")
                .SendAsync("RecibirNotificacion", notificacionDto);

            _logger.LogInformation("✅ Notificación enviada al usuario {UsuarioId}: {Titulo}", usuarioId, titulo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al enviar notificación al usuario {UsuarioId}", usuarioId);
        }
    }

    public async Task EnviarNotificacionAdministradoresAsync(string titulo, string mensaje, TipoNotificacion tipo, string? urlAccion = null)
    {
        try
        {
            _logger.LogInformation("Iniciando envío de notificación a administradores: {Titulo}", titulo);
            
            // Obtener todos los administradores
            var administradores = await _usuarioRepository.GetAdministradoresAsync();
            _logger.LogInformation("Se encontraron {Count} administradores para enviar notificación", administradores.Count);

            // Crear notificaciones para cada administrador
            var tareas = administradores.Select(admin => 
                EnviarNotificacionAsync(admin.Id, titulo, mensaje, tipo, urlAccion));

            await Task.WhenAll(tareas);

            _logger.LogInformation("✅ Notificación enviada a {Count} administradores: {Titulo}", 
                administradores.Count, titulo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al enviar notificación a administradores");
        }
    }

    public async Task<List<Notificacion>> ObtenerNotificacionesUsuarioAsync(Guid usuarioId, int limit = 20)
    {
        return await _notificacionRepository.GetByUsuarioIdAsync(usuarioId, limit);
    }

    public async Task<List<Notificacion>> ObtenerNotificacionesNoLeidasAsync(Guid usuarioId)
    {
        return await _notificacionRepository.GetNoLeidasByUsuarioIdAsync(usuarioId);
    }

    public async Task<int> ObtenerContadorNoLeidasAsync(Guid usuarioId)
    {
        return await _notificacionRepository.GetCountNoLeidasByUsuarioIdAsync(usuarioId);
    }

    public async Task MarcarComoLeidaAsync(Guid notificacionId)
    {
        await _notificacionRepository.MarcarComoLeidaAsync(notificacionId);
    }

    public async Task MarcarTodasComoLeidasAsync(Guid usuarioId)
    {
        await _notificacionRepository.MarcarTodasComoLeidasAsync(usuarioId);
    }

    // Métodos específicos para tipos de notificaciones del sistema

    public async Task NotificarNuevaSolicitudAsync(string tipoSolicitud, string nombreDocente, string detalles = "")
    {
        var titulo = $"Nueva solicitud de {tipoSolicitud}";
        var mensaje = $"El docente {nombreDocente} ha enviado una nueva solicitud de {tipoSolicitud}.";
        if (!string.IsNullOrEmpty(detalles))
        {
            mensaje += $" {detalles}";
        }

        await EnviarNotificacionAdministradoresAsync(titulo, mensaje, TipoNotificacion.NuevaSolicitud, 
            GetUrlPorTipoSolicitud(tipoSolicitud));
    }

    public async Task NotificarAprobacionAsync(Guid usuarioId, string tipoSolicitud, string titulo, string detalles = "")
    {
        var tituloNotif = $"{tipoSolicitud} Aprobada";
        var mensaje = $"Tu {tipoSolicitud.ToLower()} '{titulo}' ha sido aprobada.";
        if (!string.IsNullOrEmpty(detalles))
        {
            mensaje += $" {detalles}";
        }

        var tipo = tipoSolicitud.ToLower() switch
        {
            "ascenso" => TipoNotificacion.AscensoAprobado,
            "certificado" => TipoNotificacion.CertificadoAprobado,
            "obra académica" => TipoNotificacion.ObraAprobada,
            "evidencia" => TipoNotificacion.EvidenciaAprobada,
            _ => TipoNotificacion.SolicitudAprobada
        };

        await EnviarNotificacionAsync(usuarioId, tituloNotif, mensaje, tipo);
    }

    public async Task NotificarRechazoAsync(Guid usuarioId, string tipoSolicitud, string titulo, string motivo = "")
    {
        var tituloNotif = $"{tipoSolicitud} Rechazada";
        var mensaje = $"Tu {tipoSolicitud.ToLower()} '{titulo}' ha sido rechazada.";
        if (!string.IsNullOrEmpty(motivo))
        {
            mensaje += $" Motivo: {motivo}";
        }

        var tipo = tipoSolicitud.ToLower() switch
        {
            "ascenso" => TipoNotificacion.AscensoRechazado,
            "certificado" => TipoNotificacion.CertificadoRechazado,
            "obra académica" => TipoNotificacion.ObraRechazada,
            "evidencia" => TipoNotificacion.EvidenciaRechazada,
            _ => TipoNotificacion.SolicitudRechazada
        };

        await EnviarNotificacionAsync(usuarioId, tituloNotif, mensaje, tipo);
    }

    public async Task NotificarAscensoAprobadoAsync(Guid usuarioId, string nivelAnterior, string nivelNuevo)
    {
        var titulo = "¡Felicitaciones! Ascenso Aprobado";
        var mensaje = $"Tu solicitud de ascenso ha sido aprobada. Has sido promovido de {nivelAnterior} a {nivelNuevo}.";

        await EnviarNotificacionAsync(usuarioId, titulo, mensaje, TipoNotificacion.AscensoAprobado, "/perfil");
    }

    public async Task NotificarAscensoRechazadoAsync(Guid usuarioId, string nivelSolicitado, string motivo)
    {
        var titulo = "Solicitud de Ascenso Rechazada";
        var mensaje = $"Tu solicitud de ascenso a {nivelSolicitado} ha sido rechazada.";
        if (!string.IsNullOrEmpty(motivo))
        {
            mensaje += $" Motivo: {motivo}";
        }

        await EnviarNotificacionAsync(usuarioId, titulo, mensaje, TipoNotificacion.AscensoRechazado);
    }

    public async Task EnviarNotificacionPorEmailAsync(string email, string titulo, string mensaje, TipoNotificacion tipo, string? urlAccion = null)
    {
        try
        {
            // Buscar usuario por email
            var usuario = await _usuarioRepository.GetByEmailAsync(email);
            if (usuario == null)
            {
                _logger.LogWarning("⚠️ Usuario no encontrado con email: {Email}", email);
                return;
            }

            // Enviar usando el método existente
            await EnviarNotificacionAsync(usuario.Id, titulo, mensaje, tipo, urlAccion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al enviar notificación por email {Email}", email);
        }
    }

    // Métodos auxiliares

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

    private static string? GetUrlPorTipoSolicitud(string tipoSolicitud)
    {
        return tipoSolicitud.ToLower() switch
        {
            "ascenso" => "/admin/solicitudes",
            "certificado" or "certificado de capacitación" => "/admin/certificados",
            "obra académica" => "/admin/obras",
            "evidencia" or "evidencia de investigación" => "/admin/evidencias",
            _ => "/admin"
        };
    }
}
