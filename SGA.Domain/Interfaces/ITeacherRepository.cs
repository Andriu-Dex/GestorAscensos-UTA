using SGA.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGA.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Teacher entity
    /// </summary>
    public interface ITeacherRepository
    {
        /// <summary>
        /// Gets a teacher by ID
        /// </summary>
        Task<Teacher?> GetByIdAsync(int id);
        
        /// <summary>
        /// Gets a teacher by identification number (cédula)
        /// </summary>
        Task<Teacher?> GetByIdentificationNumberAsync(string identificationNumber);
        
        /// <summary>
        /// Gets all teachers
        /// </summary>
        Task<IEnumerable<Teacher>> GetAllAsync();
        
        /// <summary>
        /// Adds a new teacher
        /// </summary>
        Task AddAsync(Teacher teacher);
        
        /// <summary>
        /// Updates an existing teacher
        /// </summary>
        Task UpdateAsync(Teacher teacher);
        
        /// <summary>
        /// Saves changes to the database
        /// </summary>
        Task SaveChangesAsync();
    }
}
