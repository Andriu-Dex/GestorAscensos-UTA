namespace SGA.Web.Models
{
    public class IndicadoresModel
    {
        public int TiempoRol { get; set; } = 0;
        public int NumeroObras { get; set; } = 0;
        public decimal PuntajeEvaluacion { get; set; } = 0;
        public int HorasCapacitacion { get; set; } = 0;
        public int TiempoInvestigacion { get; set; } = 0;
        
        // Propiedades adicionales para cálculo de tiempo detallado
        public DateTime FechaInicioRol { get; set; } = DateTime.Now;
        public TimeSpan TiempoTranscurrido { get; set; }
        public TimeSpan TiempoRestante { get; set; }
        public string TiempoTranscurridoTexto { get; set; } = string.Empty;
        public string TiempoRestanteTexto { get; set; } = string.Empty;
        public bool CumpleTiempoMinimo { get; set; } = false;
        
        // Propiedades adicionales para evaluaciones docentes
        public int PeriodosEvaluados { get; set; } = 0;
        public DateTime? FechaUltimaEvaluacion { get; set; }
        public string PeriodoEvaluado { get; set; } = string.Empty;
        public bool CumpleRequisitoEvaluacion { get; set; } = false;
    }
}
