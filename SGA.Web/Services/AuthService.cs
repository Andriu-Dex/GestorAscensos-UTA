using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using SGA.Web.Models;

namespace SGA.Web.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly SGA.Web.Services.ILocalStorageService _localStorage;
        private UserInfoModel? _currentUser;

        public AuthService(HttpClient httpClient, AuthenticationStateProvider authStateProvider, SGA.Web.Services.ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
            _localStorage = localStorage;
        }

        public async Task<LoginResult> Login(LoginModel loginModel)
        {
            try
            {
                Console.WriteLine($"[AUTH DEBUG] Iniciando login para: {loginModel.Email}");
                Console.WriteLine($"[AUTH DEBUG] URL Base: {_httpClient.BaseAddress}");
                
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginModel);
                
                Console.WriteLine($"[AUTH DEBUG] Status Code: {response.StatusCode}");
                Console.WriteLine($"[AUTH DEBUG] IsSuccessStatusCode: {response.IsSuccessStatusCode}");
                
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[AUTH DEBUG] Response Content: {content}");
                
                if (response.IsSuccessStatusCode)
                {
                    // Parsear la respuesta de la API
                    var apiResponse = JsonSerializer.Deserialize<ApiLoginResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    Console.WriteLine($"[AUTH DEBUG] API Response Parsed: {apiResponse != null}");
                    if (apiResponse?.Token != null)
                    {
                        var tokenPreview = apiResponse.Token.Length > 20 ? apiResponse.Token.Substring(0, 20) + "..." : apiResponse.Token;
                        Console.WriteLine($"[AUTH DEBUG] Token: {tokenPreview}");
                    }
                    Console.WriteLine($"[AUTH DEBUG] Usuario: {apiResponse?.Usuario?.Email}");

                    if (apiResponse != null && !string.IsNullOrEmpty(apiResponse.Token))
                    {
                        Console.WriteLine("[AUTH DEBUG] Guardando token en localStorage");
                        await _localStorage.SetItemAsync("authToken", apiResponse.Token);
                        
                        Console.WriteLine("[AUTH DEBUG] Marcando usuario como autenticado");
                        ((ApiAuthenticationStateProvider)_authStateProvider).MarkUserAsAuthenticated(apiResponse.Token);
                        
                        var userInfoModel = new UserInfoModel
                        {
                            Id = apiResponse.Usuario?.Id ?? Guid.Empty,
                            Username = apiResponse.Usuario?.Email ?? "",
                            Email = apiResponse.Usuario?.Email ?? "",
                            Nombres = apiResponse.Usuario?.Docente?.Nombres ?? "",
                            Apellidos = apiResponse.Usuario?.Docente?.Apellidos ?? "",
                            Cedula = apiResponse.Usuario?.Docente?.Cedula ?? "",
                            Facultad = "", // Se puede obtener de otro endpoint si es necesario
                            Departamento = "",
                            NivelActual = 1, // Por defecto
                            FechaIngresoNivelActual = apiResponse.Usuario?.Docente?.FechaInicioNivelActual ?? DateTime.Now,
                            EsAdmin = apiResponse.Usuario?.Rol == "Admin"
                        };
                        
                        // Guardar la información del usuario en memoria
                        _currentUser = userInfoModel;
                        
                        var loginResult = new LoginResult 
                        { 
                            Success = true, 
                            Token = apiResponse.Token,
                            Message = "Login exitoso",
                            User = userInfoModel
                        };
                        
                        Console.WriteLine("[AUTH DEBUG] Login exitoso, retornando resultado");
                        return loginResult;
                    }
                    else
                    {
                        Console.WriteLine("[AUTH DEBUG] Token vacío o nulo en la respuesta");
                        return new LoginResult { Success = false, Message = "No se recibió token de autenticación" };
                    }
                }
                else
                {
                    // Error HTTP, usar mensaje genérico
                    
                    string errorMessage = "Credenciales inválidas";
                    try
                    {
                        var error = JsonSerializer.Deserialize<ErrorResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        errorMessage = error?.Message ?? errorMessage;
                    }
                    catch (Exception ex)
                    {
                        errorMessage = "Error al procesar la respuesta del servidor";
                    }
                    
                    return new LoginResult { Success = false, Message = errorMessage };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AUTH DEBUG] Excepción en Login: {ex.Message}");
                Console.WriteLine($"[AUTH DEBUG] Stack Trace: {ex.StackTrace}");
                return new LoginResult { Success = false, Message = $"Error de conexión: {ex.Message}" };
            }
        }

        public async Task<RegisterResult> Register(RegisterModel registerModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerModel);

                if (response.IsSuccessStatusCode)
                {
                    return new RegisterResult { Success = true, Message = "Registro exitoso" };
                }

                string errorMessage = "Error de registro";
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var error = JsonSerializer.Deserialize<ErrorResponse>(errorContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        errorMessage = error?.Message ?? errorMessage;
                    }
                    catch
                    {
                        errorMessage = "Error al procesar la respuesta del servidor";
                    }
                }

                return new RegisterResult { Success = false, Message = errorMessage };
            }
            catch (Exception ex)
            {
                return new RegisterResult { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<UserInfoModel?> GetUserInfo()
        {
            try
            {
                // Si ya tenemos la información del usuario en memoria, la devolvemos
                if (_currentUser != null)
                {
                    return _currentUser;
                }
                
                // Verificar si hay un token válido
                var token = await _localStorage.GetItemAsync<string>("authToken");
                
                if (string.IsNullOrWhiteSpace(token))
                {
                    return null;
                }
                
                // Si hay token pero no información del usuario, 
                // intentamos extraerla del token JWT
                try
                {
                    var claims = ((ApiAuthenticationStateProvider)_authStateProvider).ParseClaimsFromJwt(token);
                    if (claims != null && claims.Any())
                    {
                        var email = claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value ?? "";
                        var role = claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value ?? "";
                        
                        _currentUser = new UserInfoModel
                        {
                            Id = Guid.Empty,
                            Username = email,
                            Email = email,
                            Nombres = "Usuario", // Por defecto, se actualizará con datos reales
                            Apellidos = "",
                            Cedula = "",
                            Facultad = "",
                            Departamento = "",
                            NivelActual = 1,
                            FechaIngresoNivelActual = DateTime.Now,
                            EsAdmin = role == "Admin"
                        };
                        
                        return _currentUser;
                    }
                }
                catch (Exception ex)
                {
                    // Error parsing JWT claims
                }

                return null;
            }
            catch (Exception ex)
            {
                // Error in GetUserInfo
                return null;
            }
        }

        public async Task Logout()
        {
            _currentUser = null; // Limpiar información del usuario
            await _localStorage.RemoveItemAsync("authToken");
            ((ApiAuthenticationStateProvider)_authStateProvider).MarkUserAsLoggedOut();
        }

        public async Task<RegisterResult> VerificarEmail(string email)
        {
            try
            {
                // En un entorno real, esto debería consultar la API
                // var response = await _httpClient.GetAsync($"api/auth/verificar-email/{email}");
                
                // Simulación para desarrollo
                await Task.Delay(500); // Simular latencia de red
                
                // Simular que el correo juan.perez@uta.edu.ec ya está registrado
                if (email.ToLower() == "juan.perez@uta.edu.ec")
                {
                    return new RegisterResult 
                    { 
                        Success = false, 
                        Message = "Ya existe un usuario registrado con este correo electrónico" 
                    };
                }
                
                return new RegisterResult { Success = true };
            }
            catch (Exception ex)
            {
                return new RegisterResult 
                { 
                    Success = false, 
                    Message = $"Error al verificar el correo: {ex.Message}" 
                };
            }
        }

        public async Task<RegisterResult> VerificarCedula(string cedula)
        {
            try
            {
                // En un entorno real, esto debería consultar la API
                // var response = await _httpClient.GetAsync($"api/auth/verificar-cedula/{cedula}");
                
                // Simulación para desarrollo
                await Task.Delay(500); // Simular latencia de red
                
                // Simular que la cédula 1804567891 ya está registrada
                if (cedula == "1804567891")
                {
                    return new RegisterResult 
                    { 
                        Success = false, 
                        Message = "Ya existe un usuario registrado con esta cédula" 
                    };
                }
                
                return new RegisterResult { Success = true };
            }
            catch (Exception ex)
            {
                return new RegisterResult 
                { 
                    Success = false, 
                    Message = $"Error al verificar la cédula: {ex.Message}" 
                };
            }
        }

        public async Task<bool> ValidateToken()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                
                if (string.IsNullOrWhiteSpace(token))
                {
                    return false;
                }
                
                // Configurar el header de autorización para futuras peticiones
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                // Solo verificamos si el token existe, no hacemos llamadas al servidor
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task ConfigureAuthenticationHeader()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                
                if (!string.IsNullOrWhiteSpace(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    _httpClient.DefaultRequestHeaders.Authorization = null;
                }
            }
            catch
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }
    }

    public class LoginResult
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public UserInfoModel User { get; set; } = new UserInfoModel();
        public string Message { get; set; } = string.Empty;
    }

    public class RegisterResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
    }

    // Modelos para deserializar la respuesta de la API
    public class ApiLoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public ApiUsuarioDto? Usuario { get; set; }
    }

    public class ApiUsuarioDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public ApiDocenteDto? Docente { get; set; }
    }

    public class ApiDocenteDto
    {
        public Guid Id { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NivelActual { get; set; } = string.Empty;
        public DateTime FechaInicioNivelActual { get; set; }
        public DateTime? FechaUltimoAscenso { get; set; }
    }
}
