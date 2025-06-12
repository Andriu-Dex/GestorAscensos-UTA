using SGA.Domain.Enums;
using System;
using System.Collections.Generic;

namespace SGA.Domain.Entities
{
    /// <summary>
    /// Represents a teacher in the promotion management system
    /// </summary>
    public class Teacher
    {        public int Id { get; set; } // Cambiado a público para pruebas
        public string? IdentificationNumber { get; set; } // Cambiado a público para pruebas
        public string? FirstName { get; set; } // Cambiado a público para pruebas
        public string? LastName { get; set; } // Cambiado a público para pruebas
        public string? Email { get; set; } // Cambiado a público para pruebas
        public AcademicRank CurrentRank { get; set; } // Cambiado a público para pruebas
        public DateTime StartDateInCurrentRank { get; set; } // Cambiado a público para pruebas
          // Indicators for promotion requirements
        public int Works { get; set; } // Cambiado de AcademicWorksCount para simplificar
        public decimal EvaluationScore { get; set; } // Percentage (0-100)
        public int TrainingHours { get; set; }
        public int ResearchMonths { get; set; }
        public int YearsInCurrentRank { get; set; } // Nueva propiedad para simplificar pruebas
        
        // Navigation property for promotion requests
        public ICollection<PromotionRequest> PromotionRequests { get; private set; } = new List<PromotionRequest>();
        
        // Private constructor for EF Core
        private Teacher() { }
        
        public Teacher(
            string identificationNumber,
            string firstName,
            string lastName,
            string email,
            AcademicRank initialRank)
        {
            ValidateIdentificationNumber(identificationNumber);
            ValidateName(firstName, nameof(firstName));
            ValidateName(lastName, nameof(lastName));
            ValidateEmail(email);
            
            IdentificationNumber = identificationNumber;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            CurrentRank = initialRank;
            StartDateInCurrentRank = DateTime.UtcNow;
              // Initialize counters
            Works = 0;
            EvaluationScore = 0;
            TrainingHours = 0;
            ResearchMonths = 0;
        }
        
