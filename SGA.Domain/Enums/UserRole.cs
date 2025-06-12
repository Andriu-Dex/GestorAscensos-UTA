namespace SGA.Domain.Enums
{
    /// <summary>
    /// Represents the roles available in the system
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// Teacher role with limited permissions
        /// </summary>
        Teacher = 1,
        
        /// <summary>
        /// Administrator role with full permissions
        /// </summary>
        Administrator = 2
    }
}
