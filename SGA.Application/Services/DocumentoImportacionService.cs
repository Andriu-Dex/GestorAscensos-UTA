using Microsoft.Extensions.Logging;
using SGA.Application.DTOs.DocumentoImportacion;
using SGA.Application.Interfaces;
using SGA.Application.Interfaces.Repositories;
using SGA.Domain.Entities;
using SGA.Domain.Enums;

namespace SGA.Application.Services;

/// <summary>
/// Servicio para importación de documentos existentes
/// </summary>
public class DocumentoImportacionService : IDocumentoImportacionService
{
    private readonly IDocumentoRepository _documentoRepository;
    private readonly IDocenteRepository _docenteRepository;
    private readonly IDocumentoService _documentoService;
    private readonly ILogger<DocumentoImportacionService> _logger;

    public DocumentoImportacionService(
        IDocumentoRepository documentoRepository,
        IDocenteRepository docenteRepository,
        IDocumentoService documentoService,
        ILogger<DocumentoImportacionService> logger)
    {
        _documentoRepository = documentoRepository;
        _docenteRepository = docenteRepository;
        _documentoService = documentoService;
        _logger = logger;
    }

    /// <summary>
    /// Buscar documentos disponibles para importación
    /// </summary>
    public async Task<List<DocumentoImportacionDto>> BuscarDocumentosImportablesAsync(string cedula, FiltrosImportacionDto filtros)
    {
        try
        {
            var docente = await _docenteRepository.GetByCedulaAsync(cedula);
            if (docente == null)
            {
                _logger.LogWarning("Docente no encontrado para cédula: {Cedula}", cedula);
                return new List<DocumentoImportacionDto>();
            }

            var documentos = await _documentoRepository.GetDocumentosImportablesAsync(docente.Id, filtros);
            
            var documentosDto = new List<DocumentoImportacionDto>();
            
            foreach (var documento in documentos)
            {
                var esImportable = await _documentoRepository.EsDocumentoDisponibleParaImportacionAsync(documento.Id, docente.Id);
                var motivoNoImportable = string.Empty;
                
                if (!esImportable)
                {
                    motivoNoImportable = documento.FueUtilizadoEnSolicitudAprobada 
                        ? "Documento ya utilizado en solicitud aprobada"
                        : "Documento no disponible para importación";
                }

                documentosDto.Add(new DocumentoImportacionDto
                {
                    Id = documento.Id,
                    Nombre = documento.NombreArchivo,
                    TipoDocumento = documento.TipoDocumento.ToString(),
                    FechaCreacion = documento.FechaCreacion,
                    TamanoArchivo = documento.TamanoArchivo,
                    SolicitudOrigen = documento.SolicitudAscensoId?.ToString(),
                    EsImportable = esImportable,
                    MotivoNoImportable = motivoNoImportable,
                    TipoDocumentoTexto = ObtenerTextoTipoDocumento(documento.TipoDocumento),
                    TamanoFormateado = FormatearTamano(documento.TamanoArchivo)
                });
            }

            return documentosDto.OrderByDescending(d => d.FechaCreacion).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar documentos importables para docente {Cedula}", cedula);
            return new List<DocumentoImportacionDto>();
        }
    }

