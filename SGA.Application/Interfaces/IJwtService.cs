using System.Security.Claims;

namespace SGA.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(Guid userId, string email, string role);
    string GenerateRefreshToken();
    bool ValidateToken(string token);
    Dictionary<string, string> GetClaimsFromToken(string token);
}
