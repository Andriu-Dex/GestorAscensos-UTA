// Entidades para la base de datos DAC (Evaluación Docente)
namespace SGA.Domain.Entities.External;

public class EvaluacionDocenteDAC
{
    public int Id { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public int PeriodoId { get; set; }
    public decimal PuntajeTotal { get; set; }
    public decimal PuntajeMaximo { get; set; }
    public decimal Porcentaje { get; set; }
    public DateTime FechaEvaluacion { get; set; }
    public string? Observaciones { get; set; }
    
    public virtual PeriodoAcademicoDAC Periodo { get; set; } = null!;
}

public class PeriodoAcademicoDAC
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public bool EstaActivo { get; set; }
    
    public virtual ICollection<EvaluacionDocenteDAC> Evaluaciones { get; set; } = new List<EvaluacionDocenteDAC>();
}

public class CriterioEvaluacionDAC
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal PesoMaximo { get; set; }
    public bool EstaActivo { get; set; }
}
