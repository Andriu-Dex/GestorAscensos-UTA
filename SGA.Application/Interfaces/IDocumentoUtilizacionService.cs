using SGA.Domain.Entities;

namespace SGA.Application.Interfaces;

/// <summary>
/// Servicio para gestionar la utilización de documentos en solicitudes aprobadas
/// </summary>
public interface IDocumentoUtilizacionService
{
    /// <summary>
    /// Marca todos los documentos de una solicitud como utilizados cuando la solicitud es aprobada
    /// </summary>
    /// <param name="solicitudId">ID de la solicitud aprobada</param>
    /// <returns>True si se marcaron los documentos correctamente</returns>
    Task<bool> MarcarDocumentosComoUtilizadosAsync(Guid solicitudId);
    
    /// <summary>
    /// Obtiene los documentos disponibles (no utilizados) para un docente
    /// </summary>
    /// <param name="docenteId">ID del docente</param>
    /// <returns>Lista de documentos disponibles para usar</returns>
    Task<List<Documento>> ObtenerDocumentosDisponiblesAsync(Guid docenteId);
    
    /// <summary>
    /// Verifica si un documento específico está disponible para usar
    /// </summary>
    /// <param name="documentoId">ID del documento</param>
    /// <returns>True si el documento está disponible para usar</returns>
    Task<bool> DocumentoEstaDisponibleAsync(Guid documentoId);
    
    /// <summary>
    /// Obtiene información detallada sobre por qué un documento no está disponible
    /// </summary>
    /// <param name="documentoId">ID del documento</param>
    /// <returns>Información sobre la utilización del documento</returns>
    Task<(bool EstaDisponible, string? MotivoNoDisponible, DateTime? FechaUtilizacion)> ObtenerEstadoDocumentoAsync(Guid documentoId);
    
    /// <summary>
    /// Verifica si los documentos de una obra académica están disponibles para usar
    /// </summary>
    /// <param name="solicitudId">ID de la solicitud de obra académica</param>
    /// <returns>Información sobre la disponibilidad de los documentos</returns>
    Task<(bool EstaDisponible, string? MotivoNoDisponible, DateTime? FechaUtilizacion)> VerificarDisponibilidadObraAsync(Guid solicitudId);
    
    /// <summary>
    /// Verifica si los documentos de un certificado de capacitación están disponibles para usar
    /// </summary>
    /// <param name="certificadoId">ID del certificado de capacitación</param>
    /// <returns>Información sobre la disponibilidad de los documentos</returns>
    Task<(bool EstaDisponible, string? MotivoNoDisponible, DateTime? FechaUtilizacion)> VerificarDisponibilidadCertificadoAsync(Guid certificadoId);
    
    /// <summary>
    /// Verifica si los documentos de una evidencia de investigación están disponibles para usar
    /// </summary>
    /// <param name="evidenciaId">ID de la evidencia de investigación</param>
    /// <returns>Información sobre la disponibilidad de los documentos</returns>
    Task<(bool EstaDisponible, string? MotivoNoDisponible, DateTime? FechaUtilizacion)> VerificarDisponibilidadEvidenciaAsync(Guid evidenciaId);
}
