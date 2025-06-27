namespace SGA.Application.DTOs;

public class DocumentoDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string TipoContenido { get; set; } = string.Empty;
    public long Tamaño { get; set; }
    public DateTime FechaSubida { get; set; }
    public Guid SolicitudAscensoId { get; set; }
    public Guid SubidoPorId { get; set; }
    public string? SubidoPor { get; set; }
    public Domain.Enums.TipoDocumento Tipo { get; set; }
    
    // Propiedades calculadas
    public string TamañoFormateado => FormatearTamaño(Tamaño);
    public bool EsPdf => TipoContenido.Contains("pdf", StringComparison.OrdinalIgnoreCase);
    
    private static string FormatearTamaño(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024:F1} KB";
        return $"{bytes / (1024 * 1024):F1} MB";
    }
}

public class CreateDocumentoDto
{
    public string NombreArchivo { get; set; } = string.Empty;
    public string TipoContenido { get; set; } = string.Empty;
    public byte[] Contenido { get; set; } = Array.Empty<byte>();
    public Guid SolicitudAscensoId { get; set; }
    public Guid SubidoPorId { get; set; }
}

public class UploadDocumentoDto
{
    public Guid SolicitudId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public byte[] Contenido { get; set; } = Array.Empty<byte>();
    public Domain.Enums.TipoDocumento Tipo { get; set; }
}

public class DownloadDocumentoDto
{
    public string Nombre { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public byte[] Contenido { get; set; } = Array.Empty<byte>();
}
