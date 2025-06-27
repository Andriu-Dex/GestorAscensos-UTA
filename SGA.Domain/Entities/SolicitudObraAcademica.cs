using SGA.Domain.Common;

namespace SGA.Domain.Entities;

public class SolicitudObraAcademica : BaseEntity
{
    public Guid DocenteId { get; set; }
    public string DocenteCedula { get; set; } = string.Empty;
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
    public string? ArchivoNombre { get; set; }
    public string? ArchivoRuta { get; set; }
    public string? ArchivoTipo { get; set; }
    public long? ArchivoTamano { get; set; }
    
    // Estado de la solicitud
    public string Estado { get; set; } = "Pendiente"; // Pendiente, Aprobada, Rechazada
    public string? ComentariosRevision { get; set; }
    public Guid? RevisadoPorId { get; set; }
    public DateTime? FechaRevision { get; set; }
    public string? MotivoRechazo { get; set; }
    
    // Metadatos de la solicitud
    public string? ComentariosSolicitud { get; set; }
    public Guid SolicitudGrupoId { get; set; } // Para agrupar múltiples obras en una solicitud
    
    // Navegación
    public virtual Docente Docente { get; set; } = null!;
    public virtual Usuario? RevisadoPor { get; set; }
}
