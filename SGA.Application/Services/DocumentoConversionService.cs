using SGA.Application.Interfaces.Repositories;
using SGA.Application.Interfaces;
using SGA.Domain.Entities;
using SGA.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace SGA.Application.Services;

/// <summary>
/// Servicio para convertir diferentes tipos de solicitudes aprobadas en documentos genéricos
/// para asociar con solicitudes de ascenso
/// </summary>
public class DocumentoConversionService
{
    private readonly IDocumentoRepository _documentoRepository;
    private readonly IApplicationDbContext _context;

    public DocumentoConversionService(IDocumentoRepository documentoRepository, IApplicationDbContext context)
    {
        _documentoRepository = documentoRepository;
        _context = context;
    }

    public async Task<List<Guid>> ConvertirYCrearDocumentosAsync(Dictionary<string, List<string>> documentosSeleccionados)
    {
        var documentosCreados = new List<Guid>();

        // Procesar obras académicas
        if (documentosSeleccionados.ContainsKey("obras"))
        {
            foreach (var obraIdStr in documentosSeleccionados["obras"])
            {
                if (Guid.TryParse(obraIdStr, out var obraId))
                {
                    var documento = await ConvertirObraADocumentoAsync(obraId);
                    if (documento != null)
                    {
                        documentosCreados.Add(documento.Id);
                    }
                }
            }
        }

        // Procesar certificados de capacitación
        if (documentosSeleccionados.ContainsKey("certificados"))
        {
            foreach (var certIdStr in documentosSeleccionados["certificados"])
            {
                if (Guid.TryParse(certIdStr, out var certId))
                {
                    var documento = await ConvertirCertificadoADocumentoAsync(certId);
                    if (documento != null)
                    {
                        documentosCreados.Add(documento.Id);
                    }
                }
            }
        }

        // Procesar evidencias de investigación
        if (documentosSeleccionados.ContainsKey("evidencias"))
        {
            foreach (var evidenciaIdStr in documentosSeleccionados["evidencias"])
            {
                if (Guid.TryParse(evidenciaIdStr, out var evidenciaId))
                {
                    var documento = await ConvertirEvidenciaADocumentoAsync(evidenciaId);
                    if (documento != null)
                    {
                        documentosCreados.Add(documento.Id);
                    }
                }
            }
        }

        return documentosCreados;
    }

