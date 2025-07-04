using SGA.Domain.Entities;
using SGA.Domain.Enums;

namespace SGA.Application.Interfaces;

/// <summary>
/// Interfaz para el servicio de notificaciones en tiempo real
/// </summary>
public interface INotificacionTiempoRealService
{
    // Crear y enviar notificaciones
    Task EnviarNotificacionAsync(Guid usuarioId, string titulo, string mensaje, TipoNotificacion tipo, string? urlAccion = null);
    Task EnviarNotificacionPorEmailAsync(string email, string titulo, string mensaje, TipoNotificacion tipo, string? urlAccion = null);
    Task EnviarNotificacionAdministradoresAsync(string titulo, string mensaje, TipoNotificacion tipo, string? urlAccion = null);
    
    // Gestión de notificaciones
    Task<List<Notificacion>> ObtenerNotificacionesUsuarioAsync(Guid usuarioId, int limit = 20);
    Task<List<Notificacion>> ObtenerNotificacionesNoLeidasAsync(Guid usuarioId);
    Task<int> ObtenerContadorNoLeidasAsync(Guid usuarioId);
    Task MarcarComoLeidaAsync(Guid notificacionId);
    Task MarcarTodasComoLeidasAsync(Guid usuarioId);
    
    // Notificaciones específicas del sistema
    Task NotificarNuevaSolicitudAsync(string tipoSolicitud, string nombreDocente, string detalles = "");
    Task NotificarAprobacionAsync(Guid usuarioId, string tipoSolicitud, string titulo, string detalles = "");
    Task NotificarRechazoAsync(Guid usuarioId, string tipoSolicitud, string titulo, string motivo = "");
    Task NotificarAscensoAprobadoAsync(Guid usuarioId, string nivelAnterior, string nivelNuevo);
    Task NotificarAscensoRechazadoAsync(Guid usuarioId, string nivelSolicitado, string motivo);
}
