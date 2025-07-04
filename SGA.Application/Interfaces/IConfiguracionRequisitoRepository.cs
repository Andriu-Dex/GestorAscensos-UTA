using SGA.Domain.Entities;
using SGA.Domain.Enums;

namespace SGA.Application.Interfaces;

/// <summary>
/// Interfaz para el repositorio de configuración de requisitos
/// </summary>
public interface IConfiguracionRequisitoRepository
{
    /// <summary>
    /// Obtiene todas las configuraciones de requisitos
    /// </summary>
    Task<List<ConfiguracionRequisito>> GetAllAsync();
    
    /// <summary>
    /// Obtiene solo las configuraciones activas
    /// </summary>
    Task<List<ConfiguracionRequisito>> GetActivasAsync();
    
    /// <summary>
    /// Obtiene configuración por ID
    /// </summary>
    Task<ConfiguracionRequisito?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Obtiene configuración específica para un ascenso
    /// </summary>
    Task<ConfiguracionRequisito?> GetByNivelesAsync(NivelTitular nivelActual, NivelTitular nivelSolicitado);
    
    /// <summary>
    /// Obtiene configuraciones para un nivel actual específico
    /// </summary>
    Task<List<ConfiguracionRequisito>> GetByNivelActualAsync(NivelTitular nivelActual);
    
    /// <summary>
    /// Crea una nueva configuración
    /// </summary>
    Task<ConfiguracionRequisito> CreateAsync(ConfiguracionRequisito configuracion);
    
    /// <summary>
    /// Actualiza una configuración existente
    /// </summary>
    Task<ConfiguracionRequisito> UpdateAsync(ConfiguracionRequisito configuracion);
    
    /// <summary>
    /// Elimina una configuración
    /// </summary>
    Task DeleteAsync(Guid id);
    
    /// <summary>
    /// Activa o desactiva una configuración
    /// </summary>
    Task<bool> ToggleActivoAsync(Guid id);
    
    /// <summary>
    /// Verifica si existe una configuración para un ascenso específico
    /// </summary>
    Task<bool> ExisteConfiguracionAsync(NivelTitular nivelActual, NivelTitular nivelSolicitado, Guid? excludeId = null);
    
    /// <summary>
    /// Obtiene configuraciones ordenadas por nivel
    /// </summary>
    Task<List<ConfiguracionRequisito>> GetOrderedByNivelAsync();
    
    /// <summary>
    /// Valida que todas las transiciones de nivel tengan configuración
    /// </summary>
    Task<Dictionary<string, bool>> ValidarCoberturaNivelesAsync();
}
