using SGA.Domain.Common;
using SGA.Domain.Enums;
using SGA.Domain.Extensions;

namespace SGA.Domain.Entities;

/// <summary>
/// Entidad para configuración dinámica de requisitos de ascenso.
/// Permite a los administradores modificar los parámetros de ascenso sin cambiar código.
/// </summary>
public class ConfiguracionRequisito : BaseEntity
{
    /// <summary>
    /// Nivel actual del docente (ej: Titular1, Titular2, etc.)
    /// </summary>
    public NivelTitular NivelActual { get; set; }
    
    /// <summary>
    /// Nivel al que desea ascender (ej: Titular2, Titular3, etc.)
    /// </summary>
    public NivelTitular NivelSolicitado { get; set; }
    
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
    public string NombreAscenso => $"{NivelActual.GetDescription()} → {NivelSolicitado.GetDescription()}";
    
    /// <summary>
    /// Valida que la configuración del requisito sea válida
    /// </summary>
    public bool EsConfiguracionValida()
    {
        // Validar que el ascenso sea secuencial
        if (!NivelActual.EsAscensoValido(NivelSolicitado))
            return false;
            
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
