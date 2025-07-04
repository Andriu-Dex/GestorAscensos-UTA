namespace SGA.Application.DTOs.Docentes;

public class IndicadoresDocenteDto
{
    public int TiempoRol { get; set; }
    public int NumeroObras { get; set; }
    public decimal PuntajeEvaluacion { get; set; }
    public int HorasCapacitacion { get; set; }
    public int TiempoInvestigacion { get; set; }
    
    // Propiedades adicionales para evaluaciones docentes
    public int PeriodosEvaluados { get; set; }
    public DateTime? FechaUltimaEvaluacion { get; set; }
    public string PeriodoEvaluado { get; set; } = string.Empty;
    public bool CumpleRequisitoEvaluacion { get; set; }
    
    // Propiedades para tiempo detallado que espera el frontend
    public DateTime FechaInicioRol { get; set; }
    public TimeSpan TiempoTranscurrido { get; set; }
    public TimeSpan TiempoRestante { get; set; }
    public string TiempoTranscurridoTexto { get; set; } = string.Empty;
    public string TiempoRestanteTexto { get; set; } = string.Empty;
    public bool CumpleTiempoMinimo { get; set; }
}
