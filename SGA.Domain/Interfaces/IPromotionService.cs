using SGA.Domain.Entities;
using SGA.Domain.Enums;
using SGA.Domain.Models;
using System.Threading.Tasks;

namespace SGA.Domain.Interfaces
{
    /// <summary>
    /// Service interface for handling teacher promotions
    /// </summary>
    public interface IPromotionService
    {
        /// <summary>
        /// Checks if a teacher is eligible for promotion and returns detailed requirements status
        /// </summary>
        Task<PromotionEligibilityResult> CheckEligibilityAsync(int teacherId);
          /// <summary>
        /// Creates a new promotion request for a teacher
        /// </summary>
        Task<PromotionRequestResult> CreatePromotionRequestAsync(int teacherId);

        /// <summary>
        /// Creates a new promotion request for a teacher with an associated document
        /// </summary>
        Task<PromotionRequestResult> CreatePromotionRequestWithDocumentAsync(int teacherId, int documentId);
          /// <summary>
        /// Processes a promotion request by changing its status (Approve, Reject, InProgress)
        /// </summary>
        Task<PromotionRequestResult> ProcessPromotionRequestAsync(int requestId, PromotionRequestStatus newStatus, string? comments = null);
    }
}
