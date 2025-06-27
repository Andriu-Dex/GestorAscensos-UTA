// Entidades para la base de datos DIR INV (Investigación)
namespace SGA.Domain.Entities.External;

public class ObraAcademicaDIRINV
{
    public int Id { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string TipoObra { get; set; } = string.Empty; // Artículo, Libro, Capítulo, etc.
    public DateTime FechaPublicacion { get; set; }
    public string? Editorial { get; set; }
    public string? Revista { get; set; }
    public string? ISBN_ISSN { get; set; }
    public string? DOI { get; set; }
    public bool EsIndexada { get; set; }
    public string? IndiceIndexacion { get; set; }
    public string? Autores { get; set; }
    public string? Descripcion { get; set; }
}

public class ProyectoInvestigacionDIRINV
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string Estado { get; set; } = string.Empty; // En curso, Finalizado, Suspendido
    public decimal? PresupuestoTotal { get; set; }
    public string? FuenteFinanciamiento { get; set; }
    
    public virtual ICollection<ParticipacionProyectoDIRINV> Participaciones { get; set; } = new List<ParticipacionProyectoDIRINV>();
}

public class ParticipacionProyectoDIRINV
{
    public int Id { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public int ProyectoId { get; set; }
    public string RolEnProyecto { get; set; } = string.Empty; // Director, Investigador, Colaborador
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int HorasSemanales { get; set; }
    
    public virtual ProyectoInvestigacionDIRINV Proyecto { get; set; } = null!;
}
