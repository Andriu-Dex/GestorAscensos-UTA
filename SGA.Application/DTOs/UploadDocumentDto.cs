using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace SGA.Application.DTOs
{
    /// <summary>
    /// DTO para la carga de documentos
    /// </summary>
    public class UploadDocumentDto
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        [Required]
        public string DocumentType { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        [Required]
        public string Department { get; set; }
        
        [Required]
        public string IssuingInstitution { get; set; }
        
        public int? DurationHours { get; set; }
        
        [Required]
        public int TeacherId { get; set; }
        
        public int? RequirementId { get; set; }
        
        [Required]
        public IFormFile File { get; set; }
    }
}
