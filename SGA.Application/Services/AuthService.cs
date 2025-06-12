using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SGA.Application.DTOs;
using SGA.Domain.Entities;
using SGA.Domain.Interfaces;

namespace SGA.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(
            IUserRepository userRepository,
            IConfiguration configuration,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            // Get user by username
            var user = await _userRepository.GetByUsernameAsync(loginRequest.Username);
            
            if (user == null)
            {
                return null;
            }

            // Check if account is locked
            if (user.IsLockedOut())
            {
                throw new InvalidOperationException($"Account is locked. Try again after {user.LockoutEnd.Value}");
            }

            // Verify password
            if (!_passwordHasher.VerifyPassword(loginRequest.Password, user.PasswordHash))
            {
                // Increment failed login attempts
                user.IncrementFailedLoginAttempts();
                await _userRepository.UpdateAsync(user);
                
                return null;
            }

            // Update last login time and reset failed attempts
            user.UpdateLastLogin();
            await _userRepository.UpdateAsync(user);

            // Generate token
            var token = GenerateJwtToken(user);

            return new LoginResponseDto
            {
                Token = token.token,
                Expiration = token.expiration,
                Username = user.Username,
                UserType = user.UserType?.Name,
                UserId = user.Id,
                IsTeacher = user.Teacher != null,
                TeacherId = user.Teacher?.Id
            };
        }

        public async Task<bool> RegisterAsync(RegisterUserDto registerRequest)
        {
            // Check if username already exists
            if (await _userRepository.UsernameExistsAsync(registerRequest.Username))
            {
                throw new InvalidOperationException("Username already exists");
            }

            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(registerRequest.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            // Hash password
            string passwordHash = _passwordHasher.HashPassword(registerRequest.Password);

            // Create user
            var user = new User(
                registerRequest.Username,
                passwordHash,
                registerRequest.Email,
                registerRequest.UserTypeId
            );

            await _userRepository.CreateAsync(user);
            return true;
        }

        private (string token, DateTime expiration) GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            
            var tokenExpirationHours = int.Parse(_configuration["Jwt:ExpirationHours"] ?? "24");
            var expiration = DateTime.UtcNow.AddHours(tokenExpirationHours);
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.UserType?.Name ?? "Unknown"),
                new Claim("UserTypeId", user.UserTypeId.ToString())
            };

            if (user.Teacher != null)
            {
                claims.Add(new Claim("TeacherId", user.Teacher.Id.ToString()));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiration,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (tokenHandler.WriteToken(token), expiration);
        }
    }
}
