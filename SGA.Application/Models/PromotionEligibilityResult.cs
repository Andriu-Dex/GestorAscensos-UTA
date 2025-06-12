using System.Collections.Generic;
using SGA.Domain.Enums;

namespace SGA.Application.Models
{
    public class PromotionEligibilityResult
    {
        public bool IsEligible { get; set; }
        public string Message { get; set; } = string.Empty;
        public AcademicRank CurrentRank { get; set; }
        public AcademicRank TargetRank { get; set; }
        public Dictionary<string, bool> RequirementsMet { get; set; } = new Dictionary<string, bool>();
    }
}
