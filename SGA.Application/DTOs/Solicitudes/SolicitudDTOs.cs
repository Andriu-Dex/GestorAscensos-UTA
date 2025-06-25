namespace SGA.Application.DTOs.Solicitudes;

public class SolicitudAscensoDto
{
    public Guid Id { get; set; }
    public Guid DocenteId { get; set; }
    public string DocenteNombre { get; set; } = string.Empty;
    public string NivelActual { get; set; } = string.Empty;
    public string NivelSolicitado { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string? MotivoRechazo { get; set; }
    public DateTime FechaSolicitud { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public string? AprobadoPor { get; set; }
    
    // Datos al momento de la solicitud
    public decimal PromedioEvaluaciones { get; set; }
    public int HorasCapacitacion { get; set; }
    public int NumeroObrasAcademicas { get; set; }
    public int MesesInvestigacion { get; set; }
    public int TiempoEnNivelDias { get; set; }
    
    public List<DocumentoDto> Documentos { get; set; } = new();
}

public class CrearSolicitudRequest
{
    public string NivelSolicitado { get; set; } = string.Empty;
    public List<DocumentoUploadDto> Documentos { get; set; } = new();
}

public class ProcesarSolicitudRequest
{
    public Guid SolicitudId { get; set; }
    public bool Aprobar { get; set; }
    public string? MotivoRechazo { get; set; }
}

public class DocumentoDto
{
    public Guid Id { get; set; }
    public string NombreArchivo { get; set; } = string.Empty;
    public long TamanoArchivo { get; set; }
    public string TipoDocumento { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}

public class DocumentoUploadDto
{
    public string NombreArchivo { get; set; } = string.Empty;
    public string TipoDocumento { get; set; } = string.Empty;
    public byte[] ContenidoArchivo { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = string.Empty;
}
