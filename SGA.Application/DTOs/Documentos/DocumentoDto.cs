using SGA.Domain.Enums;

namespace SGA.Application.DTOs.Documentos;

public class DocumentoResponseDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public TipoDocumento Tipo { get; set; }
    public string RutaArchivo { get; set; } = string.Empty;
    public long TamanoArchivo { get; set; }
    public string TipoContenido { get; set; } = string.Empty;
    public DateTime FechaSubida { get; set; }
    public Guid SolicitudId { get; set; }
    public bool EsValido { get; set; }
    public string? Observaciones { get; set; }
    public DateTime? FechaValidacion { get; set; }
    public string? ValidadoPor { get; set; }
}

public class SubirDocumentoRequestDto
{
    public string Nombre { get; set; } = string.Empty;
    public TipoDocumento Tipo { get; set; }
    public Guid SolicitudId { get; set; }
    public byte[] Contenido { get; set; } = Array.Empty<byte>();
    public string TipoContenido { get; set; } = string.Empty;
}
