using SGA.Application.DTOs.Admin;

namespace SGA.Application.Interfaces;

/// <summary>
/// Interfaz para el servicio de títulos académicos
/// </summary>
public interface ITituloAcademicoService
{
    /// <summary>
    /// Obtiene todos los títulos académicos
    /// </summary>
    Task<List<TituloAcademicoDto>> GetAllAsync();
    
    /// <summary>
    /// Obtiene solo los títulos académicos activos
    /// </summary>
    Task<List<TituloAcademicoDto>> GetActivosAsync();
    
    /// <summary>
    /// Obtiene un título académico por ID
    /// </summary>
    Task<TituloAcademicoDto?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Obtiene un título académico por código
    /// </summary>
    Task<TituloAcademicoDto?> GetByCodigoAsync(string codigo);
    
    /// <summary>
    /// Crea un nuevo título académico
    /// </summary>
    Task<TituloAcademicoDto> CreateAsync(CrearActualizarTituloAcademicoDto dto, string usuarioEmail);
    
    /// <summary>
    /// Actualiza un título académico existente
    /// </summary>
    Task<TituloAcademicoDto> UpdateAsync(Guid id, CrearActualizarTituloAcademicoDto dto, string usuarioEmail);
    
    /// <summary>
    /// Elimina un título académico (solo si no está en uso)
    /// </summary>
    Task<bool> DeleteAsync(Guid id, string usuarioEmail);
    
    /// <summary>
    /// Activa o desactiva un título académico
    /// </summary>
    Task<bool> ToggleActivoAsync(Guid id, string usuarioEmail);
    
    /// <summary>
    /// Obtiene títulos académicos como opciones de selección
    /// </summary>
    Task<List<TituloAcademicoOpcionDto>> GetOpcionesAsync(bool soloActivos = true);
    
    /// <summary>
    /// Obtiene resumen de títulos para administración
    /// </summary>
    Task<List<TituloAcademicoResumenDto>> GetResumenAsync();
    
    /// <summary>
    /// Obtiene niveles híbridos (enum + títulos dinámicos) para formularios
    /// </summary>
    Task<List<NivelAcademicoHibridoDto>> GetNivelesHibridosAsync();
    
    /// <summary>
    /// Obtiene posibles ascensos para un título dado
    /// </summary>
    Task<List<TituloAcademicoOpcionDto>> GetPosiblesAscensosAsync(Guid tituloActualId);
    
    /// <summary>
    /// Verifica si un título puede ser eliminado (no está en uso)
    /// </summary>
    Task<bool> PuedeSerEliminadoAsync(Guid id);
    
    /// <summary>
    /// Inicializa títulos académicos por defecto equivalentes al enum
    /// </summary>
    Task InicializarTitulosPorDefectoAsync(string usuarioEmail);
    
    /// <summary>
    /// Valida si un código de título es único
    /// </summary>
    Task<bool> EsCodigoUnicoAsync(string codigo, Guid? excluirId = null);
    
    /// <summary>
    /// Valida si un nombre de título es único
    /// </summary>
    Task<bool> EsNombreUnicoAsync(string nombre, Guid? excluirId = null);
}
