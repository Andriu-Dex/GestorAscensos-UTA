namespace SGA.Application.Interfaces;

public interface IReporteService
{
    Task<byte[]> GenerarHojaDeVidaAsync(Guid docenteId);
    Task<byte[]> GenerarReporteProcesoAscensoAsync(Guid solicitudId);
    Task<byte[]> GenerarReporteSolicitudesAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null);
}
