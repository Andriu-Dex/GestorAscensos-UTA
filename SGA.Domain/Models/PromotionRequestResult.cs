namespace SGA.Domain.Models
{
    public class PromotionRequestResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int? PromotionRequestId { get; set; }
    }
}
