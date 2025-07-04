using SGA.Domain.Entities;

namespace SGA.Application.Interfaces;

/// <summary>
/// Interfaz para el repositorio de notificaciones
/// </summary>
public interface INotificacionRepository
{
    Task<Notificacion> CreateAsync(Notificacion notificacion);
    Task<List<Notificacion>> GetByUsuarioIdAsync(Guid usuarioId, int limit = 20);
    Task<List<Notificacion>> GetNoLeidasByUsuarioIdAsync(Guid usuarioId);
    Task<int> GetCountNoLeidasByUsuarioIdAsync(Guid usuarioId);
    Task<Notificacion?> GetByIdAsync(Guid id);
    Task UpdateAsync(Notificacion notificacion);
    Task MarcarComoLeidaAsync(Guid notificacionId);
    Task MarcarTodasComoLeidasAsync(Guid usuarioId);
    Task DeleteAsync(Guid id);
    Task DeleteOlderThanAsync(DateTime fecha);
}
