using System;

namespace SGA.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object para DocumentObservation
    /// </summary>
    public class DocumentObservationDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        
        public int DocumentId { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        
        public int ReviewerId { get; set; }
        public string ReviewerFullName { get; set; } = string.Empty;
    }
}
