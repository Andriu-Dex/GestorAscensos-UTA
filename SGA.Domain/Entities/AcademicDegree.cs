using System;

namespace SGA.Domain.Entities
{
    /// <summary>
    /// Represents an academic degree of a teacher
    /// </summary>
    public class AcademicDegree
    {
        public int Id { get; private set; }
        public string DegreeType { get; private set; }
        public string Title { get; private set; }
        public string IssuingInstitution { get; private set; }
        
        // Foreign key for Teacher
        public int TeacherId { get; private set; }
        public Teacher Teacher { get; private set; }
        
        // Private constructor for EF Core
        private AcademicDegree() { }
        
        public AcademicDegree(
            string degreeType,
            string title,
            string issuingInstitution,
            Teacher teacher)
        {
            ValidateDegreeType(degreeType);
            ValidateTitle(title);
            ValidateInstitution(issuingInstitution);
            
            DegreeType = degreeType;
            Title = title;
            IssuingInstitution = issuingInstitution;
            Teacher = teacher ?? throw new ArgumentNullException(nameof(teacher));
            TeacherId = teacher.Id;
        }
        
        private void ValidateDegreeType(string degreeType)
        {
            if (string.IsNullOrWhiteSpace(degreeType))
                throw new ArgumentException("Degree type cannot be empty", nameof(degreeType));
        }
        
        private void ValidateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));
        }
        
        private void ValidateInstitution(string institution)
        {
            if (string.IsNullOrWhiteSpace(institution))
                throw new ArgumentException("Institution cannot be empty", nameof(institution));
        }
        
        public void Update(string degreeType, string title, string issuingInstitution)
        {
            ValidateDegreeType(degreeType);
            ValidateTitle(title);
            ValidateInstitution(issuingInstitution);
            
            DegreeType = degreeType;
            Title = title;
            IssuingInstitution = issuingInstitution;
        }
    }
}
