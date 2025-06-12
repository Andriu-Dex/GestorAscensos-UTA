namespace SGA.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object para Requirement
    /// </summary>
    public class RequirementDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int YearsInCurrentRank { get; set; }
        public int RequiredWorks { get; set; }
        public decimal MinimumEvaluationScore { get; set; }
        public int RequiredTrainingHours { get; set; }
        public int RequiredResearchMonths { get; set; }
    }
}
