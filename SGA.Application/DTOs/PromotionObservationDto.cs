using System;

namespace SGA.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object para PromotionObservation
    /// </summary>
    public class PromotionObservationDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        
        public int PromotionRequestId { get; set; }
    }
}
