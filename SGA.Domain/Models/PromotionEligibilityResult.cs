using SGA.Domain.Enums;
using System.Collections.Generic;

namespace SGA.Domain.Models
{
    public class PromotionEligibilityResult
    {
        public bool IsEligible { get; set; }
        public string? Message { get; set; }
        public AcademicRank CurrentRank { get; set; }
        public AcademicRank TargetRank { get; set; }
        public Dictionary<string, bool> RequirementsMet { get; set; } = new Dictionary<string, bool>();
    }
}
