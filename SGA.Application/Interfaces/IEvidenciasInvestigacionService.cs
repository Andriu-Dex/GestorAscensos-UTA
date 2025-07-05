using SGA.Application.DTOs.EvidenciasInvestigacion;

namespace SGA.Application.Interfaces;

/// <summary>
/// Interface para el servicio de gestión de evidencias de investigación
/// Sigue el mismo patrón que ICertificadosCapacitacionService
/// </summary>
public interface IEvidenciasInvestigacionService
{
    // Métodos para docentes
    Task<ResponseEvidenciasInvestigacionDto> SolicitarNuevasEvidenciasAsync(string cedula, SolicitarEvidenciasInvestigacionDto solicitud);
    Task<ResponseEvidenciasInvestigacionDto> GetMisEvidenciasAsync(string cedula);
    Task<ResponseGenericoEvidenciaDto> EditarMetadatosEvidenciaAsync(Guid evidenciaId, string cedula, EditarMetadatosEvidenciaDto datos);
    Task<ResponseGenericoEvidenciaDto> ReemplazarArchivoEvidenciaAsync(Guid evidenciaId, string cedula, ReemplazarArchivoEvidenciaDto archivo);
    Task<ResponseGenericoEvidenciaDto> EliminarSolicitudEvidenciaAsync(Guid evidenciaId, string cedula);
    Task<ResponseGenericoEvidenciaDto> ReenviarSolicitudEvidenciaAsync(Guid evidenciaId, string cedula);
    Task<byte[]?> GetArchivoEvidenciaAsync(Guid evidenciaId, string cedula);

    // Métodos para administradores
    Task<ResponseSolicitudesEvidenciasAdminDto> GetTodasSolicitudesEvidenciasAsync();
    Task<ResponseSolicitudesEvidenciasAdminDto> GetSolicitudesEvidenciasPorRevisarAsync();
    Task<ResponseGenericoEvidenciaDto> RevisarSolicitudEvidenciaAsync(RevisionSolicitudEvidenciaDto revision, string adminEmail);
    Task<byte[]?> GetArchivoEvidenciaSolicitudAsync(Guid solicitudId);
}
