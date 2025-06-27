namespace SGA.Application.DTOs.Docentes;

public class RequisitosAscensoDto
{
    public bool CumpleTiempoRol { get; set; }
    public bool CumpleObras { get; set; }
    public bool CumpleEvaluacion { get; set; }
    public bool CumpleCapacitacion { get; set; }
    public bool CumpleInvestigacion { get; set; }
    
    public int TiempoRolRequerido { get; set; }
    public int TiempoRolActual { get; set; }
    
    public int ObrasRequeridas { get; set; }
    public int ObrasActuales { get; set; }
    
    public decimal EvaluacionRequerida { get; set; }
    public decimal EvaluacionActual { get; set; }
    
    public int CapacitacionRequerida { get; set; }
    public int CapacitacionActual { get; set; }
    
    public int InvestigacionRequerida { get; set; }
    public int InvestigacionActual { get; set; }
    
    public bool CumpleTodosRequisitos => CumpleTiempoRol && CumpleObras && CumpleEvaluacion && CumpleCapacitacion && CumpleInvestigacion;
}
