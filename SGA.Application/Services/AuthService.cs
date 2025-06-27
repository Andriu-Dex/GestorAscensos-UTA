using SGA.Application.DTOs.Auth;
using SGA.Application.DTOs.Docentes;
using SGA.Application.Interfaces;
using SGA.Application.Interfaces.Repositories;
using SGA.Domain.Entities;
using SGA.Domain.Entities.External;
using SGA.Domain.Enums;
using BCrypt.Net;
using DocenteDto = SGA.Application.DTOs.Docentes.DocenteDto;

namespace SGA.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IDocenteRepository _docenteRepository;
    private readonly IJwtService _jwtService;
    private readonly IAuditoriaService _auditoriaService;
    private readonly ITTHHService _tthhService;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IDocenteRepository docenteRepository,
        IJwtService jwtService,
        IAuditoriaService auditoriaService,
        ITTHHService tthhService)
    {
        _usuarioRepository = usuarioRepository;
        _docenteRepository = docenteRepository;
        _jwtService = jwtService;
        _auditoriaService = auditoriaService;
        _tthhService = tthhService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);
        
        if (usuario == null || !usuario.EstaActivo)
        {
            throw new UnauthorizedAccessException("Credenciales inválidas");
        }

        // Verificar si el usuario está bloqueado
        if (usuario.UltimoBloqueado.HasValue && 
            usuario.UltimoBloqueado.Value.AddMinutes(15) > DateTime.UtcNow &&
            usuario.IntentosLogin >= 3)
        {
            throw new UnauthorizedAccessException("Usuario bloqueado temporalmente");
        }

        // Verificar contraseña
        Console.WriteLine($"[DEBUG] Password from request: '{request.Password}'");
        Console.WriteLine($"[DEBUG] Hash from database: '{usuario.PasswordHash}'");
        Console.WriteLine($"[DEBUG] Hash length: {usuario.PasswordHash?.Length}");
        
        bool passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash);
        Console.WriteLine($"[DEBUG] Password verification result: {passwordValid}");
        
        if (!passwordValid)
        {
            usuario.IntentosLogin++;
            if (usuario.IntentosLogin >= 3)
            {
                usuario.UltimoBloqueado = DateTime.UtcNow;
            }
            await _usuarioRepository.UpdateAsync(usuario);
            
            await _auditoriaService.RegistrarAccionAsync("LOGIN_FALLIDO", usuario.Id.ToString(), 
                usuario.Email, "Usuario", null, null, null);
            
            throw new UnauthorizedAccessException("Credenciales inválidas");
        }

        // Login exitoso - resetear intentos fallidos
        usuario.IntentosLogin = 0;
        usuario.UltimoBloqueado = null;
        usuario.UltimoLogin = DateTime.UtcNow;
        await _usuarioRepository.UpdateAsync(usuario);

        var docente = await _docenteRepository.GetByUsuarioIdAsync(usuario.Id);
        var token = _jwtService.GenerateToken(usuario.Id, usuario.Email, usuario.Rol.ToString(), docente?.Cedula);
        var refreshToken = _jwtService.GenerateRefreshToken();

        await _auditoriaService.RegistrarAccionAsync("LOGIN_EXITOSO", usuario.Id.ToString(), 
            usuario.Email, "Usuario", null, null, null);
        
        return new LoginResponse
        {
            Token = token,
            RefreshToken = refreshToken,
            Expiration = DateTime.UtcNow.AddHours(8),
            Usuario = new UsuarioDto
            {
                Id = usuario.Id,
                Email = usuario.Email,
                Rol = usuario.Rol.ToString(),
                Docente = docente != null ? new DocenteDto
                {
                    Id = docente.Id,
                    Cedula = docente.Cedula,
                    Nombres = docente.Nombres,
                    Apellidos = docente.Apellidos,
                    Email = docente.Email,
                    NivelActual = docente.NivelActual.ToString(),
                    FechaInicioNivelActual = docente.FechaInicioNivelActual,
                    FechaUltimoAscenso = docente.FechaUltimoAscenso,
                    NombreCompleto = docente.NombreCompleto
                } : null
            }
        };
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        // Primero validar que la cédula existe en TTHH
        var empleadoTTHH = await _tthhService.GetEmpleadoByCedulaAsync(request.Cedula);
        if (empleadoTTHH == null)
        {
            throw new ArgumentException("La cédula no se encuentra registrada en la base de datos de Talento Humano");
        }

        // Verificar si ya existe usuario con este correo institucional
        var usuarioExistente = await _usuarioRepository.GetByEmailAsync(empleadoTTHH.CorreoInstitucional);
        if (usuarioExistente != null)
        {
            throw new ArgumentException("Ya existe un usuario con este correo institucional");
        }

        // Verificar si ya existe docente con esta cédula
        var docenteExistente = await _docenteRepository.GetByCedulaAsync(request.Cedula);
        if (docenteExistente != null)
        {
            throw new ArgumentException("Ya existe un docente registrado con esta cédula");
        }

        // Crear usuario con correo institucional
        var usuario = new Usuario
        {
            Email = empleadoTTHH.CorreoInstitucional, // Usar correo institucional
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Rol = RolUsuario.Docente,
            EstaActivo = true
        };

        usuario = await _usuarioRepository.CreateAsync(usuario);

        // Mapear el nivel titular desde TTHH
        var nivelTitular = MapearNivelTitular(empleadoTTHH.CargoActual);

        // Crear docente con datos de TTHH
        var docente = new Docente
        {
            Cedula = empleadoTTHH.Cedula,
            Nombres = empleadoTTHH.Nombres,
            Apellidos = empleadoTTHH.Apellidos,
            Email = empleadoTTHH.CorreoInstitucional, // Usar correo institucional
            NivelActual = nivelTitular,
            FechaInicioNivelActual = empleadoTTHH.FechaNombramiento,
            UsuarioId = usuario.Id,
            EstaActivo = true
        };

        docente = await _docenteRepository.CreateAsync(docente);

        await _auditoriaService.RegistrarAccionAsync("REGISTRO_USUARIO", usuario.Id.ToString(), 
            usuario.Email, "Usuario", null, null, null);

        // Generar tokens
        var token = _jwtService.GenerateToken(usuario.Id, usuario.Email, usuario.Rol.ToString());
        var refreshToken = _jwtService.GenerateRefreshToken();

        return new LoginResponse
        {
            Token = token,
            RefreshToken = refreshToken,
            Expiration = DateTime.UtcNow.AddHours(8),
            Usuario = new UsuarioDto
            {
                Id = usuario.Id,
                Email = usuario.Email,
                Rol = usuario.Rol.ToString(),
                Docente = new DocenteDto
                {
                    Id = docente.Id,
                    Cedula = docente.Cedula,
                    Nombres = docente.Nombres,
                    Apellidos = docente.Apellidos,
                    Email = docente.Email,
                    NivelActual = docente.NivelActual.ToString(),
                    FechaInicioNivelActual = docente.FechaInicioNivelActual,
                    FechaUltimoAscenso = docente.FechaUltimoAscenso,
                    NombreCompleto = docente.NombreCompleto
                }
            }
        };
    }

    private NivelTitular MapearNivelTitular(string cargoActual)
    {
        return cargoActual.ToLower() switch
        {
            var cargo when cargo.Contains("titular 1") => NivelTitular.Titular1,
            var cargo when cargo.Contains("titular 2") => NivelTitular.Titular2,
            var cargo when cargo.Contains("titular 3") => NivelTitular.Titular3,
            var cargo when cargo.Contains("titular 4") => NivelTitular.Titular4,
            var cargo when cargo.Contains("titular 5") => NivelTitular.Titular5,
            _ => NivelTitular.Titular1 // Por defecto
        };
    }

    public async Task<LoginResponse> RefreshTokenAsync(string refreshToken)
    {
        await Task.CompletedTask;
        // Implementar lógica de refresh token
        throw new NotImplementedException("Refresh token no implementado aún");
    }

    public async Task LogoutAsync(string email)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(email);
        if (usuario != null)
        {
            await _auditoriaService.RegistrarAccionAsync("LOGOUT", usuario.Id.ToString(), 
                usuario.Email, "Usuario", null, null, null);
        }
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        await Task.CompletedTask;
        return _jwtService.ValidateToken(token);
    }

    public async Task<EmpleadoTTHH?> ValidateCedulaAsync(string cedula)
    {
        return await _tthhService.GetEmpleadoByCedulaAsync(cedula);
    }
}
