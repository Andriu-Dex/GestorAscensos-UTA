using SGA.Domain.Common;

namespace SGA.Domain.Entities;

/// <summary>
/// Entidad para almacenar obras académicas del docente con archivos PDF comprimidos
/// </summary>
public class ObraAcademica : BaseEntity
{
    public Guid DocenteId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string TipoObra { get; set; } = string.Empty;
    public DateTime FechaPublicacion { get; set; }
    public string? Editorial { get; set; }
    public string? Revista { get; set; }
    public string? ISBN_ISSN { get; set; }
    public string? DOI { get; set; }
    public bool EsIndexada { get; set; }
    public string? IndiceIndexacion { get; set; }
    public string? Autores { get; set; }
    public string? Descripcion { get; set; }
    
    // Archivo PDF comprimido
    public string? NombreArchivo { get; set; }
    public byte[]? ContenidoArchivoPDF { get; set; }
    public long? TamanoArchivo { get; set; }
    public string? ContentType { get; set; } = "application/pdf";
    
    // Origen de la obra
    public string OrigenDatos { get; set; } = "DIRINV"; // DIRINV, Manual, Solicitud
    public bool EsVerificada { get; set; } = true;
    
    // Relación
    public virtual Docente Docente { get; set; } = null!;
    
    /// <summary>
    /// Comprime y asigna un archivo PDF a la obra académica
    /// </summary>
    public async Task AsignarArchivoPDFAsync(byte[] contenidoPdf, string nombreArchivo)
    {
        if (contenidoPdf == null || contenidoPdf.Length == 0)
            return;
            
        NombreArchivo = nombreArchivo;
        ContenidoArchivoPDF = await ComprimirPDFAsync(contenidoPdf);
        TamanoArchivo = ContenidoArchivoPDF.Length;
        ContentType = "application/pdf";
    }
    
    /// <summary>
    /// Método para comprimir archivos PDF de manera eficiente
    /// </summary>
    private async Task<byte[]> ComprimirPDFAsync(byte[] contenidoPdf)
    {
        // Si el archivo es pequeño (< 1MB), no comprimimos
        if (contenidoPdf.Length < 1 * 1024 * 1024)
            return contenidoPdf;
            
        try
        {
            // Implementación básica de compresión usando System.IO.Compression
            using var inputStream = new MemoryStream(contenidoPdf);
            using var outputStream = new MemoryStream();
            using var compressionStream = new System.IO.Compression.GZipStream(outputStream, System.IO.Compression.CompressionLevel.Optimal);
            
            await inputStream.CopyToAsync(compressionStream);
            await compressionStream.FlushAsync();
            
            var compressedBytes = outputStream.ToArray();
            
            // Si la compresión no reduce significativamente el tamaño, devolver original
            return compressedBytes.Length < contenidoPdf.Length * 0.9 ? compressedBytes : contenidoPdf;
        }
        catch
        {
            // En caso de error, devolver contenido original
            return contenidoPdf;
        }
    }
    
    /// <summary>
    /// Descomprime el archivo PDF si está comprimido
    /// </summary>
    public async Task<byte[]?> ObtenerArchivoPDFAsync()
    {
        if (ContenidoArchivoPDF == null || ContenidoArchivoPDF.Length == 0)
            return null;
            
        try
        {
            // Verificar si el archivo está comprimido (magic number de GZip)
            if (ContenidoArchivoPDF.Length >= 3 && 
                ContenidoArchivoPDF[0] == 0x1F && 
                ContenidoArchivoPDF[1] == 0x8B && 
                ContenidoArchivoPDF[2] == 0x08)
            {
                // Descomprimir
                using var inputStream = new MemoryStream(ContenidoArchivoPDF);
                using var decompressionStream = new System.IO.Compression.GZipStream(inputStream, System.IO.Compression.CompressionMode.Decompress);
                using var outputStream = new MemoryStream();
                
                await decompressionStream.CopyToAsync(outputStream);
                return outputStream.ToArray();
            }
            else
            {
                // No está comprimido, devolver directamente
                return ContenidoArchivoPDF;
            }
        }
        catch
        {
            // En caso de error, devolver contenido original
            return ContenidoArchivoPDF;
        }
    }
}
