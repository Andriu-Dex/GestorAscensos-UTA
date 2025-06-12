using SGA.Domain.Enums;
using System;
using System.Collections.Generic;

namespace SGA.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for PromotionRequest entity
    /// </summary>
    public class PromotionRequestDto
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public string? TeacherFullName { get; set; }
        public AcademicRank CurrentRank { get; set; }
        public AcademicRank TargetRank { get; set; }
        public PromotionRequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string? Comments { get; set; }
        
        // Nuevas propiedades
        public int? DocumentId { get; set; }
        public string? DocumentName { get; set; }
        
        public int? ReviewerId { get; set; }
        public string? ReviewerFullName { get; set; }
        
        public IEnumerable<PromotionObservationDto> Observations { get; set; } = new List<PromotionObservationDto>();
    }
}
