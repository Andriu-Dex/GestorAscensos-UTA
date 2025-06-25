using SGA.Domain.Common;
using SGA.Domain.Enums;

namespace SGA.Domain.Entities;

public class Documento : BaseEntity
{
    public string NombreArchivo { get; set; } = string.Empty;
    public string RutaArchivo { get; set; } = string.Empty;
    public long TamanoArchivo { get; set; }
    public TipoDocumento TipoDocumento { get; set; }
    public byte[] ContenidoArchivo { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = string.Empty;
    
    // Relación con SolicitudAscenso
    public Guid SolicitudAscensoId { get; set; }
    public virtual SolicitudAscenso SolicitudAscenso { get; set; } = null!;
}
