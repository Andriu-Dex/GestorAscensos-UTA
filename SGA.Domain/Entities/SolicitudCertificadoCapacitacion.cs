using SGA.Domain.Common;

namespace SGA.Domain.Entities;

/// <summary>
/// Entidad para gestionar solicitudes de certificados de capacitación subidos por docentes
/// </summary>
public class SolicitudCertificadoCapacitacion : BaseEntity
{
    public Guid DocenteId { get; set; }
    public string DocenteCedula { get; set; } = string.Empty;
    
    // Información del certificado
    public string NombreCurso { get; set; } = string.Empty;
    public string InstitucionOfertante { get; set; } = string.Empty;
    public string TipoCapacitacion { get; set; } = string.Empty; // Curso, Taller, Seminario, Diplomado, etc.
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int HorasDuracion { get; set; }
    public string Modalidad { get; set; } = string.Empty; // Presencial, Virtual, Mixta
    public string? NumeroRegistro { get; set; } // Número de registro o código del certificado
    public string? AreaTematica { get; set; } // Área temática del curso
    public string? Descripcion { get; set; }
    
    // Archivo del certificado PDF
    public string? ArchivoNombre { get; set; }
    public string? ArchivoRuta { get; set; }
    public string? ArchivoTipo { get; set; }
    public long? ArchivoTamano { get; set; }
    
    // Estado de la solicitud
    public string Estado { get; set; } = "Pendiente"; // Pendiente, Aprobada, Rechazada, En Proceso
    public string? ComentariosRevision { get; set; }
    public Guid? RevisadoPorId { get; set; }
    public DateTime? FechaRevision { get; set; }
    public string? MotivoRechazo { get; set; }
    
    // Metadatos de la solicitud
    public string? ComentariosSolicitud { get; set; }
    public Guid SolicitudGrupoId { get; set; } // Para agrupar múltiples certificados en una solicitud
    
    // Navegación
    public virtual Docente Docente { get; set; } = null!;
    public virtual Usuario? RevisadoPor { get; set; }
}
