using SGA.Domain.Enums;

namespace SGA.Application.DTOs.Admin;

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

/// <summary>
/// DTO para listado resumido de configuraciones
/// </summary>
public class ConfiguracionRequisitoResumenDto
{
    public Guid Id { get; set; }
    public string NombreAscenso { get; set; } = string.Empty;
    public string ResumenRequisitos { get; set; } = string.Empty;
    public bool EstaActivo { get; set; }
    public DateTime FechaModificacion { get; set; }
    public string? ModificadoPor { get; set; }
}

/// <summary>
/// DTO para validación masiva de configuraciones
/// </summary>
public class ValidacionConfiguracionesDto
{
    public List<ConfiguracionRequisitoDto> ConfiguracionesValidas { get; set; } = new();
    public List<ConfiguracionRequisitoDto> ConfiguracionesInvalidas { get; set; } = new();
    public List<string> Errores { get; set; } = new();
    public int TotalConfiguraciones { get; set; }
    public int ConfiguracionesActivas { get; set; }
    public bool TodasConfiguradas { get; set; }
}

/// <summary>
/// DTO para historial de cambios en configuraciones
/// </summary>
public class HistorialConfiguracionDto
{
    public Guid ConfiguracionId { get; set; }
    public string NombreAscenso { get; set; } = string.Empty;
    public string CamposModificados { get; set; } = string.Empty;
    public string ValoresAnteriores { get; set; } = string.Empty;
    public string ValoresNuevos { get; set; } = string.Empty;
    public string ModificadoPor { get; set; } = string.Empty;
    public DateTime FechaModificacion { get; set; }
    public string TipoAccion { get; set; } = string.Empty; // Creado, Actualizado, Activado, Desactivado
}
