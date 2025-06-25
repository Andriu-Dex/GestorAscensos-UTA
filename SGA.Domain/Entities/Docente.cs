using SGA.Domain.Common;
using SGA.Domain.Enums;

namespace SGA.Domain.Entities;

public class Docente : BaseEntity
{
    public string Cedula { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public NivelTitular NivelActual { get; set; } = NivelTitular.Titular1;
    public DateTime FechaInicioNivelActual { get; set; }
    public DateTime? FechaUltimoAscenso { get; set; }
    public bool EstaActivo { get; set; } = true;
    
    // Datos importados de sistemas externos
    public DateTime? FechaNombramiento { get; set; }
    public decimal? PromedioEvaluaciones { get; set; }
    public int? HorasCapacitacion { get; set; }
    public int? NumeroObrasAcademicas { get; set; }
    public int? MesesInvestigacion { get; set; }
    public DateTime? FechaUltimaImportacion { get; set; }
    
    // Relaciones
    public Guid UsuarioId { get; set; }
    public virtual Usuario Usuario { get; set; } = null!;
    public virtual ICollection<SolicitudAscenso> SolicitudesAscenso { get; set; } = new List<SolicitudAscenso>();
    
    // Métodos de validación
    public bool CumpleRequisitosParaNivel(NivelTitular nivelObjetivo)
    {
        var tiempoEnNivel = DateTime.UtcNow - FechaInicioNivelActual;
        var cumpleTiempo = tiempoEnNivel.TotalDays >= (4 * 365); // 4 años
        
        return nivelObjetivo switch
        {
            NivelTitular.Titular2 => cumpleTiempo && 
                                    (NumeroObrasAcademicas ?? 0) >= 1 && 
                                    (PromedioEvaluaciones ?? 0) >= 75 && 
                                    (HorasCapacitacion ?? 0) >= 96,
                                    
            NivelTitular.Titular3 => cumpleTiempo && 
                                    (NumeroObrasAcademicas ?? 0) >= 2 && 
                                    (PromedioEvaluaciones ?? 0) >= 75 && 
                                    (HorasCapacitacion ?? 0) >= 96 && 
                                    (MesesInvestigacion ?? 0) >= 12,
                                    
            NivelTitular.Titular4 => cumpleTiempo && 
                                    (NumeroObrasAcademicas ?? 0) >= 3 && 
                                    (PromedioEvaluaciones ?? 0) >= 75 && 
                                    (HorasCapacitacion ?? 0) >= 128 && 
                                    (MesesInvestigacion ?? 0) >= 24,
                                    
            NivelTitular.Titular5 => cumpleTiempo && 
                                    (NumeroObrasAcademicas ?? 0) >= 5 && 
                                    (PromedioEvaluaciones ?? 0) >= 75 && 
                                    (HorasCapacitacion ?? 0) >= 160 && 
                                    (MesesInvestigacion ?? 0) >= 24,
            _ => false
        };
    }
    
    public string NombreCompleto => $"{Nombres} {Apellidos}";
}
