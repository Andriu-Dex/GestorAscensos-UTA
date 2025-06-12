using System;

namespace SGA.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object para Document
    /// </summary>
    public class DocumentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? UploadDate { get; set; }
        public bool IsEditable { get; set; }        public string Department { get; set; } = string.Empty;
        public string IssuingInstitution { get; set; } = string.Empty;
        public int? DurationHours { get; set; }
        
        // Propiedades para manejar archivos (útiles para upload/download)
        public byte[]? FileContent { get; set; }
        public string? FileName { get; set; }
        public long? FileSize { get; set; }
        public string? ContentType { get; set; }
        
        public int TeacherId { get; set; }
        public string TeacherFullName { get; set; } = string.Empty;
        
        public int? ReviewerId { get; set; }
        public string ReviewerFullName { get; set; } = string.Empty;
        
        public int? RequirementId { get; set; }
        public string RequirementName { get; set; } = string.Empty;
    }
}
