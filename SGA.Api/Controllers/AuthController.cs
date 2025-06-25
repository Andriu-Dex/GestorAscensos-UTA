using Microsoft.AspNetCore.Mvc;
using SGA.Application.DTOs.Auth;
using SGA.Application.Interfaces;

namespace SGA.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<LoginResponse>> RefreshToken([FromBody] string refreshToken)
    {
        try
        {
            var response = await _authService.RefreshTokenAsync(refreshToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        try
        {
            var email = User.Identity?.Name;
            if (!string.IsNullOrEmpty(email))
            {
                await _authService.LogoutAsync(email);
            }
            return Ok(new { message = "Logout exitoso" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate-token")]
    public async Task<ActionResult> ValidateToken([FromBody] string token)
    {
        try
        {
            var isValid = await _authService.ValidateTokenAsync(token);
            return Ok(new { isValid });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate-cedula")]
    public async Task<ActionResult> ValidateCedula([FromBody] ValidateCedulaRequest request)
    {
        try
        {
            var empleado = await _authService.ValidateCedulaAsync(request.Cedula);
            if (empleado == null)
            {
                return NotFound(new { message = "Cédula no encontrada en la base de datos de Talento Humano" });
            }

            return Ok(new
            {
                valid = true,
                empleado = new
                {
                    cedula = empleado.Cedula,
                    nombres = empleado.Nombres,
                    apellidos = empleado.Apellidos,
                    email = empleado.Email,
                    cargoActual = empleado.CargoActual,
                    nivelAcademico = empleado.NivelAcademico,
                    fechaNombramiento = empleado.FechaNombramiento
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
