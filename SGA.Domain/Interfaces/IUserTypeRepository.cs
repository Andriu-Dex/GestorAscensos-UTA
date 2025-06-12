using SGA.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGA.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for UserType entity
    /// </summary>
    public interface IUserTypeRepository
    {
        /// <summary>
        /// Gets a user type by ID
        /// </summary>
        Task<UserType> GetByIdAsync(int id);
        
        /// <summary>
        /// Gets a user type by name
        /// </summary>
        Task<UserType> GetByNameAsync(string name);
        
        /// <summary>
        /// Gets all user types
        /// </summary>
        Task<IEnumerable<UserType>> GetAllAsync();
        
        /// <summary>
        /// Adds a new user type
        /// </summary>
        Task AddAsync(UserType userType);
        
        /// <summary>
        /// Updates an existing user type
        /// </summary>
        Task UpdateAsync(UserType userType);
        
        /// <summary>
        /// Saves changes to the database
        /// </summary>
        Task SaveChangesAsync();
    }
}
