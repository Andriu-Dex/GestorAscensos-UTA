using SGA.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGA.Domain.Interfaces
{
    /// <summary>
    /// Interfaz de repositorio para la entidad Requirement
    /// </summary>
    public interface IRequirementRepository
    {
        /// <summary>
        /// Obtiene un requisito por ID
        /// </summary>
        Task<Requirement> GetByIdAsync(int id);
        
        /// <summary>
        /// Obtiene un requisito por nombre
        /// </summary>
        Task<Requirement> GetByNameAsync(string name);
        
        /// <summary>
        /// Obtiene todos los requisitos
        /// </summary>
        Task<IEnumerable<Requirement>> GetAllAsync();
        
        /// <summary>
        /// Añade un nuevo requisito
        /// </summary>
        Task AddAsync(Requirement requirement);
        
        /// <summary>
        /// Actualiza un requisito existente
        /// </summary>
        Task UpdateAsync(Requirement requirement);
        
        /// <summary>
        /// Guarda los cambios en la base de datos
        /// </summary>
        Task SaveChangesAsync();
    }
}
