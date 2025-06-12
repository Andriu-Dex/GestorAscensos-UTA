using System;

namespace SGA.Domain.Entities
{
    /// <summary>
    /// Represents an observation about a promotion request
    /// </summary>
    public class PromotionObservation
    {
        public int Id { get; private set; }
        public string Description { get; private set; }
        public DateTime CreatedAt { get; private set; }
        
        // Foreign key
        public int PromotionRequestId { get; private set; }
        public PromotionRequest PromotionRequest { get; private set; }
        
        // Private constructor for EF Core
        private PromotionObservation() { }
        
        public PromotionObservation(
            string description,
            PromotionRequest promotionRequest)
        {
            ValidateDescription(description);
            
            Description = description;
            CreatedAt = DateTime.UtcNow;
            
            PromotionRequest = promotionRequest ?? throw new ArgumentNullException(nameof(promotionRequest));
            PromotionRequestId = promotionRequest.Id;
        }
        
        private void ValidateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Observation description cannot be empty", nameof(description));
        }
        
        public void UpdateDescription(string description)
        {
            ValidateDescription(description);
            Description = description;
        }
    }
}
