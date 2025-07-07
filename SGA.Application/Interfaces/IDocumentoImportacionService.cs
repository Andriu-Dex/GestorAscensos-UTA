using SGA.Application.DTOs.DocumentoImportacion;

namespace SGA.Application.Interfaces;

/// <summary>
/// Interfaz para el servicio de importación de documentos
/// </summary>
public interface IDocumentoImportacionService
{
    /// <summary>
    /// Buscar documentos disponibles para importación
    /// </summary>
    /// <param name="cedula">Cédula del docente</param>
    /// <param name="filtros">Filtros de búsqueda</param>
    /// <returns>Lista de documentos disponibles</returns>
    Task<List<DocumentoImportacionDto>> BuscarDocumentosImportablesAsync(string cedula, FiltrosImportacionDto filtros);

    /// <summary>
    /// Importar documentos seleccionados
    /// </summary>
    /// <param name="cedula">Cédula del docente</param>
    /// <param name="documentosIds">Lista de IDs de documentos a importar</param>
    /// <returns>Resultado de la importación</returns>
    Task<ImportarDocumentosResponseDto> ImportarDocumentosSeleccionadosAsync(string cedula, List<Guid> documentosIds);

    /// <summary>
    /// Obtener detalle de un documento específico
    /// </summary>
    /// <param name="documentoId">ID del documento</param>
    /// <param name="cedula">Cédula del docente</param>
    /// <returns>Detalle del documento</returns>
    Task<DocumentoImportacionDto?> ObtenerDetalleDocumentoAsync(Guid documentoId, string cedula);

    /// <summary>
    /// Validar si un documento es importable
    /// </summary>
    /// <param name="documentoId">ID del documento</param>
    /// <param name="cedula">Cédula del docente</param>
    /// <returns>True si es importable, false si no</returns>
    Task<bool> ValidarDocumentoImportableAsync(Guid documentoId, string cedula);

    /// <summary>
    /// Obtener contenido del documento para visualización/descarga
    /// </summary>
    /// <param name="documentoId">ID del documento</param>
    /// <returns>Contenido del documento en bytes</returns>
    Task<byte[]?> ObtenerContenidoDocumentoAsync(Guid documentoId);
}
