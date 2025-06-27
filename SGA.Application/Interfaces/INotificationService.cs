namespace SGA.Application.Interfaces;

public interface INotificationService
{
    Task NotificarNuevaSolicitudObrasAsync(string nombreDocente, int cantidadObras);
    Task NotificarAprobacionObraAsync(string emailDocente, string tituloObra, string comentarios);
    Task NotificarRechazoObraAsync(string emailDocente, string tituloObra, string motivo);
    Task NotificarCambioEstadoSolicitudAsync(string emailDocente, string tipoSolicitud, string nuevoEstado);
}
