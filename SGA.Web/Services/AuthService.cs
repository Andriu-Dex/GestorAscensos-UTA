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
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginModel);                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadFromJsonAsync<LoginResult>();

                    if (content != null && !string.IsNullOrEmpty(content.Token))
                    {
                        await _localStorage.SetItemAsync("authToken", content.Token);
                        ((ApiAuthenticationStateProvider)_authStateProvider).MarkUserAsAuthenticated(content.Token);
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", content.Token);
                        content.Success = true;
                        return content;
                    }
                }

                string errorMessage = "Error de inicio de sesión";
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

                return new LoginResult { Success = false, Message = errorMessage };
            }
            catch (Exception ex)
            {
                return new LoginResult { Success = false, Message = $"Error: {ex.Message}" };
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
                var response = await _httpClient.GetAsync("api/auth/me");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserInfoModel>();
                }

                return null;
            }
            catch
            {
                return null;
            }
        }        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((ApiAuthenticationStateProvider)_authStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
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
    }public class LoginResult
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
    }    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}
