using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SGA.Application.Services;
using SGA.Domain.Entities;

namespace SGA.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IDocenteService _docenteService;
        private readonly IDatosTTHHService _datosTTHHService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly IAntiforgery _antiforgery;

        public AuthController(
            IDocenteService docenteService,
            IDatosTTHHService datosTTHHService,
            IConfiguration configuration,
            ILogger<AuthController> logger,
            IAntiforgery antiforgery)
        {
            _docenteService = docenteService;
            _datosTTHHService = datosTTHHService;
            _configuration = configuration;
            _logger = logger;
            _antiforgery = antiforgery;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var docente = await _docenteService.GetDocenteByUsernameAsync(model.Username);
                if (docente == null)
                {
                    _logger.LogWarning($"Intento de login con usuario inexistente: {model.Username}");
                    return Unauthorized(new { message = "Correo o contraseña incorrectos" });
                }

                // Verificar si el usuario está bloqueado
                if (docente.Bloqueado)
                {
                    if (docente.FechaBloqueo.HasValue && docente.FechaBloqueo.Value.AddMinutes(15) > DateTime.Now)
                    {
                        // Todavía está bloqueado
                        var tiempoRestante = (docente.FechaBloqueo.Value.AddMinutes(15) - DateTime.Now).Minutes;
                        return Unauthorized(new { message = $"Usuario bloqueado temporalmente. Intente nuevamente en {tiempoRestante} minutos." });
                    }
                    else
                    {
                        // Ya pasó el tiempo de bloqueo, desbloqueamos
                        docente.Bloqueado = false;
                        docente.IntentosFallidos = 0;
                        await _docenteService.UpdateDocenteAsync(docente);
                    }
                }

                // Verificar contraseña
                if (!VerifyPasswordHash(model.Password, docente.PasswordHash))
                {
                    // Incrementar intentos fallidos
                    docente.IntentosFallidos++;
                    
                    // Bloquear después de 3 intentos fallidos
                    if (docente.IntentosFallidos >= 3)
                    {
                        docente.Bloqueado = true;
                        docente.FechaBloqueo = DateTime.Now;
                        _logger.LogWarning($"Usuario bloqueado por múltiples intentos fallidos: {model.Username}");
                    }

                    await _docenteService.UpdateDocenteAsync(docente);

                    if (docente.Bloqueado)
                    {
                        return Unauthorized(new { message = "Usuario bloqueado temporalmente por múltiples intentos fallidos. Intente nuevamente en 15 minutos." });
                    }

                    return Unauthorized(new { message = "Correo o contraseña incorrectos" });
                }

                // Restablecer intentos fallidos
                docente.IntentosFallidos = 0;
                await _docenteService.UpdateDocenteAsync(docente);

                // Generar token JWT
                var token = GenerateJwtToken(docente);

                _logger.LogInformation($"Login exitoso para el usuario: {model.Username}");

                return Ok(new
                {
                    token,
                    user = new
                    {
                        id = docente.Id,
                        username = docente.NombreUsuario,
                        nombres = docente.Nombres,
                        apellidos = docente.Apellidos,
                        esAdmin = docente.EsAdministrador
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error en el login: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar formato de correo electrónico
                if (!model.Email.EndsWith("@uta.edu.ec", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { message = "El correo electrónico debe ser institucional (@uta.edu.ec)" });
                }

                // Verificar si ya existe un usuario con ese correo
                var existingUserByEmail = await _docenteService.GetDocenteByEmailAsync(model.Email);
                if (existingUserByEmail != null)
                {
                    return BadRequest(new { message = "El correo electrónico ya está registrado" });
                }

                // Verificar si ya existe un usuario con ese nombre de usuario
                var existingUser = await _docenteService.GetDocenteByUsernameAsync(model.Username);
                if (existingUser != null)
                {
                    return BadRequest(new { message = "El nombre de usuario ya está en uso" });
                }

                // Verificar si la cédula es válida
                if (!ValidarCedulaEcuatoriana(model.Cedula))
                {
                    return BadRequest(new { message = "El número de cédula ingresado no es válido" });
                }

                // Verificar si ya existe un docente con esa cédula
                var existingDocente = await _docenteService.GetDocenteByCedulaAsync(model.Cedula);
                if (existingDocente != null)
                {
                    return BadRequest(new { message = "Ya existe un docente registrado con esta cédula" });
                }

                // Verificar si existen datos de TTHH para esta cédula
                var datosTTHH = await _datosTTHHService.GetDatosByCedulaAsync(model.Cedula);
                
                // Si no existen datos en TTHH, registrarlos
                if (datosTTHH == null)
                {
                    var nuevosDatosTTHH = new DatosTTHH
                    {
                        Cedula = model.Cedula,
                        Nombres = model.Nombres,
                        Apellidos = model.Apellidos,
                        Facultad = model.Facultad,
                        FechaRegistro = DateTime.Now
                    };
                    await _datosTTHHService.CreateDatosTTHHAsync(nuevosDatosTTHH);
                }

                // Crear nuevo docente (sin departamento)
                var docente = new Docente
                {
                    Cedula = model.Cedula,
                    Nombres = model.Nombres,
                    Apellidos = model.Apellidos,
                    Email = model.Email,
                    TelefonoContacto = model.Telefono,
                    Facultad = model.Facultad,
                    NivelActual = 1, // Por defecto, todos inician en Titular 1
                    FechaIngresoNivelActual = DateTime.Now,
                    NombreUsuario = model.Username,
                    PasswordHash = HashPassword(model.Password),
                    IntentosFallidos = 0,
                    Bloqueado = false,
                    EsAdministrador = false, // Por defecto, los usuarios registrados son docentes, no administradores
                    FechaRegistro = DateTime.Now
                };

                await _docenteService.CreateDocenteAsync(docente);

                _logger.LogInformation($"Nuevo docente registrado: {model.Username}");

                return Ok(new { message = "Registro exitoso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error en el registro: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
        
        [HttpGet("verificar-email/{email}")]
        public async Task<IActionResult> VerificarEmail(string email)
        {
            try
            {
                var docente = await _docenteService.GetDocenteByEmailAsync(email);
                if (docente != null)
                {
                    return BadRequest(new { message = "El correo electrónico ya está registrado" });
                }
                
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar correo: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
        
        [HttpGet("verificar-cedula/{cedula}")]
        public async Task<IActionResult> VerificarCedula(string cedula)
        {
            try
            {
                if (!ValidarCedulaEcuatoriana(cedula))
                {
                    return BadRequest(new { message = "El número de cédula ingresado no es válido" });
                }
                
                var docente = await _docenteService.GetDocenteByCedulaAsync(cedula);
                if (docente != null)
                {
                    return BadRequest(new { message = "Ya existe un docente registrado con esta cédula" });
                }
                
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar cédula: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int id))
                {
                    return Unauthorized(new { message = "Token no válido" });
                }

                var docente = await _docenteService.GetDocenteByIdAsync(id);
                if (docente == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                return Ok(new
                {
                    id = docente.Id,
                    cedula = docente.Cedula,
                    nombres = docente.Nombres,
                    apellidos = docente.Apellidos,
                    email = docente.Email,
                    facultad = docente.Facultad,
                    nivelActual = docente.NivelActual,
                    fechaIngresoNivelActual = docente.FechaIngresoNivelActual,
                    esAdmin = docente.EsAdministrador
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el usuario actual: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("csrf-token")]
        public IActionResult GetCsrfToken()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            return Ok(new { token = tokens.RequestToken });
        }

        private string GenerateJwtToken(Docente docente)
        {
            var jwtSecret = _configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT:Secret no está configurado");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, docente.Id.ToString()),
                new Claim(ClaimTypes.Name, docente.NombreUsuario),
                new Claim(ClaimTypes.GivenName, docente.Nombres),
                new Claim(ClaimTypes.Surname, docente.Apellidos),
                new Claim("cedula", docente.Cedula)
            };

            if (docente.EsAdministrador)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Administrador"));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "Docente"));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string HashPassword(string password)
        {
            // En una aplicación real, usar un algoritmo más seguro como BCrypt o Argon2
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private static bool VerifyPasswordHash(string password, string storedHash)
        {
            // En una aplicación real, usar BCrypt.Verify o equivalente
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            var computedHash = Convert.ToBase64String(hash);
            return computedHash == storedHash;
        }
        
        private bool ValidarCedulaEcuatoriana(string cedula)
        {
            // Validar que tenga 10 dígitos
            if (cedula.Length != 10 || !long.TryParse(cedula, out _))
                return false;

            // Validar provincia (códigos del 01 al 24)
            int provincia = int.Parse(cedula.Substring(0, 2));
            if (provincia < 1 || provincia > 24)
                return false;

            // Validar tercer dígito (menor a 6 para personas naturales)
            int tercerDigito = int.Parse(cedula.Substring(2, 1));
            if (tercerDigito > 6)
                return false;

            // Algoritmo de validación del último dígito
            int[] coeficientes = { 2, 1, 2, 1, 2, 1, 2, 1, 2 };
            int verificador = int.Parse(cedula.Substring(9, 1));
            int suma = 0;

            for (int i = 0; i < 9; i++)
            {
                int valor = int.Parse(cedula.Substring(i, 1)) * coeficientes[i];
                suma += (valor >= 10) ? valor - 9 : valor;
            }

            int digitoVerificador = (suma % 10 != 0) ? 10 - (suma % 10) : 0;
            return verificador == digitoVerificador;
        }
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "La cédula es obligatoria")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "La cédula debe tener 10 dígitos")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "La cédula debe contener solo números")]
        public string Cedula { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los nombres son obligatorios")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Los nombres deben tener entre 2 y 100 caracteres")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Los apellidos deben tener entre 2 y 100 caracteres")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@uta\.edu\.ec$", ErrorMessage = "El correo debe ser institucional (@uta.edu.ec)")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(15, MinimumLength = 7, ErrorMessage = "El teléfono debe tener entre 7 y 15 caracteres")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "La facultad es obligatoria")]
        public string Facultad { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "El nombre de usuario debe tener entre 4 y 50 caracteres")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "La contraseña debe contener al menos una letra mayúscula, una minúscula, un número y un carácter especial")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La confirmación de contraseña es obligatoria")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
