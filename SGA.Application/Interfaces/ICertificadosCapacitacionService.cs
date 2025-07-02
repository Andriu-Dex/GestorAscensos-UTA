using SGA.Application.DTOs;

namespace SGA.Application.Interfaces;

/// <summary>
/// Interfaz para el servicio de gestión de certificados de capacitación
/// </summary>
public interface ICertificadosCapacitacionService
{
    // Métodos para docentes
    Task<ResponseCertificadosCapacitacionDto> GetCertificadosDocenteAsync(string cedula);
    Task<ResponseCertificadosCapacitacionDto> SolicitarNuevosCertificadosAsync(string cedula, SolicitarCertificadosCapacitacionDto solicitud);
    Task<ResponseCertificadosCapacitacionDto> GetSolicitudesPendientesAsync(string cedula);
    Task<ResponseCertificadosCapacitacionDto> GetTodasSolicitudesDocenteAsync(string cedula);
    Task<byte[]?> VisualizarArchivoCertificadoAsync(Guid solicitudId, string cedula);
    Task<byte[]?> DescargarArchivoCertificadoAsync(Guid solicitudId);
    
    // Métodos para gestión de documentos del usuario
    Task<ResponseGenericoCertificadoDto> EliminarSolicitudCertificadoAsync(Guid solicitudId, string cedula);
    Task<ResponseGenericoCertificadoDto> EditarMetadatosCertificadoAsync(Guid solicitudId, string cedula, EditarMetadatosCertificadoDto metadatos);
    Task<ResponseGenericoCertificadoDto> ReemplazarArchivoCertificadoAsync(Guid solicitudId, string cedula, ReemplazarArchivoCertificadoDto archivo);
    Task<ResponseGenericoCertificadoDto> AgregarComentarioCertificadoAsync(Guid solicitudId, string cedula, string comentario);
    Task<ResponseGenericoCertificadoDto> ReenviarSolicitudCertificadoAsync(Guid solicitudId, string cedula);
    
    // Métodos para administradores
    Task<ResponseSolicitudesCertificadosAdminDto> GetTodasSolicitudesCertificadosAsync();
    Task<ResponseSolicitudesCertificadosAdminDto> GetSolicitudesCertificadosPorRevisarAsync();
    Task<ResponseCertificadosCapacitacionDto> AprobarSolicitudCertificadoAsync(Guid solicitudId, string comentarios);
    Task<ResponseCertificadosCapacitacionDto> RechazarSolicitudCertificadoAsync(Guid solicitudId, string motivo);
    Task<ResponseGenericoCertificadoDto> RevisarSolicitudCertificadoAsync(RevisionSolicitudCertificadoDto revision, string adminEmail);
    Task<byte[]?> GetArchivoCertificadoSolicitudAsync(Guid solicitudId);
}
