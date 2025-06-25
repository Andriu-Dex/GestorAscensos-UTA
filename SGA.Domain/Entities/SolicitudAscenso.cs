using SGA.Domain.Common;
using SGA.Domain.Enums;

namespace SGA.Domain.Entities;

public class SolicitudAscenso : BaseEntity
{
    public Guid DocenteId { get; set; }
    public NivelTitular NivelActual { get; set; }
    public NivelTitular NivelSolicitado { get; set; }
    public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Pendiente;
    public string? MotivoRechazo { get; set; }
    public DateTime FechaSolicitud { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public Guid? AprobadoPorId { get; set; }
    
    // Datos al momento de la solicitud
    public decimal PromedioEvaluaciones { get; set; }
    public int HorasCapacitacion { get; set; }
    public int NumeroObrasAcademicas { get; set; }
    public int MesesInvestigacion { get; set; }
    public int TiempoEnNivelDias { get; set; }
    
    // Relaciones
    public virtual Docente Docente { get; set; } = null!;
    public virtual Usuario? AprobadoPor { get; set; }
    public virtual ICollection<Documento> Documentos { get; set; } = new List<Documento>();
}
