using SGA.Web.Models.Enums;

namespace SGA.Web.Models.Admin;

/// <summary>
/// DTO para visualizar configuración de requisitos (híbrido: enum + títulos dinámicos)
/// </summary>
public class ConfiguracionRequisitoDto
{
    public Guid Id { get; set; }
    
    // Soporte híbrido para niveles enum
    public NivelTitular? NivelActual { get; set; }
    public NivelTitular? NivelSolicitado { get; set; }
    
    // Soporte híbrido para títulos dinámicos
    public Guid? TituloActualId { get; set; }
    public Guid? TituloSolicitadoId { get; set; }
    public string? TituloActualNombre { get; set; }
    public string? TituloSolicitadoNombre { get; set; }
    
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
    
    // Propiedades de conveniencia para determinar el tipo
    public bool EsNivelEnum => NivelActual.HasValue && NivelSolicitado.HasValue;
    public bool EsTituloDinamico => TituloActualId.HasValue && TituloSolicitadoId.HasValue;
}

/// <summary>
/// DTO para crear o actualizar configuración de requisitos (híbrido)
/// </summary>
public class CrearActualizarConfiguracionRequisitoDto
{
    // Soporte híbrido para niveles enum
    public NivelTitular? NivelActual { get; set; }
    public NivelTitular? NivelSolicitado { get; set; }
    
    // Soporte híbrido para títulos dinámicos
    public Guid? TituloActualId { get; set; }
    public Guid? TituloSolicitadoId { get; set; }
    
    public int TiempoMinimoMeses { get; set; }
    public int ObrasMinimas { get; set; }
    public decimal PuntajeEvaluacionMinimo { get; set; }
    public int HorasCapacitacionMinimas { get; set; }
    public int TiempoInvestigacionMinimo { get; set; }
    public bool EstaActivo { get; set; } = true;
    public string? Descripcion { get; set; }
}

/// <summary>
/// DTO simplificado para opciones de título académico en selectores
/// </summary>
public class TituloAcademicoOpcionDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int OrdenJerarquico { get; set; }
    public string? ColorHex { get; set; }
    public bool EstaActivo { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public bool EsTituloSistema { get; set; }
}
