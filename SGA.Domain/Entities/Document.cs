using System;
using System.Collections.Generic;

namespace SGA.Domain.Entities
{
    /// <summary>
    /// Represents a document in the promotion management system
    /// </summary>
    public class Document
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string DocumentType { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public DateTime? UploadDate { get; private set; }
        public bool IsEditable { get; private set; }
        public string Department { get; private set; }
        public string IssuingInstitution { get; private set; }
        public int? DurationHours { get; private set; }
        public byte[] FileContent { get; private set; }
        
        // Foreign keys
        public int TeacherId { get; private set; }
        public Teacher Teacher { get; private set; }
        
        public int? ReviewerId { get; private set; }
        public Teacher Reviewer { get; private set; }
        
        public int? RequirementId { get; private set; }
        public Requirement Requirement { get; private set; }
        
        // Navigation properties
        public ICollection<DocumentObservation> Observations { get; private set; } = new List<DocumentObservation>();
        
        // Private constructor for EF Core
        private Document() { }
        
        public Document(
            string name,
            string description,
            string documentType,
            DateTime startDate,
            DateTime endDate,
            string department,
            string issuingInstitution,
            int? durationHours,
            byte[] fileContent,
            Teacher teacher,
            Requirement requirement = null)
        {
            ValidateName(name);
            ValidateDate(startDate, endDate);
            ValidateFileContent(fileContent);
            
            Name = name;
            Description = description;
            DocumentType = documentType;
            StartDate = startDate;
            EndDate = endDate;
            UploadDate = DateTime.UtcNow;
            IsEditable = true;
            Department = department;
            IssuingInstitution = issuingInstitution;
            DurationHours = durationHours;
            FileContent = fileContent;
            
            Teacher = teacher ?? throw new ArgumentNullException(nameof(teacher));
            TeacherId = teacher.Id;
            
            if (requirement != null)
            {
                Requirement = requirement;
                RequirementId = requirement.Id;
            }
        }
        
        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Document name cannot be empty", nameof(name));
        }
        
        private void ValidateDate(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Start date cannot be after end date");
        }
        
        private void ValidateFileContent(byte[] fileContent)
        {
            if (fileContent == null || fileContent.Length == 0)
                throw new ArgumentException("File content cannot be empty", nameof(fileContent));
        }
        
        public void AssignReviewer(Teacher reviewer)
        {
            Reviewer = reviewer ?? throw new ArgumentNullException(nameof(reviewer));
            ReviewerId = reviewer.Id;
        }
        
        public void DisableEditing()
        {
            IsEditable = false;
        }
        
        public void EnableEditing()
        {
            IsEditable = true;
        }
        
        public void AddObservation(string description, Teacher reviewer)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Observation description cannot be empty", nameof(description));
                
            var observation = new DocumentObservation(description, reviewer, this);
            Observations.Add(observation);
        }
    }
}
