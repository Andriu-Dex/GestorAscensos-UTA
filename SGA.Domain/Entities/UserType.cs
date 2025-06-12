using System.Collections.Generic;

namespace SGA.Domain.Entities
{
    /// <summary>
    /// Represents a user type in the system
    /// </summary>
    public class UserType
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        
        // Navigation property
        public ICollection<Teacher> Teachers { get; private set; } = new List<Teacher>();
        
        // Private constructor for EF Core
        private UserType() { }
        
        public UserType(string name, string description)
        {
            ValidateName(name);
            Name = name;
            Description = description;
        }
        
        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new System.ArgumentException("User type name cannot be empty", nameof(name));
        }
        
        public void UpdateDescription(string description)
        {
            Description = description;
        }
    }
}
