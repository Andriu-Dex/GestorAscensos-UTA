namespace SGA.Application.DTOs.Solicitudes;

public class SolicitudAscensoDto
{
    public Guid Id { get; set; }
    public Guid DocenteId { get; set; }
    public string DocenteNombre { get; set; } = string.Empty;
    public string DocenteNombres { get; set; } = string.Empty;
    public string DocenteApellidos { get; set; } = string.Empty;
    public string DocenteEmail { get; set; } = string.Empty;
    public string DocenteCedula { get; set; } = string.Empty;
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
    public int NivelActual { get; set; }
    public int NivelSolicitado { get; set; }
    public int TiempoRol { get; set; }
    public int NumeroObras { get; set; }
    public decimal PuntajeEvaluacion { get; set; }
    public int HorasCapacitacion { get; set; }
    public int TiempoInvestigacion { get; set; }
    public bool CumpleTiempoRol { get; set; }
    public bool CumpleObras { get; set; }
    public bool CumpleEvaluacion { get; set; }
    public bool CumpleCapacitacion { get; set; }
    public bool CumpleInvestigacion { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    
    // Documentos seleccionados por tipo para conversión automática
    public Dictionary<string, List<string>> DocumentosSeleccionados { get; set; } = new();
    
    // Lista de IDs de documentos genéricos (legacy - mantener para compatibilidad)
    public List<Guid> DocumentosIds { get; set; } = new List<Guid>();
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
    public string Nombre { get; set; } = string.Empty;
    public string NombreArchivo { get; set; } = string.Empty;
    public long TamanoArchivo { get; set; }
    public string TipoDocumento { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}

public class TipoDocumentoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class DocumentoUploadDto
{
    public string NombreArchivo { get; set; } = string.Empty;
    public string TipoDocumento { get; set; } = string.Empty;
    public byte[] ContenidoArchivo { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = string.Empty;
}
