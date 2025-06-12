using SGA.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGA.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for AcademicDegree entity
    /// </summary>
    public interface IAcademicDegreeRepository
    {
        /// <summary>
        /// Gets an academic degree by ID
        /// </summary>
        Task<AcademicDegree> GetByIdAsync(int id);
        
        /// <summary>
        /// Gets all academic degrees for a specific teacher
        /// </summary>
        Task<IEnumerable<AcademicDegree>> GetByTeacherIdAsync(int teacherId);
        
        /// <summary>
        /// Gets all academic degrees
        /// </summary>
        Task<IEnumerable<AcademicDegree>> GetAllAsync();
        
        /// <summary>
        /// Adds a new academic degree
        /// </summary>
        Task AddAsync(AcademicDegree academicDegree);
        
        /// <summary>
        /// Updates an existing academic degree
        /// </summary>
        Task UpdateAsync(AcademicDegree academicDegree);
        
        /// <summary>
        /// Deletes an academic degree
        /// </summary>
        Task DeleteAsync(AcademicDegree academicDegree);
        
        /// <summary>
        /// Saves changes to the database
        /// </summary>
        Task SaveChangesAsync();
    }
}
