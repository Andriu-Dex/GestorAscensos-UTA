using SGA.Domain.Entities;

namespace SGA.Application.Interfaces;

/// <summary>
/// Interfaz para el repositorio de títulos académicos
/// </summary>
public interface ITituloAcademicoRepository
{
    /// <summary>
    /// Obtiene un título académico por ID
    /// </summary>
    Task<TituloAcademico?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Obtiene todos los títulos académicos
    /// </summary>
    Task<List<TituloAcademico>> GetAllAsync();
    
    /// <summary>
    /// Crea un nuevo título académico
    /// </summary>
    Task<TituloAcademico> CreateAsync(TituloAcademico titulo);
    
    /// <summary>
    /// Actualiza un título académico existente
    /// </summary>
    Task<TituloAcademico> UpdateAsync(TituloAcademico titulo);
    
    /// <summary>
    /// Elimina un título académico
    /// </summary>
    Task DeleteAsync(Guid id);
    
    /// <summary>
    /// Obtiene todos los títulos académicos activos ordenados por jerarquía
    /// </summary>
    Task<List<TituloAcademico>> GetActivosOrdenadosAsync();
    
    /// <summary>
    /// Obtiene un título académico por su código único
    /// </summary>
    Task<TituloAcademico?> GetByCodigoAsync(string codigo);
    
    /// <summary>
    /// Verifica si existe un título con el código especificado (excluyendo el ID dado)
    /// </summary>
    Task<bool> ExisteCodigoAsync(string codigo, Guid? excluirId = null);
    
    /// <summary>
    /// Verifica si existe un título con el nombre especificado (excluyendo el ID dado)
    /// </summary>
    Task<bool> ExisteNombreAsync(string nombre, Guid? excluirId = null);
    
    /// <summary>
    /// Obtiene títulos por rango jerárquico
    /// </summary>
    Task<List<TituloAcademico>> GetByRangoJerarquicoAsync(int ordenMinimo, int ordenMaximo);
    
    /// <summary>
    /// Obtiene los títulos que pueden ser ascenso del título dado
    /// </summary>
    Task<List<TituloAcademico>> GetPosiblesAscensosAsync(Guid tituloActualId);
    
    /// <summary>
    /// Obtiene títulos equivalentes a niveles del enum
    /// </summary>
    Task<List<TituloAcademico>> GetEquivalentesANivelAsync(int nivelEnum);
    
    /// <summary>
    /// Verifica si un orden jerárquico está disponible
    /// </summary>
    Task<bool> EstaOrdenDisponibleAsync(int orden, Guid? excluirId = null);
    
    /// <summary>
    /// Obtiene el siguiente orden jerárquico disponible
    /// </summary>
    Task<int> GetSiguienteOrdenDisponibleAsync();
}
