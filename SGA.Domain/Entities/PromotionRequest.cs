using SGA.Domain.Enums;
using System;

namespace SGA.Domain.Entities
{
    /// <summary>
    /// Represents a promotion request from one academic rank to another
    /// </summary>
    public class PromotionRequest
    {
        public int Id { get; set; } // Cambiado a público para pruebas
        public int TeacherId { get; set; } // Cambiado a público para pruebas
        public Teacher? Teacher { get; set; } // Cambiado a público para pruebas
        public AcademicRank CurrentRank { get; set; } // Cambiado a público para pruebas
        public AcademicRank TargetRank { get; set; } // Cambiado a público para pruebas
        public PromotionRequestStatus Status { get; set; } // Cambiado a público para pruebas
        public DateTime CreatedAt { get; set; } // Renombrado de RequestDate para consistencia
        public DateTime? ProcessedDate { get; set; } // Cambiado a público para pruebas
        public string? Comments { get; set; } // Renombrado de RejectionReason para ser más general
        
        // Constructor vacío para pruebas y EF Core
        public PromotionRequest() { }
        
        /// <summary>
        /// Creates a new promotion request with all required properties
        /// </summary>
        public static PromotionRequest Create(Teacher teacher, AcademicRank targetRank)
        {
            if (teacher == null)
                throw new ArgumentNullException(nameof(teacher));
                
            if ((int)targetRank != (int)teacher.CurrentRank + 1)
                throw new ArgumentException("Target rank must be the next sequential rank", nameof(targetRank));
                
            return new PromotionRequest
            {
                Teacher = teacher,
                TeacherId = teacher.Id,
                CurrentRank = teacher.CurrentRank,
                TargetRank = targetRank,
                Status = PromotionRequestStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
        }
        
        /// <summary>
        /// Sets the request status to InProgress with optional comments
        /// </summary>
        public void MarkInProgress(string? comments = null)
        {
            if (Status == PromotionRequestStatus.Approved || Status == PromotionRequestStatus.Rejected)
                throw new InvalidOperationException("Cannot change status of finalized requests");
                
            Status = PromotionRequestStatus.InProgress;
            Comments = comments;
        }
        
        /// <summary>
        /// Approves the promotion request
        /// </summary>
        public void Approve(string? comments = null)
        {
            if (Status == PromotionRequestStatus.Approved)
                throw new InvalidOperationException("Request is already approved");
                
            if (Status == PromotionRequestStatus.Rejected)
                throw new InvalidOperationException("Cannot approve a rejected request");
                
            Status = PromotionRequestStatus.Approved;
            ProcessedDate = DateTime.UtcNow;
            Comments = comments;
        }
        
        /// <summary>
        /// Rejects the promotion request with a reason
        /// </summary>
        public void Reject(string comments)
        {
            if (Status == PromotionRequestStatus.Approved)
                throw new InvalidOperationException("Cannot reject an approved request");
                
            if (Status == PromotionRequestStatus.Rejected)
                throw new InvalidOperationException("Request is already rejected");
                
            if (string.IsNullOrWhiteSpace(comments))
                throw new ArgumentException("Rejection reason cannot be empty", nameof(comments));
                
            Status = PromotionRequestStatus.Rejected;
            Comments = comments;
            ProcessedDate = DateTime.UtcNow;
        }
    }
}
