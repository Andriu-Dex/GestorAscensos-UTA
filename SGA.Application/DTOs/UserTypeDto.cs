namespace SGA.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object para UserType
    /// </summary>
    public class UserTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
