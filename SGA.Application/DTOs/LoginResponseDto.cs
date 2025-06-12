using System;

namespace SGA.Application.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string Username { get; set; }
        public string UserType { get; set; }
        public int UserId { get; set; }
        public bool IsTeacher { get; set; }
        public int? TeacherId { get; set; }
    }
}
