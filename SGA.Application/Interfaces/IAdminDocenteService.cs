using SGA.Application.DTOs.Admin;

namespace SGA.Application.Interfaces;

public interface IAdminDocenteService
{
    /// <summary>
    /// Obtiene todos los docentes para la vista de administración
    /// </summary>
    Task<List<DocenteAdminDto>> GetAllDocentesAsync();

    /// <summary>
    /// Obtiene el detalle completo de un docente
    /// </summary>
    Task<DocenteDetalleAdminDto?> GetDocenteDetalleAsync(Guid docenteId);

    /// <summary>
    /// Obtiene las solicitudes de un docente
    /// </summary>
    Task<List<SolicitudResumenDto>> GetSolicitudesDocenteAsync(Guid docenteId);

    /// <summary>
    /// Genera un reporte PDF de un docente
    /// </summary>
    Task<byte[]> GenerarReporteDocenteAsync(Guid docenteId);

    /// <summary>
    /// Actualiza el nivel de un docente
    /// </summary>
    Task<bool> ActualizarNivelDocenteAsync(Guid docenteId, ActualizarNivelDocenteDto dto, string adminEmail);

    /// <summary>
    /// Obtiene estadísticas generales de docentes
    /// </summary>
    Task<EstadisticasDocentesDto> GetEstadisticasDocentesAsync();

    /// <summary>
    /// Obtiene la lista de facultades
    /// </summary>
    Task<List<string>> GetFacultadesAsync();
}
