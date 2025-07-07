using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGA.Application.Interfaces;
using SGA.Application.Interfaces.Repositories;
using SGA.Domain.Entities;
using SGA.Domain.Enums;

namespace SGA.Application.Services;

/// <summary>
/// Servicio para gestionar la utilización de documentos en solicitudes aprobadas
/// </summary>
public class DocumentoUtilizacionService : IDocumentoUtilizacionService
{
    private readonly IDocumentoRepository _documentoRepository;
    private readonly ISolicitudAscensoRepository _solicitudRepository;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DocumentoUtilizacionService> _logger;

    public DocumentoUtilizacionService(
        IDocumentoRepository documentoRepository,
        ISolicitudAscensoRepository solicitudRepository,
        IApplicationDbContext context,
        ILogger<DocumentoUtilizacionService> logger)
    {
        _documentoRepository = documentoRepository;
        _solicitudRepository = solicitudRepository;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Marca todos los documentos de una solicitud como utilizados cuando la solicitud es aprobada
    /// </summary>
    public async Task<bool> MarcarDocumentosComoUtilizadosAsync(Guid solicitudId)
    {
        try
        {
            // Obtener la solicitud para verificar que está aprobada
            var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
            if (solicitud == null || solicitud.Estado != Domain.Enums.EstadoSolicitud.Aprobada)
            {
                return false;
            }

            // Obtener todos los documentos asociados a esta solicitud
            var documentos = await _documentoRepository.GetBySolicitudIdAsync(solicitudId);
            
            if (!documentos.Any())
            {
                return true; // No hay documentos que marcar, se considera exitoso
            }

            // Marcar cada documento como utilizado
            foreach (var documento in documentos)
            {
                documento.FueUtilizadoEnSolicitudAprobada = true;
                documento.SolicitudAprobadaId = solicitudId;
                documento.FechaUtilizacion = DateTime.UtcNow;
                
                await _documentoRepository.UpdateAsync(documento);
            }

            return true;
        }
        catch (Exception ex)
        {
            // Log del error para depuración
            _logger.LogError(ex, "Error al marcar documentos como utilizados para solicitud {SolicitudId}", solicitudId);
            return false;
        }
    }

    /// <summary>
    /// Obtiene los documentos disponibles (no utilizados) para un docente
    /// </summary>
    public async Task<List<Documento>> ObtenerDocumentosDisponiblesAsync(Guid docenteId)
    {
        try
        {
            return await _context.Documentos
                .Where(d => d.DocenteId == docenteId && 
                           !d.FueUtilizadoEnSolicitudAprobada)
                .OrderByDescending(d => d.FechaCreacion)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener documentos disponibles para docente {DocenteId}", docenteId);
            return new List<Documento>();
        }
    }

    /// <summary>
    /// Verifica si un documento específico está disponible para usar
    /// </summary>
    public async Task<bool> DocumentoEstaDisponibleAsync(Guid documentoId)
    {
        try
        {
            var documento = await _documentoRepository.GetByIdAsync(documentoId);
            return documento != null && !documento.FueUtilizadoEnSolicitudAprobada;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Obtiene información detallada sobre por qué un documento no está disponible
    /// </summary>
    public async Task<(bool EstaDisponible, string? MotivoNoDisponible, DateTime? FechaUtilizacion)> ObtenerEstadoDocumentoAsync(Guid documentoId)
    {
        try
        {
            var documento = await _context.Documentos
                .Include(d => d.SolicitudAprobada)
                .ThenInclude(s => s!.Docente)
                .FirstOrDefaultAsync(d => d.Id == documentoId);

            if (documento == null)
            {
                return (false, "Documento no encontrado", null);
            }

            if (!documento.FueUtilizadoEnSolicitudAprobada)
            {
                return (true, null, null);
            }

            var solicitudAprobada = documento.SolicitudAprobada;
            var motivoNoDisponible = solicitudAprobada != null 
                ? $"Este documento fue utilizado en la solicitud de ascenso aprobada el {solicitudAprobada.FechaAprobacion:dd/MM/yyyy} de {solicitudAprobada.NivelActual} a {solicitudAprobada.NivelSolicitado}"
                : "Este documento fue utilizado en una solicitud de ascenso aprobada anteriormente";

            return (false, motivoNoDisponible, documento.FechaUtilizacion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estado del documento {DocumentoId}", documentoId);
            return (false, "Error al verificar el estado del documento", null);
        }
    }

    /// <summary>
    /// Verifica si los documentos de una obra académica están disponibles para usar
    /// </summary>
    public async Task<(bool EstaDisponible, string? MotivoNoDisponible, DateTime? FechaUtilizacion)> VerificarDisponibilidadObraAsync(Guid solicitudId)
    {
        try
        {
            // Buscar el documento asociado a esta obra académica usando el patrón del nombre del archivo
            var solicitudIdCorto = solicitudId.ToString().Substring(0, 8);
            var patternStart = $"ObraAcademica_{solicitudIdCorto}";
            
            var documento = await _context.Documentos
                .Include(d => d.SolicitudAprobada)
                .ThenInclude(s => s!.Docente)
                .FirstOrDefaultAsync(d => d.TipoDocumento == TipoDocumento.ObrasAcademicas 
                    && d.NombreArchivo.StartsWith(patternStart));

            if (documento == null)
            {
                return (true, null, null); // No hay documento convertido, está disponible
            }

            // Verificar si el documento encontrado fue utilizado
            if (!documento.FueUtilizadoEnSolicitudAprobada)
            {
                return (true, null, null); // Documento existe pero no fue utilizado
            }

            return await ObtenerEstadoDocumentoAsync(documento.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar disponibilidad de obra académica {SolicitudId}", solicitudId);
            return (false, "Error al verificar la disponibilidad", null);
        }
    }

    /// <summary>
    /// Verifica si los documentos de un certificado de capacitación están disponibles para usar
    /// </summary>
    public async Task<(bool EstaDisponible, string? MotivoNoDisponible, DateTime? FechaUtilizacion)> VerificarDisponibilidadCertificadoAsync(Guid certificadoId)
    {
        try
        {
            // Buscar el documento asociado a este certificado usando el patrón del nombre del archivo
            var certificadoIdCorto = certificadoId.ToString().Substring(0, 8);
            var patternStart = $"CertificadoCapacitacion_{certificadoIdCorto}";
            
            var documento = await _context.Documentos
                .Include(d => d.SolicitudAprobada)
                .ThenInclude(s => s!.Docente)
                .FirstOrDefaultAsync(d => d.TipoDocumento == TipoDocumento.CertificadosCapacitacion 
                    && d.NombreArchivo.StartsWith(patternStart));

            if (documento == null)
            {
                return (true, null, null); // No hay documento convertido, está disponible
            }

            // Verificar si el documento encontrado fue utilizado
            if (!documento.FueUtilizadoEnSolicitudAprobada)
            {
                return (true, null, null); // Documento existe pero no fue utilizado
            }

            return await ObtenerEstadoDocumentoAsync(documento.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar disponibilidad de certificado {CertificadoId}", certificadoId);
            return (false, "Error al verificar la disponibilidad", null);
        }
    }

    /// <summary>
    /// Verifica si los documentos de una evidencia de investigación están disponibles para usar
    /// </summary>
    public async Task<(bool EstaDisponible, string? MotivoNoDisponible, DateTime? FechaUtilizacion)> VerificarDisponibilidadEvidenciaAsync(Guid evidenciaId)
    {
        try
        {
            // Buscar el documento asociado a esta evidencia usando el patrón del nombre del archivo
            var evidenciaIdCorto = evidenciaId.ToString().Substring(0, 8);
            var patternStart = $"EvidenciaInvestigacion_{evidenciaIdCorto}";
            
            var documento = await _context.Documentos
                .Include(d => d.SolicitudAprobada)
                .ThenInclude(s => s!.Docente)
                .FirstOrDefaultAsync(d => d.TipoDocumento == TipoDocumento.CertificadoInvestigacion 
                    && d.NombreArchivo.StartsWith(patternStart));

            if (documento == null)
            {
                return (true, null, null); // No hay documento convertido, está disponible
            }

            // Verificar si el documento encontrado fue utilizado
            if (!documento.FueUtilizadoEnSolicitudAprobada)
            {
                return (true, null, null); // Documento existe pero no fue utilizado
            }

            return await ObtenerEstadoDocumentoAsync(documento.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar disponibilidad de evidencia {EvidenciaId}", evidenciaId);
            return (false, "Error al verificar la disponibilidad", null);
        }
    }
}
