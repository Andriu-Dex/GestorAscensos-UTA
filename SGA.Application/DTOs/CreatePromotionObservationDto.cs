using System.ComponentModel.DataAnnotations;

namespace SGA.Application.DTOs
{
    /// <summary>
    /// DTO para crear observaciones de solicitudes de promoción
    /// </summary>
    public class CreatePromotionObservationDto
    {
        [Required]
        public string Description { get; set; }
        
        [Required]
        public int PromotionRequestId { get; set; }
    }
}
