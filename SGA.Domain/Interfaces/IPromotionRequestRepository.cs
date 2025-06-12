using SGA.Domain.Entities;
using SGA.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGA.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for PromotionRequest entity
    /// </summary>
    public interface IPromotionRequestRepository
    {
        /// <summary>
        /// Gets a promotion request by ID
        /// </summary>
        Task<PromotionRequest> GetByIdAsync(int id);
          /// <summary>
        /// Gets all promotion requests for a specific teacher
        /// </summary>
        Task<IEnumerable<PromotionRequest>> GetByTeacherIdAsync(int teacherId);
        
        /// <summary>
        /// Gets active (pending or in progress) promotion request for a specific teacher
        /// </summary>
        Task<PromotionRequest> GetActiveRequestByTeacherIdAsync(int teacherId);
        
        /// <summary>
        /// Gets all promotion requests with a specific status
        /// </summary>
        Task<IEnumerable<PromotionRequest>> GetByStatusAsync(PromotionRequestStatus status);
        
        /// <summary>
        /// Gets all promotion requests
        /// </summary>
        Task<IEnumerable<PromotionRequest>> GetAllAsync();
        
        /// <summary>
        /// Adds a new promotion request
        /// </summary>
        Task AddAsync(PromotionRequest promotionRequest);
        
        /// <summary>
        /// Updates an existing promotion request
        /// </summary>
        Task UpdateAsync(PromotionRequest promotionRequest);
        
        /// <summary>
        /// Saves changes to the database
        /// </summary>
        Task SaveChangesAsync();
    }
}
