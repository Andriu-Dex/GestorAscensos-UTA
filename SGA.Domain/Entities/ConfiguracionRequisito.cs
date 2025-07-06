using SGA.Domain.Common;
using SGA.Domain.Enums;
using SGA.Domain.Extensions;

namespace SGA.Domain.Entities;

/// <summary>
/// Entidad para configuración dinámica de requisitos de ascenso.
/// Permite a los administradores modificar los parámetros de ascenso sin cambiar código.
/// Soporta tanto niveles predefinidos (enum) como títulos académicos personalizados.
/// </summary>
public class ConfiguracionRequisito : BaseEntity
{
    /// <summary>
    /// Nivel actual del docente (enum predefinido - para compatibilidad)
    /// Null si se usa TituloActualId
    /// </summary>
    public NivelTitular? NivelActual { get; set; }
    
    /// <summary>
    /// Nivel al que desea ascender (enum predefinido - para compatibilidad)
    /// Null si se usa TituloSolicitadoId
    /// </summary>
    public NivelTitular? NivelSolicitado { get; set; }
    
    /// <summary>
    /// ID del título académico actual (títulos dinámicos)
    /// Null si se usa NivelActual
    /// </summary>
    public Guid? TituloActualId { get; set; }
    
    /// <summary>
    /// ID del título académico solicitado (títulos dinámicos)
    /// Null si se usa NivelSolicitado
    /// </summary>
    public Guid? TituloSolicitadoId { get; set; }
    
    /// <summary>
    /// Navegación al título académico actual
    /// </summary>
    public virtual TituloAcademico? TituloActual { get; set; }
    
    /// <summary>
    /// Navegación al título académico solicitado
    /// </summary>
    public virtual TituloAcademico? TituloSolicitado { get; set; }
    
    /// <summary>
    /// Tiempo mínimo requerido en el nivel actual (en meses)
    /// </summary>
    public int TiempoMinimoMeses { get; set; }
    
    /// <summary>
    /// Número mínimo de obras académicas requeridas
    /// </summary>
    public int ObrasMinimas { get; set; }
    
    /// <summary>
    /// Puntaje mínimo de evaluación docente requerido (porcentaje)
    /// </summary>
    public decimal PuntajeEvaluacionMinimo { get; set; }
    
    /// <summary>
    /// Horas mínimas de capacitación requeridas
    /// </summary>
    public int HorasCapacitacionMinimas { get; set; }
    
    /// <summary>
    /// Tiempo mínimo en investigación requerido (en meses)
    /// </summary>
    public int TiempoInvestigacionMinimo { get; set; }
    
    /// <summary>
    /// Indica si este requisito está activo
    /// </summary>
    public bool EstaActivo { get; set; } = true;
    
    /// <summary>
    /// Descripción opcional del requisito
    /// </summary>
    public string? Descripcion { get; set; }
    
    /// <summary>
    /// Usuario que creó o modificó por última vez esta configuración
    /// </summary>
    public string? ModificadoPor { get; set; }
    
    /// <summary>
    /// Propiedad calculada para mostrar el nombre del ascenso
    /// </summary>
    public string NombreAscenso => ObtenerNombreAscenso();
    
    /// <summary>
    /// Obtiene el nombre completo del ascenso considerando títulos dinámicos y enum
    /// </summary>
    private string ObtenerNombreAscenso()
    {
        string nombreActual, nombreSolicitado;
        
        // Determinar nombre del nivel actual
        if (TituloActual != null)
            nombreActual = TituloActual.Nombre;
        else if (NivelActual.HasValue)
            nombreActual = NivelActual.Value.GetDescription();
        else
            nombreActual = "N/A";
            
        // Determinar nombre del nivel solicitado
        if (TituloSolicitado != null)
            nombreSolicitado = TituloSolicitado.Nombre;
        else if (NivelSolicitado.HasValue)
            nombreSolicitado = NivelSolicitado.Value.GetDescription();
        else
            nombreSolicitado = "N/A";
            
        return $"{nombreActual} → {nombreSolicitado}";
    }
    
    /// <summary>
    /// Valida que la configuración del requisito sea válida
    /// </summary>
    public bool EsConfiguracionValida()
    {
        // Validar que tenga al menos un tipo de nivel definido para actual y solicitado
        var tieneNivelActual = NivelActual.HasValue || TituloActualId.HasValue;
        var tieneNivelSolicitado = NivelSolicitado.HasValue || TituloSolicitadoId.HasValue;
        
        if (!tieneNivelActual || !tieneNivelSolicitado)
            return false;
            
        // No debe tener ambos tipos definidos simultáneamente
        if (NivelActual.HasValue && TituloActualId.HasValue)
            return false;
        if (NivelSolicitado.HasValue && TituloSolicitadoId.HasValue)
            return false;
            
        // Validar ascenso secuencial solo para niveles enum
        if (NivelActual.HasValue && NivelSolicitado.HasValue)
        {
            if (!NivelActual.Value.EsAscensoValido(NivelSolicitado.Value))
                return false;
        }
        
        // Para títulos dinámicos, validar orden jerárquico
        if (TituloActual != null && TituloSolicitado != null)
        {
            if (TituloActual.OrdenJerarquico >= TituloSolicitado.OrdenJerarquico)
                return false;
        }
            
        // Validar valores mínimos
        if (TiempoMinimoMeses < 0 || ObrasMinimas < 0 || 
            PuntajeEvaluacionMinimo < 0 || PuntajeEvaluacionMinimo > 100 ||
            HorasCapacitacionMinimas < 0 || TiempoInvestigacionMinimo < 0)
            return false;
            
        return true;
    }
    
    /// <summary>
    /// Obtiene un resumen de los requisitos configurados
    /// </summary>
    public string ObtenerResumenRequisitos()
    {
        var items = new List<string>();
        
        if (TiempoMinimoMeses > 0)
            items.Add($"{TiempoMinimoMeses} meses en nivel");
            
        if (ObrasMinimas > 0)
            items.Add($"{ObrasMinimas} obra(s) académica(s)");
            
        if (PuntajeEvaluacionMinimo > 0)
            items.Add($"{PuntajeEvaluacionMinimo}% evaluación docente");
            
        if (HorasCapacitacionMinimas > 0)
            items.Add($"{HorasCapacitacionMinimas} horas capacitación");
            
        if (TiempoInvestigacionMinimo > 0)
            items.Add($"{TiempoInvestigacionMinimo} meses investigación");
            
        return string.Join(", ", items);
    }
}
