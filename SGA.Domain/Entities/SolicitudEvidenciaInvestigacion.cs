using SGA.Domain.Common;

namespace SGA.Domain.Entities;

/// <summary>
/// Entidad para gestión de evidencias de investigación subidas por docentes
/// Sigue el mismo patrón que SolicitudCertificadoCapacitacion
/// </summary>
public class SolicitudEvidenciaInvestigacion : BaseEntity
{
    public Guid DocenteId { get; set; }
    public string DocenteCedula { get; set; } = string.Empty;
    
    // Información de la evidencia
    public string TipoEvidencia { get; set; } = string.Empty; // "Proyecto", "Publicación", "Participación", "Dirección", "Colaboración"
    public string TituloProyecto { get; set; } = string.Empty;
    public string InstitucionFinanciadora { get; set; } = string.Empty;
    public string RolInvestigador { get; set; } = string.Empty; // "Director", "Investigador Principal", "Co-investigador", "Colaborador"
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int MesesDuracion { get; set; }
    public string? CodigoProyecto { get; set; }
    public string? AreaTematica { get; set; }
    public string? Descripcion { get; set; }
    
    // Gestión del archivo PDF almacenado en base de datos
    public string ArchivoNombre { get; set; } = string.Empty;
    public byte[] ArchivoContenido { get; set; } = Array.Empty<byte>(); // PDF comprimido almacenado en BD
    public long ArchivoTamano { get; set; } // Tamaño original antes de compresión
    public long ArchivoTamanoComprimido { get; set; } // Tamaño después de compresión
    public string ArchivoTipo { get; set; } = "application/pdf";
    public bool ArchivoEstaComprimido { get; set; } = true; // Indica si está comprimido
    
    // Control de flujo
    public string Estado { get; set; } = "Pendiente"; // "Pendiente", "Aprobada", "Rechazada"
    public string? ComentariosRevision { get; set; }
    public string? MotivoRechazo { get; set; }
    public DateTime? FechaRevision { get; set; }
    public string? ComentariosSolicitud { get; set; }
    
    // Relaciones
    public virtual Docente Docente { get; set; } = null!;
}
