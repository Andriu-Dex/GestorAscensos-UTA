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

        Console.WriteLine($"[DocumentoConversion] Iniciando conversión de documentos");

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
                        Console.WriteLine($"[DocumentoConversion] Obra {obraId} convertida a documento {documento.Id}");
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
                        Console.WriteLine($"[DocumentoConversion] Certificado {certId} convertido a documento {documento.Id}");
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
                        Console.WriteLine($"[DocumentoConversion] Evidencia {evidenciaId} convertida a documento {documento.Id}");
                    }
                }
            }
        }

        Console.WriteLine($"[DocumentoConversion] Conversión completada: {documentosCreados.Count} documentos creados");
        return documentosCreados;
    }

    private async Task<Documento?> ConvertirObraADocumentoAsync(Guid obraId)
    {
        try
        {
            Console.WriteLine($"[DocumentoConversion] Convirtiendo obra académica {obraId}");
            
            // Buscar la obra académica directamente (no la solicitud)
            var obra = await _context.ObrasAcademicas
                .FirstOrDefaultAsync(o => o.Id == obraId && o.EsVerificada == true);
                
            if (obra == null)
            {
                Console.WriteLine($"[DocumentoConversion] Obra académica {obraId} no encontrada o no verificada");
                return null;
            }
            
            // Verificar que tenga archivo
            if (obra.ContenidoArchivoPDF == null || obra.ContenidoArchivoPDF.Length == 0)
            {
                Console.WriteLine($"[DocumentoConversion] Obra académica {obraId} no tiene archivo adjunto");
                return null;
            }
            
            var documento = new Documento
            {
                NombreArchivo = obra.NombreArchivo ?? $"obra_{obra.Titulo?.Replace(" ", "_") ?? obraId.ToString()[..8]}.pdf",
                RutaArchivo = $"solicitud_ascenso/obra_{obraId}",
                TamanoArchivo = obra.TamanoArchivo ?? obra.ContenidoArchivoPDF.Length,
                TipoDocumento = TipoDocumento.ObrasAcademicas,
                ContenidoArchivo = obra.ContenidoArchivoPDF,
                ContentType = obra.ContentType ?? "application/pdf",
                DocenteId = obra.DocenteId // Asignar el DocenteId de la obra
            };

            Console.WriteLine($"[DocumentoConversion] Creando documento: {documento.NombreArchivo}, Tamaño: {documento.TamanoArchivo}");
            var resultado = await _documentoRepository.CreateAsync(documento);
            Console.WriteLine($"[DocumentoConversion] Documento creado con ID: {resultado.Id}");
            
            return resultado;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DocumentoConversion] Error convirtiendo obra {obraId}: {ex.Message}");
            Console.WriteLine($"[DocumentoConversion] StackTrace: {ex.StackTrace}");
            return null;
        }
    }

    private async Task<Documento?> ConvertirCertificadoADocumentoAsync(Guid certificadoId)
    {
        try
        {
            Console.WriteLine($"[DocumentoConversion] Convirtiendo certificado de capacitación {certificadoId}");
            
            // Buscar la solicitud de certificado de capacitación aprobada
            var solicitudCertificado = await _context.SolicitudesCertificadosCapacitacion
                .FirstOrDefaultAsync(c => c.Id == certificadoId && c.Estado == "Aprobada");
                
            if (solicitudCertificado == null)
            {
                Console.WriteLine($"[DocumentoConversion] Solicitud de certificado {certificadoId} no encontrada o no aprobada");
                return null;
            }
            
            // Verificar que tenga archivo
            if (solicitudCertificado.ArchivoContenido == null || solicitudCertificado.ArchivoContenido.Length == 0)
            {
                Console.WriteLine($"[DocumentoConversion] Solicitud de certificado {certificadoId} no tiene archivo adjunto");
                return null;
            }
            
            var documento = new Documento
            {
                NombreArchivo = solicitudCertificado.ArchivoNombre ?? $"certificado_{solicitudCertificado.NombreCurso?.Replace(" ", "_") ?? certificadoId.ToString()[..8]}.pdf",
                RutaArchivo = $"solicitud_ascenso/certificado_{certificadoId}",
                TamanoArchivo = solicitudCertificado.ArchivoTamano ?? solicitudCertificado.ArchivoContenido.Length,
                TipoDocumento = TipoDocumento.CertificadosCapacitacion,
                ContenidoArchivo = solicitudCertificado.ArchivoContenido,
                ContentType = solicitudCertificado.ArchivoTipo ?? "application/pdf",
                DocenteId = solicitudCertificado.DocenteId // Asignar el DocenteId del certificado
            };

            Console.WriteLine($"[DocumentoConversion] Creando documento certificado: {documento.NombreArchivo}, Tamaño: {documento.TamanoArchivo}");
            var resultado = await _documentoRepository.CreateAsync(documento);
            Console.WriteLine($"[DocumentoConversion] Documento certificado creado con ID: {resultado.Id}");
            
            return resultado;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DocumentoConversion] Error convirtiendo certificado {certificadoId}: {ex.Message}");
            Console.WriteLine($"[DocumentoConversion] StackTrace: {ex.StackTrace}");
            return null;
        }
    }

    private async Task<Documento?> ConvertirEvidenciaADocumentoAsync(Guid evidenciaId)
    {
        try
        {
            Console.WriteLine($"[DocumentoConversion] Convirtiendo evidencia de investigación {evidenciaId}");
            
            // Buscar la solicitud de evidencia de investigación aprobada
            var solicitudEvidencia = await _context.SolicitudesEvidenciasInvestigacion
                .FirstOrDefaultAsync(e => e.Id == evidenciaId && e.Estado == "Aprobada");
                
            if (solicitudEvidencia == null)
            {
                Console.WriteLine($"[DocumentoConversion] Solicitud de evidencia {evidenciaId} no encontrada o no aprobada");
                return null;
            }
            
            // Verificar que tenga archivo
            if (solicitudEvidencia.ArchivoContenido == null || solicitudEvidencia.ArchivoContenido.Length == 0)
            {
                Console.WriteLine($"[DocumentoConversion] Solicitud de evidencia {evidenciaId} no tiene archivo adjunto");
                return null;
            }
            
            var documento = new Documento
            {
                NombreArchivo = !string.IsNullOrEmpty(solicitudEvidencia.ArchivoNombre) ? solicitudEvidencia.ArchivoNombre : $"evidencia_{solicitudEvidencia.TipoEvidencia?.Replace(" ", "_") ?? evidenciaId.ToString()[..8]}.pdf",
                RutaArchivo = $"solicitud_ascenso/evidencia_{evidenciaId}",
                TamanoArchivo = solicitudEvidencia.ArchivoTamano > 0 ? solicitudEvidencia.ArchivoTamano : solicitudEvidencia.ArchivoContenido.Length,
                TipoDocumento = TipoDocumento.CertificadoInvestigacion,
                ContenidoArchivo = solicitudEvidencia.ArchivoContenido,
                ContentType = !string.IsNullOrEmpty(solicitudEvidencia.ArchivoTipo) ? solicitudEvidencia.ArchivoTipo : "application/pdf",
                DocenteId = solicitudEvidencia.DocenteId // Asignar el DocenteId de la evidencia
            };

            Console.WriteLine($"[DocumentoConversion] Creando documento evidencia: {documento.NombreArchivo}, Tamaño: {documento.TamanoArchivo}");
            var resultado = await _documentoRepository.CreateAsync(documento);
            Console.WriteLine($"[DocumentoConversion] Documento evidencia creado con ID: {resultado.Id}");
            
            return resultado;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DocumentoConversion] Error convirtiendo evidencia {evidenciaId}: {ex.Message}");
            Console.WriteLine($"[DocumentoConversion] StackTrace: {ex.StackTrace}");
            return null;
        }
    }
}
