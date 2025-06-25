using SGA.Application.DTOs.Auth;
using SGA.Domain.Entities.External;

namespace SGA.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<LoginResponse> RegisterAsync(RegisterRequest request);
    Task<LoginResponse> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(string email);
    Task<bool> ValidateTokenAsync(string token);
    Task<EmpleadoTTHH?> ValidateCedulaAsync(string cedula);
}
