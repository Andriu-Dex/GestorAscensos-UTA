using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Application.DTOs.Auth;
using SGA.Application.Interfaces;
using System.Security.Claims;

namespace SGA.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IDocenteService _docenteService;

    public AuthController(IAuthService authService, IDocenteService docenteService)
    {
        _authService = authService;
        _docenteService = docenteService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            // DEMO: Datos hardcodeados para demostración
            if (request.Email == "docente@uta.edu.ec" && request.Password == "123456")
            {
                return Ok(new LoginResponse
                {
                    Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkb2NlbnRlQHV0YS5lZHUuZWMiLCJlbWFpbCI6ImRvY2VudGVAdXRhLmVkdS5lYyIsInJvbGUiOiJEb2NlbnRlIiwibmFtZSI6IkRyLiBKdWFuIFDDqXJleiIsImV4cCI6MTc0NTgxNDEwMH0.demo-token-for-presentation",
                    Email = "docente@uta.edu.ec",
                    Role = "Docente",
                    FullName = "Dr. Juan Pérez",
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                });
            }
            
            if (request.Email == "admin@uta.edu.ec" && request.Password == "admin123")
            {
                return Ok(new LoginResponse
                {
                    Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZG1pbkB1dGEuZWR1LmVjIiwiZW1haWwiOiJhZG1pbkB1dGEuZWR1LmVjIiwicm9sZSI6IkFkbWluaXN0cmFkb3IiLCJuYW1lIjoiQWRtaW5pc3RyYWRvciBTaXN0ZW1hIiwiZXhwIjoxNzQ1ODE0MTAwfQ.demo-admin-token-for-presentation",
                    Email = "admin@uta.edu.ec",
                    Role = "Administrador",
                    FullName = "Administrador Sistema",
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                });
            }

            // Para otros usuarios, usar el servicio real
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = "Credenciales inválidas. Use: docente@uta.edu.ec/123456 o admin@uta.edu.ec/admin123" });
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

    [HttpGet("test")]
    public ActionResult Test()
    {
        return Ok(new { message = "API funcionando correctamente", timestamp = DateTime.UtcNow });
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
                    correoInstitucional = empleado.CorreoInstitucional,
                    celular = empleado.Celular,
                    cargoActual = empleado.CargoActual,
                    facultad = empleado.Facultad,
                    nivelAcademico = empleado.NivelAcademico,
                    fechaNombramiento = empleado.FechaNombramiento,
                    direccion = empleado.Direccion,
                    fechaNacimiento = empleado.FechaNacimiento,
                    estadoCivil = empleado.EstadoCivil,
                    tipoContrato = empleado.TipoContrato
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult> GetCurrentUser()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var docente = await _docenteService.GetDocenteByEmailAsync(email);
            if (docente == null)
                return NotFound("Docente no encontrado");

            return Ok(new
            {
                id = docente.Id,
                nombres = docente.Nombres,
                apellidos = docente.Apellidos,
                cedula = docente.Cedula,
                email = docente.Email,
                telefonoContacto = "", // Se puede agregar este campo al modelo si es necesario
                facultadId = 1, // Por defecto, se puede obtener de la base de datos si es necesario
                facultad = "Facultad de Ingeniería", // String simple para compatibilidad
                facultadInfo = new { id = 1, nombre = "Facultad de Ingeniería" }, // Objeto separado
                nivelActual = int.Parse(docente.NivelActual.Replace("Titular", "")),
                fechaIngresoNivelActual = docente.FechaInicioNivelActual
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
