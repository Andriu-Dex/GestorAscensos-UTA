using System;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using SGA.Application.Services;

namespace SGA.Infrastructure.Services
{
    public class BcryptPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, 12); // 12 is the work factor
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
