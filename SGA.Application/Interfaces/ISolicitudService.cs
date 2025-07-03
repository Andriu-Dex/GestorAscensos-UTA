using SGA.Application.DTOs.Solicitudes;

namespace SGA.Application.Interfaces;

public interface ISolicitudService
{
    Task<SolicitudAscensoDto> CrearSolicitudAsync(Guid docenteId, CrearSolicitudRequest request);
    Task<List<SolicitudAscensoDto>> GetSolicitudesByDocenteAsync(Guid docenteId);
    Task<List<SolicitudAscensoDto>> GetTodasLasSolicitudesAsync();
    Task<SolicitudAscensoDto?> GetSolicitudByIdAsync(Guid solicitudId);
    Task<bool> ProcesarSolicitudAsync(Guid solicitudId, ProcesarSolicitudRequest request, Guid administradorId);
    Task<bool> CancelarSolicitudAsync(Guid solicitudId, Guid docenteId);
    Task<bool> TieneDocumenteSolicitudActivaAsync(Guid docenteId);
    Task<byte[]?> DescargarDocumentoAsync(Guid documentoId);
}