    /// <summary>
    /// Importar documentos seleccionados
    /// </summary>
    public async Task<ImportarDocumentosResponseDto> ImportarDocumentosSeleccionadosAsync(string cedula, List<Guid> documentosIds)
    {
        try
        {
            // Validar parámetros
            if (string.IsNullOrEmpty(cedula))
            {
                return new ImportarDocumentosResponseDto
                {
                    Exitoso = false,
                    Mensaje = "La cédula es requerida",
                    Errores = new List<string> { "Cédula no proporcionada" }
                };
            }

            if (documentosIds == null || !documentosIds.Any())
            {
                return new ImportarDocumentosResponseDto
                {
                    Exitoso = false,
                    Mensaje = "Debe seleccionar al menos un documento para importar",
                    Errores = new List<string> { "No se seleccionaron documentos" }
                };
            }

            var docente = await _docenteRepository.GetByCedulaAsync(cedula);
            if (docente == null)
            {
                return new ImportarDocumentosResponseDto
                {
                    Exitoso = false,
                    Mensaje = "Docente no encontrado"
                };
            }

            var documentosImportados = new List<Guid>();
            var errores = new List<string>();

            foreach (var documentoId in documentosIds)
            {
                try
                {
                    var documentoOriginal = await _documentoRepository.GetByIdAsync(documentoId);
                    if (documentoOriginal == null)
                    {
                        errores.Add($"Documento {documentoId}: No encontrado");
                        continue;
                    }

                    var esImportable = await _documentoRepository.EsDocumentoDisponibleParaImportacionAsync(documentoId, docente.Id);
                    if (!esImportable)
                    {
                        errores.Add($"{documentoOriginal.NombreArchivo}: No disponible para importación");
                        continue;
                    }

                    // Clonar el documento sin solicitud destino específica (se asignará después)
                    var documentoClonado = await _documentoRepository.ClonarDocumentoAsync(documentoOriginal);
                    
                    documentosImportados.Add(documentoClonado.Id);
                    
                    _logger.LogInformation("Documento importado: {DocumentoId} -> {NuevoDocumentoId}", 
                        documentoId, documentoClonado.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al importar documento {DocumentoId}", documentoId);
                    errores.Add($"Error al importar documento: {ex.Message}");
                }
            }

            var totalSolicitados = documentosIds.Count;
            var totalImportados = documentosImportados.Count;

            return new ImportarDocumentosResponseDto
            {
                Exitoso = totalImportados > 0,
                Mensaje = totalImportados > 0 
                    ? $"Se importaron {totalImportados} de {totalSolicitados} documentos exitosamente."
                    : "No se pudo importar ningún documento.",
                DocumentosImportados = documentosImportados,
                Errores = errores
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al importar documentos para docente {Cedula}", cedula);
            return new ImportarDocumentosResponseDto
            {
                Exitoso = false,
                Mensaje = $"Error al importar documentos: {ex.Message}",
                Errores = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Obtener detalle de un documento específico
    /// </summary>
    public async Task<DocumentoImportacionDto?> ObtenerDetalleDocumentoAsync(Guid documentoId, string cedula)
    {
        try
        {
            var docente = await _docenteRepository.GetByCedulaAsync(cedula);
            if (docente == null)
            {
                return null;
            }

            var documento = await _documentoRepository.GetByIdAsync(documentoId);
            if (documento == null)
            {
                return null;
            }

            var esImportable = await _documentoRepository.EsDocumentoDisponibleParaImportacionAsync(documentoId, docente.Id);
            var motivoNoImportable = string.Empty;
            
            if (!esImportable)
            {
                motivoNoImportable = documento.FueUtilizadoEnSolicitudAprobada 
                    ? "Documento ya utilizado en solicitud aprobada"
                    : "Documento no disponible para importación";
            }

            return new DocumentoImportacionDto
            {
                Id = documento.Id,
                Nombre = documento.NombreArchivo,
                TipoDocumento = documento.TipoDocumento.ToString(),
                FechaCreacion = documento.FechaCreacion,
                TamanoArchivo = documento.TamanoArchivo,
                SolicitudOrigen = documento.SolicitudAscensoId?.ToString(),
                EsImportable = esImportable,
                MotivoNoImportable = motivoNoImportable,
                TipoDocumentoTexto = ObtenerTextoTipoDocumento(documento.TipoDocumento),
                TamanoFormateado = FormatearTamano(documento.TamanoArchivo)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener detalle del documento {DocumentoId}", documentoId);
            return null;
        }
    }

    /// <summary>
    /// Validar si un documento es importable
    /// </summary>
    public async Task<bool> ValidarDocumentoImportableAsync(Guid documentoId, string cedula)
    {
        try
        {
            var docente = await _docenteRepository.GetByCedulaAsync(cedula);
            if (docente == null)
            {
                return false;
            }

            return await _documentoRepository.EsDocumentoDisponibleParaImportacionAsync(documentoId, docente.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar documento importable {DocumentoId}", documentoId);
            return false;
        }
    }

    /// <summary>
    /// Obtener contenido del documento para visualización/descarga
    /// </summary>
    public async Task<byte[]?> ObtenerContenidoDocumentoAsync(Guid documentoId)
    {
        try
        {
            // Reutilizar el servicio existente de documentos que maneja correctamente
            // tanto archivos almacenados en BD como en sistema de archivos
            return await _documentoService.DescargarDocumentoAsync(documentoId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener contenido del documento {DocumentoId}", documentoId);
            return null;
        }
    }

    /// <summary>
    /// Obtener texto descriptivo del tipo de documento
    /// </summary>
    private static string ObtenerTextoTipoDocumento(TipoDocumento tipo)
    {
        return tipo switch
        {
            TipoDocumento.ObrasAcademicas => "Obras Académicas",
            TipoDocumento.CertificadosCapacitacion => "Certificados de Capacitación",
            TipoDocumento.CertificadoInvestigacion => "Evidencias de Investigación",
            TipoDocumento.CertificadoTrabajo => "Certificado de Trabajo",
            TipoDocumento.EvaluacionesDocentes => "Evaluaciones Docentes",
            TipoDocumento.Otro => "Otro",
            _ => "Desconocido"
        };
    }

    /// <summary>
    /// Formatear tamaño de archivo
    /// </summary>
    private static string FormatearTamano(long tamano)
    {
        const long KB = 1024;
        const long MB = KB * 1024;
        const long GB = MB * 1024;

        if (tamano < KB)
            return $"{tamano} B";
        else if (tamano < MB)
            return $"{tamano / KB:F1} KB";
        else if (tamano < GB)
            return $"{tamano / MB:F1} MB";
        else
            return $"{tamano / GB:F1} GB";
    }
}
