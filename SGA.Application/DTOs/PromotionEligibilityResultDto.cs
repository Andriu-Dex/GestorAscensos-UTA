using SGA.Domain.Enums;
using System.Collections.Generic;

namespace SGA.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for PromotionEligibilityResult
    /// </summary>
    public class PromotionEligibilityResultDto
    {
        public bool IsEligible { get; set; }
        public string? Message { get; set; }
        public AcademicRank CurrentRank { get; set; }
        public AcademicRank TargetRank { get; set; }
        public Dictionary<string, bool> RequirementsMet { get; set; } = new Dictionary<string, bool>();
    }
}
