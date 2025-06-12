using SGA.Domain.Enums;
using System;
using System.Collections.Generic;

namespace SGA.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for Teacher entity
    /// </summary>
    public class TeacherDto
    {
        public int Id { get; set; }
        public string? IdentificationNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public AcademicRank CurrentRank { get; set; }
        public DateTime StartDateInCurrentRank { get; set; }
        public int DaysInCurrentRank { get; set; }
        public int UserTypeId { get; set; }
        public string UserTypeName { get; set; } = string.Empty;
        public int Works { get; set; }
        public decimal EvaluationScore { get; set; }
        public int TrainingHours { get; set; }
        public int ResearchMonths { get; set; }
        public int YearsInCurrentRank { get; set; }
        
        // No incluimos Password por seguridad
        
        public IEnumerable<AcademicDegreeDto> AcademicDegrees { get; set; } = new List<AcademicDegreeDto>();
        
        public string FullName => $"{FirstName} {LastName}";
    }
}
