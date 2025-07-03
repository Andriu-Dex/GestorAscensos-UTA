namespace SGA.Application.Interfaces;

public interface INotificationService
{
    // Notificaciones para obras académicas
    Task NotificarNuevaSolicitudObrasAsync(string nombreDocente, int cantidadObras);
    Task NotificarAprobacionObraAsync(string emailDocente, string tituloObra, string comentarios);
    Task NotificarRechazoObraAsync(string emailDocente, string tituloObra, string motivo);
    
    // Notificaciones para certificados de capacitación
    Task NotificarNuevaSolicitudCertificadosAsync(string nombreDocente, int cantidadCertificados);
    Task NotificarAprobacionCertificadoAsync(string emailDocente, string nombreCurso, string comentarios);
    Task NotificarRechazoCertificadoAsync(string emailDocente, string nombreCurso, string motivo);
    
    // Notificaciones generales
    Task NotificarCambioEstadoSolicitudAsync(string emailDocente, string tipoSolicitud, string nuevoEstado);
    
    // Notificaciones para solicitudes de ascenso
    Task NotificarAprobacionAscensoAsync(string emailDocente, string nombreDocente, string nivelAnterior, string nivelNuevo);
    Task NotificarRechazoAscensoAsync(string emailDocente, string nombreDocente, string nivelSolicitado, string motivo);
}
