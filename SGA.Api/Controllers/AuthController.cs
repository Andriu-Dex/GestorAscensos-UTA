using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Application.DTOs;
using SGA.Application.Services;
using System;
using System.Threading.Tasks;

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
    }
}
