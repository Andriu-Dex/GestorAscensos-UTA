using SGA.Application.DTOs.Docentes;

namespace SGA.Application.DTOs.Auth;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public UsuarioDto Usuario { get; set; } = null!;
}

public class RegisterRequest
{
    public string Cedula { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class ValidateCedulaRequest
{
    public string Cedula { get; set; } = string.Empty;
}

public class UsuarioDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public DocenteDto? Docente { get; set; }
}
