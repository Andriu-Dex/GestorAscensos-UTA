// Entidades para la base de datos DITIC (Capacitación)
namespace SGA.Domain.Entities.External;

public class CursoDITIC
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int HorasDuracion { get; set; }
    public string Modalidad { get; set; } = string.Empty; // Presencial, Virtual, Mixta
    public bool EstaActivo { get; set; }
    
    public virtual ICollection<ParticipacionCursoDITIC> Participaciones { get; set; } = new List<ParticipacionCursoDITIC>();
}

public class ParticipacionCursoDITIC
{
    public int Id { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public int CursoId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public bool Aprobado { get; set; }
    public decimal? NotaFinal { get; set; }
    public bool CertificadoEmitido { get; set; }
    
    public virtual CursoDITIC Curso { get; set; } = null!;
}

public class CertificacionDITIC
{
    public int Id { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public string NombreCertificacion { get; set; } = string.Empty;
    public string Institucion { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public int HorasEquivalentes { get; set; }
}
