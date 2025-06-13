using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SGA.Application.DTOs;
using SGA.Application.Services;
using System;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SGA.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto loginRequest)
        {
            try
            {
                var result = await _authService.LoginAsync(loginRequest);
                if (result == null)
                {
                    return Unauthorized(new { message = "Nombre de usuario o contraseña incorrectos" });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Error al procesar la solicitud de inicio de sesión" });
            }
        }

        [HttpPost("register")]
        [Authorize(Roles = "Administrador")] // Solo administradores pueden registrar usuarios
        public async Task<ActionResult> Register(RegisterUserDto registerRequest)
        {
            try
            {
                var result = await _authService.RegisterAsync(registerRequest);
                return Ok(new { message = "Usuario registrado exitosamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Error al procesar la solicitud de registro" });
            }
        }

        [HttpPost("test-token")]
        [AllowAnonymous]
        public ActionResult<object> GenerateTestToken()
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("C1ave5ecreta5uper5egur4Par4JWT2025_SistemaGestionAscensos");
                
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "1"),
                        new Claim(ClaimTypes.Name, "testuser"),
                        new Claim(ClaimTypes.Role, "Administrador")
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = "SistemaGestionAscensos",
                    Audience = "SistemaGestionAscensosClients"
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Ok(new 
                { 
                    token = tokenString,
                    expires = tokenDescriptor.Expires,
                    message = "JWT Test Token Generated Successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error generating test token", error = ex.Message });
            }
        }

        [HttpGet("test-protected")]
        [Authorize]
        public ActionResult<object> TestProtectedEndpoint()
        {
            var currentUser = User.Identity?.Name;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            return Ok(new 
            { 
                message = "Successfully accessed protected endpoint!",
                username = currentUser,
                userId = userId,
                role = userRole,
                timestamp = DateTime.UtcNow
            });
        }
    }
}
