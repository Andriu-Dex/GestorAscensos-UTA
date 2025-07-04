namespace SGA.Application.Interfaces;

public interface IReporteService
{
    // Reportes existentes
    Task<byte[]> GenerarHojaVidaAsync(Guid docenteId);
    Task<byte[]> GenerarReporteSolicitudAsync(Guid solicitudId, Guid docenteId, bool esAdmin);
    Task<byte[]> GenerarEstadisticasAsync();
    
    // Nuevos reportes para usuarios
    Task<byte[]> GenerarReporteEstadoRequisitosPorNivelAsync(Guid docenteId);
    Task<byte[]> GenerarReporteHistorialAscensosAsync(Guid docenteId);
    Task<byte[]> GenerarReporteCapacitacionesAsync(Guid docenteId);
    Task<byte[]> GenerarReporteObrasAcademicasAsync(Guid docenteId);
    Task<byte[]> GenerarReporteCompletoAscensoAsync(Guid docenteId);
    Task<byte[]> GenerarCertificadoEstadoDocenteAsync(Guid docenteId);
    
    // Versiones para vista previa en modal (HTML)
    Task<string> GenerarVistaHojaVidaAsync(Guid docenteId);
    Task<string> GenerarVistaSolicitudAsync(Guid solicitudId, Guid docenteId, bool esAdmin);
    Task<string> GenerarVistaEstadoRequisitosAsync(Guid docenteId);
    Task<string> GenerarVistaHistorialAscensosAsync(Guid docenteId);
    Task<string> GenerarVistaCapacitacionesAsync(Guid docenteId);
    Task<string> GenerarVistaObrasAcademicasAsync(Guid docenteId);
    Task<string> GenerarVistaReporteCompletoAsync(Guid docenteId);
    Task<string> GenerarVistaCertificadoEstadoAsync(Guid docenteId);
}