        /// <summary>
        /// Updates the teacher's academic indicators
        /// </summary>
        public void UpdateIndicators(
            int academicWorksCount,
            decimal evaluationScore,
            int trainingHours,
            int researchMonths)
        {
            if (academicWorksCount < 0)
                throw new ArgumentException("Academic works count cannot be negative", nameof(academicWorksCount));
                
            if (evaluationScore < 0 || evaluationScore > 100)
                throw new ArgumentException("Evaluation score must be between 0 and 100", nameof(evaluationScore));
                
            if (trainingHours < 0)
                throw new ArgumentException("Training hours cannot be negative", nameof(trainingHours));
                
            if (researchMonths < 0)
                throw new ArgumentException("Research months cannot be negative", nameof(researchMonths));                
            Works = academicWorksCount;
            EvaluationScore = evaluationScore;
            TrainingHours = trainingHours;
            ResearchMonths = researchMonths;
        }
          /// <summary>
        /// Promote to the next rank
        /// </summary>
        public void PromoteToNextRank()
        {
            if (CurrentRank == AcademicRank.Titular5)
                throw new InvalidOperationException("Cannot promote beyond the highest rank");
                
            CurrentRank = GetNextRank();
            
            // Reset counters as per requirements
            Works = 0;
            TrainingHours = 0;
            ResearchMonths = 0;
            StartDateInCurrentRank = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Gets the next academic rank
        /// </summary>
        public AcademicRank GetNextRank()
        {
            if (CurrentRank == AcademicRank.Titular5)
                return AcademicRank.Titular5; // No higher rank
                
            return (AcademicRank)((int)CurrentRank + 1);
        }
        
        /// <summary>
        /// Checks if the teacher meets all requirements for promotion and returns detailed result
        /// </summary>
        public (bool IsEligible, string Message, Dictionary<string, bool> RequirementsMet) CheckPromotionEligibility()
        {
            if (CurrentRank == AcademicRank.Titular5)
                return (false, "Cannot promote beyond the highest rank", new Dictionary<string, bool>());
                
            var requirementsMet = new Dictionary<string, bool>();
            
            // Years in current rank (always 4 years for all levels)
            var yearsRequirement = YearsInCurrentRank >= 4;
            requirementsMet.Add("YearsInCurrentRank", yearsRequirement);
            
            // Evaluation score (always 75% for all levels)
            var scoreRequirement = EvaluationScore >= 75;
            requirementsMet.Add("EvaluationScore", scoreRequirement);
            
            // Works, training hours, and research months vary by current rank
            bool worksRequirement = false;
            bool trainingHoursRequirement = false;
            bool researchMonthsRequirement = false;
            
            switch (CurrentRank)
            {
                case AcademicRank.Titular1:
                    worksRequirement = Works >= 1;
                    trainingHoursRequirement = TrainingHours >= 96;
                    researchMonthsRequirement = true; // Not required
                    break;
                    
                case AcademicRank.Titular2:
                    worksRequirement = Works >= 2;
                    trainingHoursRequirement = TrainingHours >= 96;
                    researchMonthsRequirement = ResearchMonths >= 12;
                    break;
                    
                case AcademicRank.Titular3:
                    worksRequirement = Works >= 3;
                    trainingHoursRequirement = TrainingHours >= 128;
                    researchMonthsRequirement = ResearchMonths >= 24;
                    break;
                    
                case AcademicRank.Titular4:
                    worksRequirement = Works >= 5;
                    trainingHoursRequirement = TrainingHours >= 160;
                    researchMonthsRequirement = ResearchMonths >= 24;
                    break;
            }
            
            requirementsMet.Add("Works", worksRequirement);
            requirementsMet.Add("TrainingHours", trainingHoursRequirement);
            requirementsMet.Add("ResearchMonths", researchMonthsRequirement);
            
            bool isEligible = yearsRequirement && scoreRequirement && 
                              worksRequirement && trainingHoursRequirement && 
                              researchMonthsRequirement;
            
            string message = isEligible 
                ? "Eligible for promotion" 
                : "Not eligible for promotion. Requirements not met.";
                
            return (isEligible, message, requirementsMet);
        }
        
        /// <summary>
        /// Creates a new promotion request
        /// </summary>
        public PromotionRequest CreatePromotionRequest()
        {
            var eligibilityCheck = CheckPromotionEligibility();
            if (!eligibilityCheck.IsEligible)
                throw new InvalidOperationException("Teacher does not meet requirements for promotion");
                
            if (CurrentRank == AcademicRank.Titular5)
                throw new InvalidOperationException("Cannot request promotion beyond the highest rank");
                
            var nextRank = GetNextRank();
            
            return new PromotionRequest
            {
                TeacherId = Id,
                CurrentRank = CurrentRank,
                TargetRank = nextRank,
                Status = PromotionRequestStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
        }
        
        /// <summary>
        /// Updates personal information
        /// </summary>
        public void UpdatePersonalInfo(string firstName, string lastName, string email)
        {
            ValidateName(firstName, nameof(firstName));
            ValidateName(lastName, nameof(lastName));
            ValidateEmail(email);
            
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }
        
        #region Validation Methods
        
        private void ValidateIdentificationNumber(string identificationNumber)
        {
            if (string.IsNullOrWhiteSpace(identificationNumber))
                throw new ArgumentException("Identification number cannot be empty", nameof(identificationNumber));
                
            // Ecuador cédula validation logic could be implemented here
        }
        
        private void ValidateName(string name, string paramName)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", paramName);
                
            if (name.Length < 2)
                throw new ArgumentException("Name must be at least 2 characters", paramName);
        }
        
        private void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));
                
            // Simple email validation
            if (!email.Contains("@") || !email.Contains("."))
                throw new ArgumentException("Invalid email format", nameof(email));
        }
        
        #endregion
    }
}
