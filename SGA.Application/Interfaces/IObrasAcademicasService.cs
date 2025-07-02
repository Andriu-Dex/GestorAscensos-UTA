using SGA.Application.DTOs;
using SGA.Application.DTOs.ExternalData;
using SGA.Application.DTOs.Docentes;

namespace SGA.Application.Interfaces;

public interface IObrasAcademicasService
{
    Task<ResponseObrasAcademicasDto> GetObrasDocenteAsync(string cedula);
    Task<ResponseObrasAcademicasDto> SolicitarNuevasObrasAsync(string cedula, SolicitudObrasAcademicasDto solicitud);
    Task<ResponseObrasAcademicasDto> GetSolicitudesPendientesAsync(string cedula);
    Task<ResponseObrasAcademicasDto> GetTodasSolicitudesDocenteAsync(string cedula);
    Task<ResponseObrasAcademicasDto> AprobarSolicitudAsync(Guid solicitudId, string comentarios);
    Task<ResponseObrasAcademicasDto> RechazarSolicitudAsync(Guid solicitudId, string motivo);
    Task<ResponseObrasAcademicasDto> GetSolicitudesPorRevisarAsync();
    Task<byte[]?> DescargarArchivoObraAsync(Guid solicitudId);
    Task<ImportarDatosResponse> ImportarObrasDesdeExternoAsync(string cedula);
    
    // Métodos para gestión de documentos del usuario
    Task<ResponseGenericoDto> EliminarSolicitudAsync(Guid solicitudId, string cedula);
    Task<ResponseGenericoDto> EditarMetadatosSolicitudAsync(Guid solicitudId, string cedula, EditarMetadatosSolicitudDto metadatos);
    Task<ResponseGenericoDto> ReemplazarArchivoSolicitudAsync(Guid solicitudId, string cedula, ReemplazarArchivoDto archivo);
    Task<ResponseGenericoDto> AgregarComentarioSolicitudAsync(Guid solicitudId, string cedula, string comentario);
    Task<ResponseGenericoDto> ReenviarSolicitudAsync(Guid solicitudId, string cedula);
    Task<byte[]?> VisualizarArchivoSolicitudAsync(Guid solicitudId, string cedula);
    
    // Métodos para administradores
    Task<ResponseSolicitudesAdminDto> GetTodasSolicitudesAsync();
    Task<ResponseGenericoDto> RevisarSolicitudAsync(RevisionSolicitudDto revision, string adminEmail);
    Task<byte[]?> GetArchivoSolicitudAsync(Guid solicitudId);
}
