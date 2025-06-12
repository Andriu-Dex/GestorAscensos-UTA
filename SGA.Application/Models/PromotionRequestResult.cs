namespace SGA.Application.Models
{
    public class PromotionRequestResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? PromotionRequestId { get; set; }
    }
}
