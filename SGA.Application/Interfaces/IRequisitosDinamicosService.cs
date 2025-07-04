using SGA.Domain.Enums;

namespace SGA.Application.Interfaces;

/// <summary>
/// Servicio para obtener requisitos de ascenso de forma dinámica
/// </summary>
public interface IRequisitosDinamicosService
{
    /// <summary>
    /// Obtiene los requisitos para un ascenso específico
    /// </summary>
    Task<RequisitoAscenso?> ObtenerRequisitosAsync(NivelTitular nivelActual, NivelTitular nivelSolicitado);
    
    /// <summary>
    /// Obtiene los requisitos para el siguiente nivel de un docente
    /// </summary>
    Task<RequisitoAscenso?> ObtenerRequisitosParaSiguienteNivelAsync(NivelTitular nivelActual);
    
    /// <summary>
    /// Verifica si un ascenso tiene requisitos configurados
    /// </summary>
    Task<bool> TieneRequisitosConfiguradosAsync(NivelTitular nivelActual, NivelTitular nivelSolicitado);
    
    /// <summary>
    /// Obtiene todos los requisitos activos
    /// </summary>
    Task<List<RequisitoAscenso>> ObtenerTodosLosRequisitosActivosAsync();
}

/// <summary>
/// Clase que representa los requisitos para un ascenso específico
/// </summary>
public class RequisitoAscenso
{
    public int NivelActual { get; set; }
    public int NivelSolicitado { get; set; }
    public int TiempoMinimo { get; set; } // En meses
    public int ObrasMinimas { get; set; }
    public decimal PuntajeEvaluacionMinimo { get; set; }
    public int HorasCapacitacionMinimas { get; set; }
    public int TiempoInvestigacionMinimo { get; set; } // En meses
    public bool EstaActivo { get; set; }
    public string? Descripcion { get; set; }
    
    // Conversión desde la nueva entidad ConfiguracionRequisito
    public static RequisitoAscenso FromConfiguracion(Domain.Entities.ConfiguracionRequisito configuracion)
    {
        return new RequisitoAscenso
        {
            NivelActual = (int)configuracion.NivelActual,
            NivelSolicitado = (int)configuracion.NivelSolicitado,
            TiempoMinimo = configuracion.TiempoMinimoMeses,
            ObrasMinimas = configuracion.ObrasMinimas,
            PuntajeEvaluacionMinimo = configuracion.PuntajeEvaluacionMinimo,
            HorasCapacitacionMinimas = configuracion.HorasCapacitacionMinimas,
            TiempoInvestigacionMinimo = configuracion.TiempoInvestigacionMinimo,
            EstaActivo = configuracion.EstaActivo,
            Descripcion = configuracion.Descripcion
        };
    }
}
