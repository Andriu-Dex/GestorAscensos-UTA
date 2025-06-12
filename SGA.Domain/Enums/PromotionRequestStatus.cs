namespace SGA.Domain.Enums
{
    /// <summary>
    /// Represents the status of a promotion request
    /// </summary>
    public enum PromotionRequestStatus
    {
        /// <summary>
        /// The request is pending review
        /// </summary>
        Pending = 1,
        
        /// <summary>
        /// The request is currently being processed
        /// </summary>
        InProgress = 2,
        
        /// <summary>
        /// The request has been approved
        /// </summary>
        Approved = 3,
        
        /// <summary>
        /// The request has been rejected
        /// </summary>
        Rejected = 4
    }
}
