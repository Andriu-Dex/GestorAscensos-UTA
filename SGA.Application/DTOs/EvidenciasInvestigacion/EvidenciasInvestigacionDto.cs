namespace SGA.Application.DTOs.EvidenciasInvestigacion;

/// <summary>
/// DTO para crear nueva evidencia de investigación
/// </summary>
public class CrearEvidenciaInvestigacionDto
{
    public string TipoEvidencia { get; set; } = string.Empty;
    public string TituloProyecto { get; set; } = string.Empty;
    public string InstitucionFinanciadora { get; set; } = string.Empty;
    public string RolInvestigador { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int MesesDuracion { get; set; }
    public string? CodigoProyecto { get; set; }
    public string? AreaTematica { get; set; }
    public string? Descripcion { get; set; }
    public string? ComentariosSolicitud { get; set; }
    
    // Archivo PDF
    public string ArchivoNombre { get; set; } = string.Empty;
    public string ArchivoContenido { get; set; } = string.Empty; // Base64
    public string ArchivoTipo { get; set; } = "application/pdf";
}

/// <summary>
/// DTO para solicitar múltiples evidencias de investigación
/// </summary>
public class SolicitarEvidenciasInvestigacionDto
{
    public List<CrearEvidenciaInvestigacionDto> Evidencias { get; set; } = new();
}

/// <summary>
/// DTO para mostrar detalle de evidencia de investigación
/// </summary>
public class EvidenciaInvestigacionDetalleDto
{
    public Guid Id { get; set; }
    public string TipoEvidencia { get; set; } = string.Empty;
    public string TituloProyecto { get; set; } = string.Empty;
    public string InstitucionFinanciadora { get; set; } = string.Empty;
    public string RolInvestigador { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int MesesDuracion { get; set; }
    public string? CodigoProyecto { get; set; }
    public string? AreaTematica { get; set; }
    public string? Descripcion { get; set; }
    public string ArchivoNombre { get; set; } = string.Empty;
    public long ArchivoTamano { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? ComentariosRevision { get; set; }
    public string? MotivoRechazo { get; set; }
    public DateTime? FechaRevision { get; set; }
    public string? ComentariosSolicitud { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaModificacion { get; set; }
}

/// <summary>
/// DTO para editar metadatos de evidencia de investigación
/// </summary>
public class EditarMetadatosEvidenciaDto
{
    public string TipoEvidencia { get; set; } = string.Empty;
    public string TituloProyecto { get; set; } = string.Empty;
    public string InstitucionFinanciadora { get; set; } = string.Empty;
    public string RolInvestigador { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int MesesDuracion { get; set; }
    public string? CodigoProyecto { get; set; }
    public string? AreaTematica { get; set; }
    public string? Descripcion { get; set; }
}

/// <summary>
/// DTO para reemplazar archivo de evidencia
/// </summary>
public class ReemplazarArchivoEvidenciaDto
{
    public string ArchivoNombre { get; set; } = string.Empty;
    public string ArchivoContenido { get; set; } = string.Empty; // Base64
    public string ArchivoTipo { get; set; } = "application/pdf";
}

/// <summary>
/// DTO de respuesta para solicitudes de evidencias
/// </summary>
public class ResponseEvidenciasInvestigacionDto
{
    public bool Exitoso { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public List<EvidenciaInvestigacionDetalleDto> Evidencias { get; set; } = new();
}

/// <summary>
/// DTO de respuesta genérica para evidencias
/// </summary>
public class ResponseGenericoEvidenciaDto
{
    public bool Exitoso { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public EvidenciaInvestigacionDetalleDto? Evidencia { get; set; }
}

/// <summary>
/// DTO para administradores - listado de evidencias
/// </summary>
public class SolicitudEvidenciaInvestigacionAdminDto
{
    public Guid Id { get; set; }
    public string DocenteNombre { get; set; } = string.Empty;
    public string DocenteCedula { get; set; } = string.Empty;
    public string TipoEvidencia { get; set; } = string.Empty;
    public string TituloProyecto { get; set; } = string.Empty;
    public string InstitucionFinanciadora { get; set; } = string.Empty;
    public string RolInvestigador { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int MesesDuracion { get; set; }
    public string? CodigoProyecto { get; set; }
    public string? AreaTematica { get; set; }
    public string? Descripcion { get; set; }
    public string ArchivoNombre { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string? ComentariosRevision { get; set; }
    public string? MotivoRechazo { get; set; }
    public string? ComentariosSolicitud { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaRevision { get; set; }
}

/// <summary>
/// DTO de respuesta para administradores
/// </summary>
public class ResponseSolicitudesEvidenciasAdminDto
{
    public bool Exitoso { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public List<SolicitudEvidenciaInvestigacionAdminDto> Solicitudes { get; set; } = new();
}

/// <summary>
/// DTO para revisión de solicitudes por administradores
/// </summary>
public class RevisionSolicitudEvidenciaDto
{
    public Guid SolicitudId { get; set; }
    public string Accion { get; set; } = string.Empty; // "Aprobar" o "Rechazar"
    public string? Comentarios { get; set; }
}
