using System;
using System.Collections.Generic;

namespace SGA.Domain.Entities
{
    /// <summary>
    /// Represents a requirement for academic promotion
    /// </summary>
    public class Requirement
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int YearsInCurrentRank { get; private set; }
        public int RequiredWorks { get; private set; }
        public decimal MinimumEvaluationScore { get; private set; }
        public int RequiredTrainingHours { get; private set; }
        public int RequiredResearchMonths { get; private set; }
        
        // Navigation property
        public ICollection<Document> Documents { get; private set; } = new List<Document>();
        
        // Private constructor for EF Core
        private Requirement() { }
        
        public Requirement(
            string name,
            int yearsInCurrentRank,
            int requiredWorks,
            decimal minimumEvaluationScore,
            int requiredTrainingHours,
            int requiredResearchMonths)
        {
            ValidateName(name);
            ValidateYears(yearsInCurrentRank);
            ValidateWorks(requiredWorks);
            ValidateEvaluationScore(minimumEvaluationScore);
            ValidateTrainingHours(requiredTrainingHours);
            ValidateResearchMonths(requiredResearchMonths);
            
            Name = name;
            YearsInCurrentRank = yearsInCurrentRank;
            RequiredWorks = requiredWorks;
            MinimumEvaluationScore = minimumEvaluationScore;
            RequiredTrainingHours = requiredTrainingHours;
            RequiredResearchMonths = requiredResearchMonths;
        }
        
        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Requirement name cannot be empty", nameof(name));
        }
        
        private void ValidateYears(int years)
        {
            if (years < 0)
                throw new ArgumentException("Years cannot be negative", nameof(years));
        }
        
        private void ValidateWorks(int works)
        {
            if (works < 0)
                throw new ArgumentException("Required works cannot be negative", nameof(works));
        }
        
        private void ValidateEvaluationScore(decimal score)
        {
            if (score < 0 || score > 100)
                throw new ArgumentException("Evaluation score must be between 0 and 100", nameof(score));
        }
        
        private void ValidateTrainingHours(int hours)
        {
            if (hours < 0)
                throw new ArgumentException("Training hours cannot be negative", nameof(hours));
        }
        
        private void ValidateResearchMonths(int months)
        {
            if (months < 0)
                throw new ArgumentException("Research months cannot be negative", nameof(months));
        }
        
        public void Update(
            string name,
            int yearsInCurrentRank,
            int requiredWorks,
            decimal minimumEvaluationScore,
            int requiredTrainingHours,
            int requiredResearchMonths)
        {
            ValidateName(name);
            ValidateYears(yearsInCurrentRank);
            ValidateWorks(requiredWorks);
            ValidateEvaluationScore(minimumEvaluationScore);
            ValidateTrainingHours(requiredTrainingHours);
            ValidateResearchMonths(requiredResearchMonths);
            
            Name = name;
            YearsInCurrentRank = yearsInCurrentRank;
            RequiredWorks = requiredWorks;
            MinimumEvaluationScore = minimumEvaluationScore;
            RequiredTrainingHours = requiredTrainingHours;
            RequiredResearchMonths = requiredResearchMonths;
        }
    }
}
