using System;

namespace SGA.Domain.Entities
{
    /// <summary>
    /// Represents an observation about a document
    /// </summary>
    public class DocumentObservation
    {
        public int Id { get; private set; }
        public string Description { get; private set; }
        public DateTime CreatedAt { get; private set; }
        
        // Foreign keys
        public int ReviewerId { get; private set; }
        public Teacher Reviewer { get; private set; }
        
        public int DocumentId { get; private set; }
        public Document Document { get; private set; }
        
        // Private constructor for EF Core
        private DocumentObservation() { }
        
        public DocumentObservation(
            string description,
            Teacher reviewer,
            Document document)
        {
            ValidateDescription(description);
            
            Description = description;
            CreatedAt = DateTime.UtcNow;
            
            Reviewer = reviewer ?? throw new ArgumentNullException(nameof(reviewer));
            ReviewerId = reviewer.Id;
            
            Document = document ?? throw new ArgumentNullException(nameof(document));
            DocumentId = document.Id;
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
