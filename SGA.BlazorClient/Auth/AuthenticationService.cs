using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using SGA.Application.DTOs;

namespace SGA.BlazorApp.Client.Auth
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly JwtTokenHandler _tokenHandler;

        public AuthenticationService(
            HttpClient httpClient,
            AuthenticationStateProvider authStateProvider,
            JwtTokenHandler tokenHandler)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
            _tokenHandler = tokenHandler;
        }        public async Task<LoginResponseDto?> Login(LoginRequestDto request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
            
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return null; // Credenciales inválidas
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al iniciar sesión: {errorContent}");
            }
            
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
              if (loginResponse != null)
            {
                // Guardar el token
                await _tokenHandler.SaveTokenAsync(loginResponse.Token);
                
                // Guardar información del usuario
                await _tokenHandler.SaveUserInfoAsync(JsonSerializer.Serialize(loginResponse));
                
                // Notificar al proveedor de estado de autenticación
                ((JwtAuthenticationStateProvider)_authStateProvider).NotifyUserAuthentication(loginResponse.Token);
            }
            
            return loginResponse;
        }        public async Task Logout()
        {
            await _tokenHandler.RemoveTokenAsync();
            ((JwtAuthenticationStateProvider)_authStateProvider).NotifyUserLogout();
        }

        public async Task<bool> IsUserAuthenticated()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            return authState.User.Identity?.IsAuthenticated ?? false;
        }

        public async Task<string?> GetUserRole()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            return authState.User.FindFirst(ClaimTypes.Role)?.Value;
        }

        public async Task<int> GetUserId()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var userIdClaim = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            
            return 0;
        }

        public async Task<int?> GetTeacherId()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var teacherIdClaim = authState.User.FindFirst("TeacherId")?.Value;
            
            if (int.TryParse(teacherIdClaim, out int teacherId))
            {
                return teacherId;
            }
            
            return null;
        }
    }
}
