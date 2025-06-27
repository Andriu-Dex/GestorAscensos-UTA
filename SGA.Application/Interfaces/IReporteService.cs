namespace SGA.Application.Interfaces;

public interface IReporteService
{
    Task<byte[]> GenerarHojaVidaAsync(Guid docenteId);
    Task<byte[]> GenerarReporteSolicitudAsync(Guid solicitudId, Guid docenteId, bool esAdmin);
    Task<byte[]> GenerarEstadisticasAsync();
}
