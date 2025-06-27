using SGA.Application.DTOs;
using SGA.Application.DTOs.Documentos;
using SGA.Application.Interfaces;
using SGA.Application.Interfaces.Repositories;
using SGA.Domain.Entities;
using SGA.Domain.Enums;

namespace SGA.Application.Services;

public class DocumentoService : IDocumentoService
{
    private readonly IDocumentoRepository _documentoRepository;

    public DocumentoService(IDocumentoRepository documentoRepository)
    {
        _documentoRepository = documentoRepository;
    }

    public async Task<Guid> SubirDocumentoAsync(Guid solicitudId, SubirDocumentoRequestDto documento)
    {
        if (!await ValidarDocumentoAsync(documento))
            throw new ArgumentException("Archivo PDF inválido");

        var contenidoComprimido = await ComprimirPDFAsync(documento.Contenido);

        var doc = new Documento
        {
            SolicitudAscensoId = solicitudId,
            NombreArchivo = documento.Nombre,
            TamanoArchivo = contenidoComprimido.Length,
            TipoDocumento = documento.Tipo,
            ContenidoArchivo = contenidoComprimido,
            ContentType = "application/pdf",
            RutaArchivo = $"/documentos/{solicitudId}/{documento.Nombre}"
        };

        doc = await _documentoRepository.CreateAsync(doc);
        
        return doc.Id;
    }

    public async Task<byte[]?> DescargarDocumentoAsync(Guid documentoId)
    {
        var documento = await _documentoRepository.GetByIdAsync(documentoId);
        return documento?.ContenidoArchivo;
    }

    public async Task<bool> EliminarDocumentoAsync(Guid documentoId)
    {
        return await _documentoRepository.DeleteAsync(documentoId);
    }

    public async Task<bool> ValidarDocumentoAsync(SubirDocumentoRequestDto documento)
    {
        await Task.CompletedTask;
        
        // Validar tipo MIME
        if (!documento.TipoContenido.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            return false;
            
        // Validar tamaño mínimo
        if (documento.Contenido.Length < 4)
            return false;

        // Validar tamaño máximo (10MB)
        if (documento.Contenido.Length > 10 * 1024 * 1024)
            return false;

        // Validar header PDF
        var header = documento.Contenido.Take(4).ToArray();
        var pdfHeader = new byte[] { 0x25, 0x50, 0x44, 0x46 }; // %PDF
        
        if (!header.SequenceEqual(pdfHeader))
            return false;
            
        // Validación de estructura PDF
        try
        {
            using (var stream = new MemoryStream(documento.Contenido))
            {
                var reader = new iText.Kernel.Pdf.PdfReader(stream);
                var document = new iText.Kernel.Pdf.PdfDocument(reader);
                
                // Verificar que tenga al menos una página
                bool isValid = document.GetNumberOfPages() >= 1;
                
                document.Close();
                reader.Close();
                
                return isValid;
            }
        }
        catch
        {
            // Si hay alguna excepción, el PDF no es válido
            return false;
        }
    }

    public async Task<byte[]> ComprimirPDFAsync(byte[] contenidoPdf)
    {
        // No comprimimos si el archivo es pequeño
        if (contenidoPdf.Length < 1 * 1024 * 1024) // < 1MB
            return await Task.FromResult(contenidoPdf);
            
        try
        {
            using (var inputStream = new MemoryStream(contenidoPdf))
            using (var outputStream = new MemoryStream())
            {
                // Configurar lector y escritor de PDF
                var reader = new iText.Kernel.Pdf.PdfReader(inputStream);
                var writer = new iText.Kernel.Pdf.PdfWriter(outputStream);
                
                // Configurar opciones de compresión
                var writerProperties = new iText.Kernel.Pdf.WriterProperties()
                    .SetCompressionLevel(9) // Nivel máximo de compresión
                    .UseSmartMode();
                
                // Crear documento comprimido
                var pdfDoc = new iText.Kernel.Pdf.PdfDocument(reader, writer, new iText.Kernel.Pdf.StampingProperties());
                
                // Comprimir imágenes en el PDF
                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    var page = pdfDoc.GetPage(i);
                    var resources = page.GetResources();
                    var resourceNames = resources.GetResourceNames();
                    
                    foreach (var name in resourceNames)
                    {
                        var resource = resources.GetResource(name);
                        if (resource is iText.Kernel.Pdf.PdfStream stream)
                        {
                            if (stream.GetAsName(iText.Kernel.Pdf.PdfName.Subtype) == iText.Kernel.Pdf.PdfName.Image)
                            {
                                // Optimizar imágenes
                                stream.SetCompressionLevel(9);
                            }
                        }
                    }
                }
                
                pdfDoc.Close();
                
                // Verificar si logramos comprimir el archivo
                var compressedBytes = outputStream.ToArray();
                if (compressedBytes.Length < contenidoPdf.Length)
                {
                    return await Task.FromResult(compressedBytes);
                }
            }
        }
        catch (Exception ex)
        {
            // Log error pero devolver original en caso de fallo
            Console.WriteLine($"Error al comprimir PDF: {ex.Message}");
        }
        
        return await Task.FromResult(contenidoPdf); // Devolver original si falló la compresión
    }
}
