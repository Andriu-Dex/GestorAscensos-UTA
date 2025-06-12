using System.ComponentModel.DataAnnotations;

namespace SGA.Application.DTOs
{
    /// <summary>
    /// DTO para agregar observaciones a documentos
    /// </summary>
    public class AddDocumentObservationDto
    {
        [Required]
        public string Description { get; set; }
        
        [Required]
        public int ReviewerId { get; set; }
    }
}
