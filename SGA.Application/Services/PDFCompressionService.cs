using Microsoft.Extensions.Logging;
using SGA.Application.Interfaces;
using System.IO.Compression;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace SGA.Application.Services;

public class PDFCompressionService : IPDFCompressionService
{
    private readonly ILogger<PDFCompressionService> _logger;
    
    // Tamaño mínimo para aplicar compresión (1MB)
    private const int TAMAÑO_MINIMO_COMPRESION = 1 * 1024 * 1024;
    
    // Tamaño máximo permitido para PDFs (10MB)
    private const int TAMAÑO_MAXIMO_PDF = 10 * 1024 * 1024;

    public PDFCompressionService(ILogger<PDFCompressionService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Comprime un archivo PDF usando múltiples estrategias según el tamaño
    /// </summary>
    public async Task<byte[]> ComprimirPDFAsync(byte[] contenidoPdf)
    {
        if (contenidoPdf == null || contenidoPdf.Length == 0)
        {
            _logger.LogWarning("Se intentó comprimir un archivo PDF vacío o nulo");
            return contenidoPdf ?? Array.Empty<byte>();
        }

        // Validar tamaño máximo
        if (contenidoPdf.Length > TAMAÑO_MAXIMO_PDF)
        {
            throw new ArgumentException($"El archivo PDF excede el tamaño máximo permitido de {TAMAÑO_MAXIMO_PDF / (1024 * 1024)}MB");
        }

        // No comprimimos archivos pequeños
        if (contenidoPdf.Length < TAMAÑO_MINIMO_COMPRESION)
        {
            _logger.LogDebug("Archivo PDF es menor a {SizeLimit}MB, no se aplica compresión", TAMAÑO_MINIMO_COMPRESION / (1024 * 1024));
            return contenidoPdf;
        }

        try
        {
            _logger.LogInformation("Iniciando compresión de PDF de {OriginalSize} bytes", contenidoPdf.Length);

            // Estrategia 1: Compresión usando iText7 (más efectiva para PDFs con imágenes)
            var resultadoIText = await ComprimirConITextAsync(contenidoPdf);
            
            // Estrategia 2: Compresión GZip (más efectiva para PDFs con mucho texto)
            var resultadoGZip = await ComprimirConGZipAsync(contenidoPdf);
            
            // Seleccionar el mejor resultado
            byte[] mejorResultado;
            string metodoUsado;
            
            if (resultadoIText.Length < resultadoGZip.Length && resultadoIText.Length < contenidoPdf.Length * 0.9)
            {
                mejorResultado = resultadoIText;
                metodoUsado = "iText7";
            }
            else if (resultadoGZip.Length < contenidoPdf.Length * 0.9)
            {
                mejorResultado = resultadoGZip;
                metodoUsado = "GZip";
            }
            else
            {
                // Si ninguna compresión es efectiva, devolver original
                mejorResultado = contenidoPdf;
                metodoUsado = "Sin compresión";
            }

            _logger.LogInformation("Compresión completada. Método: {Method}, Tamaño original: {OriginalSize} bytes, Tamaño final: {FinalSize} bytes, Reducción: {Reduction:P2}",
                metodoUsado, contenidoPdf.Length, mejorResultado.Length, 
                1.0 - (double)mejorResultado.Length / contenidoPdf.Length);

            return mejorResultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante la compresión de PDF, devolviendo archivo original");
            return contenidoPdf;
        }
    }

    /// <summary>
    /// Descomprime un archivo PDF usando la estrategia detectada automáticamente
    /// </summary>
    public async Task<byte[]> DescomprimirPDFAsync(byte[] contenidoComprimido)
    {
        if (contenidoComprimido == null || contenidoComprimido.Length == 0)
        {
            return contenidoComprimido ?? Array.Empty<byte>();
        }

        try
        {
            // Detectar si está comprimido con GZip (magic numbers: 0x1F, 0x8B)
            if (contenidoComprimido.Length >= 3 && 
                contenidoComprimido[0] == 0x1F && 
                contenidoComprimido[1] == 0x8B && 
                contenidoComprimido[2] == 0x08)
            {
                _logger.LogDebug("Descomprimiendo PDF con GZip");
                return await DescomprimirGZipAsync(contenidoComprimido);
            }
            
            // Si no está comprimido con GZip, verificar si es PDF válido
            if (ValidarPDF(contenidoComprimido))
            {
                _logger.LogDebug("Archivo ya está descomprimido");
                return contenidoComprimido;
            }

            _logger.LogWarning("No se pudo detectar el método de compresión, devolviendo contenido original");
            return contenidoComprimido;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al descomprimir PDF, devolviendo contenido original");
            return contenidoComprimido;
        }
    }

    /// <summary>
    /// Valida que un archivo sea un PDF válido verificando sus magic bytes
    /// </summary>
    public bool ValidarPDF(byte[] contenido)
    {
        if (contenido == null || contenido.Length < 4)
        {
            return false;
        }

        // Verificar magic bytes del PDF: %PDF
        return contenido[0] == 0x25 && // %
               contenido[1] == 0x50 && // P
               contenido[2] == 0x44 && // D
               contenido[3] == 0x46;   // F
    }

    /// <summary>
    /// Compresión usando iText7 para optimizar PDFs con imágenes y contenido complejo
    /// </summary>
    private async Task<byte[]> ComprimirConITextAsync(byte[] contenidoPdf)
    {
        try
        {
            using var inputStream = new MemoryStream(contenidoPdf);
            using var outputStream = new MemoryStream();
            
            var reader = new PdfReader(inputStream);
            
            // Configurar opciones de compresión máxima
            var writerProperties = new WriterProperties()
                .SetCompressionLevel(9)
                .UseSmartMode();
                
            var writer = new PdfWriter(outputStream, writerProperties);
            
            var pdfDoc = new PdfDocument(reader, writer);
            
            // Cerrar el documento para aplicar la compresión
            pdfDoc.Close();
            
            return await Task.FromResult(outputStream.ToArray());
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error en compresión iText7, fallback a GZip");
            return await ComprimirConGZipAsync(contenidoPdf);
        }
    }

    /// <summary>
    /// Compresión usando GZip para archivos con mucho contenido textual
    /// </summary>
    private async Task<byte[]> ComprimirConGZipAsync(byte[] contenidoPdf)
    {
        using var inputStream = new MemoryStream(contenidoPdf);
        using var outputStream = new MemoryStream();
        using var compressionStream = new System.IO.Compression.GZipStream(outputStream, System.IO.Compression.CompressionLevel.Optimal);
        
        await inputStream.CopyToAsync(compressionStream);
        await compressionStream.FlushAsync();
        
        return outputStream.ToArray();
    }

    /// <summary>
    /// Descompresión usando GZip
    /// </summary>
    private async Task<byte[]> DescomprimirGZipAsync(byte[] contenidoComprimido)
    {
        using var inputStream = new MemoryStream(contenidoComprimido);
        using var decompressionStream = new System.IO.Compression.GZipStream(inputStream, System.IO.Compression.CompressionMode.Decompress);
        using var outputStream = new MemoryStream();
        
        await decompressionStream.CopyToAsync(outputStream);
        return outputStream.ToArray();
    }

    /// <summary>
    /// Obtiene información sobre el nivel de compresión aplicado
    /// </summary>
    /// <param name="contenidoOriginal">Contenido original</param>
    /// <param name="contenidoComprimido">Contenido comprimido</param>
    /// <returns>Información de compresión</returns>
    public (double porcentajeCompresion, long tamahoOriginal, long tamahoComprimido) ObtenerEstadisticasCompresion(byte[] contenidoOriginal, byte[] contenidoComprimido)
    {
        var tamahoOriginal = (long)contenidoOriginal.Length;
        var tamahoComprimido = (long)contenidoComprimido.Length;
        var porcentajeCompresion = tamahoOriginal > 0 ? ((double)(tamahoOriginal - tamahoComprimido) / tamahoOriginal) * 100 : 0;
        
        return (porcentajeCompresion, tamahoOriginal, tamahoComprimido);
    }
}
