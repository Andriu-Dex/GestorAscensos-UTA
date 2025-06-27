using SGA.Domain.Enums;

namespace SGA.Application.DTOs.Docentes;

public class RequisitoAscensoDto
{
    public NivelTitular NivelActual { get; set; }
    public NivelTitular NivelDestino { get; set; }
    public bool PuedeAscender { get; set; }
    
    // Requisitos de antigüedad
    public int AñosRequeridos { get; set; }
    public int AñosActuales { get; set; }
    public bool CumpleAntiguedad { get; set; }
    
    // Requisitos de obras académicas
    public int ObrasRequeridas { get; set; }
    public int ObrasActuales { get; set; }
    public bool CumpleObras { get; set; }
    
    // Requisitos de evaluación
    public decimal PromedioRequerido { get; set; }
    public decimal PromedioActual { get; set; }
    public bool CumpleEvaluacion { get; set; }
    
    // Requisitos de capacitación
    public int HorasRequeridas { get; set; }
    public int HorasActuales { get; set; }
    public bool CumpleCapacitacion { get; set; }
    
    // Requisitos de investigación (para niveles superiores)
    public int MesesRequeridos { get; set; }
    public int MesesActuales { get; set; }
    public bool CumpleInvestigacion { get; set; }
    
    public string Observaciones { get; set; } = string.Empty;
    public List<string> RequisitosFaltantes { get; set; } = new();
}
