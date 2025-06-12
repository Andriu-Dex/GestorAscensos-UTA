using SGA.Domain.Enums;

namespace SGA.Application.DTOs
{
    /// <summary>
    /// Request model for processing a promotion request
    /// </summary>
    public class ProcessPromotionRequestDto
    {
        public PromotionRequestStatus NewStatus { get; set; }
        public string? Comments { get; set; }
    }
}
