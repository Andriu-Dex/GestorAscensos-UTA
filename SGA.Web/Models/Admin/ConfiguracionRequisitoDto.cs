using SGA.Web.Models.Enums;

namespace SGA.Web.Models.Admin;

/// <summary>
/// DTO para visualizar configuración de requisitos
/// </summary>
public class ConfiguracionRequisitoDto
{
    public Guid Id { get; set; }
    public NivelTitular NivelActual { get; set; }
    public NivelTitular NivelSolicitado { get; set; }
    public int TiempoMinimoMeses { get; set; }
    public int ObrasMinimas { get; set; }
    public decimal PuntajeEvaluacionMinimo { get; set; }
    public int HorasCapacitacionMinimas { get; set; }
    public int TiempoInvestigacionMinimo { get; set; }
    public bool EstaActivo { get; set; }
    public string? Descripcion { get; set; }
    public string? ModificadoPor { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }

    // Propiedades calculadas
    public string NombreAscenso { get; set; } = string.Empty;
    public string ResumenRequisitos { get; set; } = string.Empty;
    public string NivelActualNombre { get; set; } = string.Empty;
    public string NivelSolicitadoNombre { get; set; } = string.Empty;
    public int TiempoMinimoAnios => TiempoMinimoMeses / 12;
    public int TiempoMinimoMesesRestantes => TiempoMinimoMeses % 12;
}

/// <summary>
/// DTO para crear o actualizar configuración de requisitos
/// </summary>
public class CrearActualizarConfiguracionRequisitoDto
{
    public NivelTitular NivelActual { get; set; }
    public NivelTitular NivelSolicitado { get; set; }
    public int TiempoMinimoMeses { get; set; }
    public int ObrasMinimas { get; set; }
    public decimal PuntajeEvaluacionMinimo { get; set; }
    public int HorasCapacitacionMinimas { get; set; }
    public int TiempoInvestigacionMinimo { get; set; }
    public bool EstaActivo { get; set; } = true;
    public string? Descripcion { get; set; }
}