    private async Task<Documento?> ConvertirObraADocumentoAsync(Guid solicitudId)
    {
        try
        {
            // Buscar la solicitud de obra académica aprobada
            var solicitud = await _context.SolicitudesObrasAcademicas
                .FirstOrDefaultAsync(s => s.Id == solicitudId && s.Estado == "Aprobada");
                
            if (solicitud == null)
            {
                return null;
            }
            
            // ✅ NUEVO: Verificar si esta obra ya fue utilizada en una solicitud de ascenso aprobada
            var solicitudIdCorto = solicitud.Id.ToString().Substring(0, 8);
            var patternStart = $"ObraAcademica_{solicitudIdCorto}";
            var documentoExistente = await _context.Documentos
                .FirstOrDefaultAsync(d => d.FueUtilizadoEnSolicitudAprobada && 
                                         d.TipoDocumento == TipoDocumento.ObrasAcademicas &&
                                         d.DocenteId == solicitud.DocenteId &&
                                         d.NombreArchivo.StartsWith(patternStart));
            
            if (documentoExistente != null)
            {
                // Este documento ya fue utilizado en una solicitud aprobada
                return null;
            }
            
            // Obtener contenido del archivo (BD preferido, fallback a archivo físico)
            byte[] contenidoArchivo;
            long tamanoArchivo;
            
            if (solicitud.ArchivoContenido != null && solicitud.ArchivoContenido.Length > 0)
            {
                // Usar archivo de BD (migración completa)
                contenidoArchivo = solicitud.ArchivoContenido;
                tamanoArchivo = solicitud.ArchivoTamano ?? contenidoArchivo.Length;
            }
            else if (!string.IsNullOrEmpty(solicitud.ArchivoRuta))
            {
                // Fallback a archivo físico (compatibilidad)
                try
                {
                    contenidoArchivo = await File.ReadAllBytesAsync(solicitud.ArchivoRuta);
                    tamanoArchivo = contenidoArchivo.Length;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            
            var documento = new Documento
            {
                NombreArchivo = $"ObraAcademica_{solicitudIdCorto}_DocId_{Guid.NewGuid().ToString()[..8]}.pdf",
                RutaArchivo = $"solicitud_ascenso/obra_{solicitudId}",
                TamanoArchivo = tamanoArchivo,
                TipoDocumento = TipoDocumento.ObrasAcademicas,
                ContenidoArchivo = contenidoArchivo,
                ContentType = solicitud.ArchivoTipo ?? "application/pdf",
                DocenteId = solicitud.DocenteId
            };

            var resultado = await _documentoRepository.CreateAsync(documento);
            
            return resultado;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task<Documento?> ConvertirCertificadoADocumentoAsync(Guid certificadoId)
    {
        try
        {
            // Buscar la solicitud de certificado de capacitación aprobada
            var solicitudCertificado = await _context.SolicitudesCertificadosCapacitacion
                .FirstOrDefaultAsync(c => c.Id == certificadoId && c.Estado == "Aprobada");
                
            if (solicitudCertificado == null)
            {
                return null;
            }
            
            // ✅ NUEVO: Verificar si este certificado ya fue utilizado en una solicitud de ascenso aprobada
            var certificadoIdCorto = solicitudCertificado.Id.ToString().Substring(0, 8);
            var patternStart = $"CertificadoCapacitacion_{certificadoIdCorto}";
            var documentoExistente = await _context.Documentos
                .FirstOrDefaultAsync(d => d.FueUtilizadoEnSolicitudAprobada && 
                                         d.TipoDocumento == TipoDocumento.CertificadosCapacitacion &&
                                         d.DocenteId == solicitudCertificado.DocenteId &&
                                         d.NombreArchivo.StartsWith(patternStart));
            
            if (documentoExistente != null)
            {
                // Este documento ya fue utilizado en una solicitud aprobada
                return null;
            }
            
            // Verificar que tenga archivo
            if (solicitudCertificado.ArchivoContenido == null || solicitudCertificado.ArchivoContenido.Length == 0)
            {
                return null;
            }
            
            var documento = new Documento
            {
                NombreArchivo = $"CertificadoCapacitacion_{certificadoIdCorto}_DocId_{Guid.NewGuid().ToString()[..8]}.pdf",
                RutaArchivo = $"solicitud_ascenso/certificado_{certificadoId}",
                TamanoArchivo = solicitudCertificado.ArchivoTamano ?? solicitudCertificado.ArchivoContenido.Length,
                TipoDocumento = TipoDocumento.CertificadosCapacitacion,
                ContenidoArchivo = solicitudCertificado.ArchivoContenido,
                ContentType = solicitudCertificado.ArchivoTipo ?? "application/pdf",
                DocenteId = solicitudCertificado.DocenteId // Asignar el DocenteId del certificado
            };

            var resultado = await _documentoRepository.CreateAsync(documento);
            
            return resultado;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task<Documento?> ConvertirEvidenciaADocumentoAsync(Guid evidenciaId)
    {
        try
        {
            
            // Buscar la solicitud de evidencia de investigación aprobada
            var solicitudEvidencia = await _context.SolicitudesEvidenciasInvestigacion
                .FirstOrDefaultAsync(e => e.Id == evidenciaId && e.Estado == "Aprobada");
                
            if (solicitudEvidencia == null)
            {
                return null;
            }
            
            // ✅ NUEVO: Verificar si esta evidencia ya fue utilizada en una solicitud de ascenso aprobada
            var evidenciaIdCorto = solicitudEvidencia.Id.ToString().Substring(0, 8);
            var patternStart = $"EvidenciaInvestigacion_{evidenciaIdCorto}";
            var documentoExistente = await _context.Documentos
                .FirstOrDefaultAsync(d => d.FueUtilizadoEnSolicitudAprobada && 
                                         d.TipoDocumento == TipoDocumento.CertificadoInvestigacion &&
                                         d.DocenteId == solicitudEvidencia.DocenteId &&
                                         d.NombreArchivo.StartsWith(patternStart));
            
            if (documentoExistente != null)
            {
                // Este documento ya fue utilizado en una solicitud aprobada
                return null;
            }
            
            // Verificar que tenga archivo
            if (solicitudEvidencia.ArchivoContenido == null || solicitudEvidencia.ArchivoContenido.Length == 0)
            {
                return null;
            }
            
            var documento = new Documento
            {
                NombreArchivo = $"EvidenciaInvestigacion_{evidenciaIdCorto}_DocId_{Guid.NewGuid().ToString()[..8]}.pdf",
                RutaArchivo = $"solicitud_ascenso/evidencia_{evidenciaId}",
                TamanoArchivo = solicitudEvidencia.ArchivoTamano > 0 ? solicitudEvidencia.ArchivoTamano : solicitudEvidencia.ArchivoContenido.Length,
                TipoDocumento = TipoDocumento.CertificadoInvestigacion,
                ContenidoArchivo = solicitudEvidencia.ArchivoContenido,
                ContentType = !string.IsNullOrEmpty(solicitudEvidencia.ArchivoTipo) ? solicitudEvidencia.ArchivoTipo : "application/pdf",
                DocenteId = solicitudEvidencia.DocenteId // Asignar el DocenteId de la evidencia
            };

            var resultado = await _documentoRepository.CreateAsync(documento);
            
            return resultado;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
