using SGA.Application.DTOs.Admin;
using SGA.Domain.Enums;

namespace SGA.Application.Interfaces;

/// <summary>
/// Interfaz para el servicio de configuración de requisitos
/// </summary>
public interface IConfiguracionRequisitoService
{
    /// <summary>
    /// Obtiene todas las configuraciones de requisitos
    /// </summary>
    Task<List<ConfiguracionRequisitoDto>> GetAllAsync();
    
    /// <summary>
    /// Obtiene solo las configuraciones activas
    /// </summary>
    Task<List<ConfiguracionRequisitoDto>> GetActivasAsync();
    
    /// <summary>
    /// Obtiene configuración por ID
    /// </summary>
    Task<ConfiguracionRequisitoDto?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Obtiene configuración específica para un ascenso
    /// </summary>
    Task<ConfiguracionRequisitoDto?> GetByNivelesAsync(NivelTitular nivelActual, NivelTitular nivelSolicitado);
    
    /// <summary>
    /// Obtiene resumen de configuraciones para administración
    /// </summary>
    Task<List<ConfiguracionRequisitoResumenDto>> GetResumenAsync();
    
    /// <summary>
    /// Crea una nueva configuración
    /// </summary>
    Task<ConfiguracionRequisitoDto> CreateAsync(CrearActualizarConfiguracionRequisitoDto dto, string usuarioEmail);
    
    /// <summary>
    /// Actualiza una configuración existente
    /// </summary>
    Task<ConfiguracionRequisitoDto> UpdateAsync(Guid id, CrearActualizarConfiguracionRequisitoDto dto, string usuarioEmail);
    
    /// <summary>
    /// Elimina una configuración
    /// </summary>
    Task<bool> DeleteAsync(Guid id, string usuarioEmail);
    
    /// <summary>
    /// Activa o desactiva una configuración
    /// </summary>
    Task<bool> ToggleActivoAsync(Guid id, string usuarioEmail);
    
    /// <summary>
    /// Valida todas las configuraciones del sistema
    /// </summary>
    Task<ValidacionConfiguracionesDto> ValidarConfiguracionesAsync();
    
    /// <summary>
    /// Inicializa configuraciones por defecto si no existen
    /// </summary>
    Task InicializarConfiguracionesPorDefectoAsync(string usuarioEmail);
    
    /// <summary>
    /// Exporta configuraciones a JSON
    /// </summary>
    Task<string> ExportarConfiguracionesAsync();
    
    /// <summary>
    /// Importa configuraciones desde JSON
    /// </summary>
    Task<bool> ImportarConfiguracionesAsync(string jsonConfiguraciones, string usuarioEmail);
    
    /// <summary>
    /// Obtiene historial de cambios en configuraciones
    /// </summary>
    Task<List<HistorialConfiguracionDto>> GetHistorialCambiosAsync(int limite = 50);
}
