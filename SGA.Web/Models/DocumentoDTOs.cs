namespace SGA.Web.Models;

/// <summary>
/// DTO para documentos utilizados en las páginas de solicitudes
/// </summary>
public class DocumentoDto
{
    public Guid Id { get; set; }
    public string NombreArchivo { get; set; } = string.Empty;
    public long TamanoArchivo { get; set; }
    public string TipoDocumento { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    
    // Propiedades adicionales para compatibilidad con versiones anteriores
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string RutaArchivo { get; set; } = string.Empty;
    public int TipoDocumentoId { get; set; }
    public TipoDocumentoDto? TipoDocumento_Legacy { get; set; }
    public bool Verificado { get; set; }
    
    // Propiedades calculadas
    public string TamanoFormateado => FormatearTamano(TamanoArchivo);
    public bool EsPdf => TipoDocumento.Contains("pdf", StringComparison.OrdinalIgnoreCase) || 
                         NombreArchivo.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);
    
    private static string FormatearTamano(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024:F1} KB";
        return $"{bytes / (1024 * 1024):F1} MB";
    }
}

/// <summary>
/// DTO para solicitudes de ascenso utilizadas en las vistas
/// </summary>
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

/// <summary>
/// Resultado de operaciones de visualización de documentos
/// </summary>
public class DocumentViewResult
{
    public bool Success { get; set; }
    public string? PdfUrl { get; set; }
    public string? FileName { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Datos de solicitud para operaciones de cancelación
/// </summary>
public class SolicitudData
{
    public Guid Id { get; set; }
    public string NivelActual { get; set; } = string.Empty;
    public string NivelSolicitado { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaSolicitud { get; set; }
}
