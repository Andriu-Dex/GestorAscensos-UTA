using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SGA.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SGA.Application.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
        // Priorizar variables de entorno
        _secretKey = Environment.GetEnvironmentVariable("SGA_JWT_SECRET_KEY") 
                    ?? _configuration["JWT:SecretKey"] 
                    ?? "DefaultSecretKeyForDevelopment123456789012345678901234567890";
        _issuer = Environment.GetEnvironmentVariable("SGA_JWT_ISSUER")
                 ?? _configuration["JWT:Issuer"] 
                 ?? "SGA.Api";
        _audience = Environment.GetEnvironmentVariable("SGA_JWT_AUDIENCE")
                   ?? _configuration["JWT:Audience"] 
                   ?? "SGA.Client";
                   
        // Debug: Mostrar longitud de la clave para verificar
        Console.WriteLine($"[DEBUG JWT] SecretKey length: {_secretKey.Length}");
        Console.WriteLine($"[DEBUG JWT] Using issuer: {_issuer}");
        Console.WriteLine($"[DEBUG JWT] Using audience: {_audience}");
    }

    public string GenerateToken(Guid userId, string email, string role, string? cedula = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        if (!string.IsNullOrEmpty(cedula))
        {
            claims.Add(new Claim("cedula", cedula));
        }

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public bool ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public Dictionary<string, string> GetClaimsFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jsonToken = tokenHandler.ReadJwtToken(token);

        var claims = new Dictionary<string, string>();
        foreach (var claim in jsonToken.Claims)
        {
            claims[claim.Type] = claim.Value;
        }

        return claims;
    }
}
