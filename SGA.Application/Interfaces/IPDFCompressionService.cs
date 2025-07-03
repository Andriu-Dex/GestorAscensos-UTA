namespace SGA.Application.Interfaces;

/// <summary>
/// Servicio modular para compresión de archivos PDF
/// Centraliza toda la lógica de compresión para mantener consistencia en el sistema
/// </summary>
public interface IPDFCompressionService
{
    /// <summary>
    /// Comprime un archivo PDF si su tamaño supera el umbral mínimo
    /// </summary>
    /// <param name="contenidoPdf">Contenido original del PDF en bytes</param>
    /// <returns>Contenido comprimido del PDF</returns>
    Task<byte[]> ComprimirPDFAsync(byte[] contenidoPdf);
    
    /// <summary>
    /// Descomprime un archivo PDF previamente comprimido
    /// </summary>
    /// <param name="contenidoComprimido">Contenido comprimido del PDF</param>
    /// <returns>Contenido original del PDF</returns>
    Task<byte[]> DescomprimirPDFAsync(byte[] contenidoComprimido);
    
    /// <summary>
    /// Valida que un archivo sea un PDF válido
    /// </summary>
    /// <param name="contenido">Contenido del archivo a validar</param>
    /// <returns>True si es un PDF válido</returns>
    bool ValidarPDF(byte[] contenido);
    
    /// <summary>
    /// Obtiene información sobre el nivel de compresión aplicado
    /// </summary>
    /// <param name="contenidoOriginal">Contenido original</param>
    /// <param name="contenidoComprimido">Contenido comprimido</param>
    /// <returns>Información de compresión</returns>
    (double porcentajeCompresion, long tamahoOriginal, long tamahoComprimido) ObtenerEstadisticasCompresion(byte[] contenidoOriginal, byte[] contenidoComprimido);
}
