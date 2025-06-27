namespace SGA.Web.Models
{
    public class RequisitosModel
    {
        public bool CumpleTiempoRol { get; set; } = false;
        public bool CumpleObras { get; set; } = false;
        public bool CumpleEvaluacion { get; set; } = false;
        public bool CumpleCapacitacion { get; set; } = false;
        public bool CumpleInvestigacion { get; set; } = false;
        
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
}
