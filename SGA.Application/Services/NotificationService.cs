using Microsoft.Extensions.Logging;
using SGA.Application.Interfaces;

namespace SGA.Application.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public async Task NotificarNuevaSolicitudObrasAsync(string nombreDocente, int cantidadObras)
    {
        try
        {
            // TODO: Implementar notificación real (email, push, etc.)
            _logger.LogInformation("Nueva solicitud de obras académicas de {NombreDocente}: {CantidadObras} obras", 
                nombreDocente, cantidadObras);
            
            // Aquí iría la lógica para enviar notificaciones a administradores
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al notificar nueva solicitud de obras");
        }
    }

    public async Task NotificarAprobacionObraAsync(string emailDocente, string tituloObra, string comentarios)
    {
        try
        {
            _logger.LogInformation("Obra aprobada para {EmailDocente}: {TituloObra}", 
                emailDocente, tituloObra);
            
            // TODO: Enviar email al docente notificando la aprobación
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al notificar aprobación de obra");
        }
    }

    public async Task NotificarRechazoObraAsync(string emailDocente, string tituloObra, string motivo)
    {
        try
        {
            _logger.LogInformation("Obra rechazada para {EmailDocente}: {TituloObra}, Motivo: {Motivo}", 
                emailDocente, tituloObra, motivo);
            
            // TODO: Enviar email al docente notificando el rechazo
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al notificar rechazo de obra");
        }
    }

    public async Task NotificarCambioEstadoSolicitudAsync(string emailDocente, string tipoSolicitud, string nuevoEstado)
    {
        try
        {
            _logger.LogInformation("Cambio de estado para {EmailDocente}: {TipoSolicitud} -> {NuevoEstado}", 
                emailDocente, tipoSolicitud, nuevoEstado);
            
            // TODO: Enviar notificación de cambio de estado
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al notificar cambio de estado");
        }
    }
}
