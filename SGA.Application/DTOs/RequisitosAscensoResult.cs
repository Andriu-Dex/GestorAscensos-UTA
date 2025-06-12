namespace SGA.Application.DTOs
{
    public class RequisitosAscensoResult
    {
        public bool CumpleTodosRequisitos { get; set; }
        public bool CumpleTiempo { get; set; }
        public bool CumpleObras { get; set; }
        public bool CumpleEvaluacion { get; set; }
        public bool CumpleCapacitacion { get; set; }
        public bool CumpleInvestigacion { get; set; }
        public string? Mensaje { get; set; }
    }
}
