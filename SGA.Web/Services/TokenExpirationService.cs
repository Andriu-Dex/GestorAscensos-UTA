using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.Toast.Services;

namespace SGA.Web.Services
{
    public interface ITokenExpirationService
    {
        Task HandleTokenExpiration();
        Task<bool> IsTokenExpired();
        void StartPeriodicTokenCheck();
        void StopPeriodicTokenCheck();
    }

    public class TokenExpirationService : ITokenExpirationService, IDisposable
    {
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly NavigationManager _navigationManager;
        private readonly IToastService _toastService;
        private readonly ApiAuthenticationStateProvider _apiAuthStateProvider;
        private Timer? _tokenCheckTimer;
        private static bool _isHandlingExpiration = false;

        public TokenExpirationService(
            ILocalStorageService localStorage,
            AuthenticationStateProvider authStateProvider,
            NavigationManager navigationManager,
            IToastService toastService)
        {
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
            _navigationManager = navigationManager;
            _toastService = toastService;
            _apiAuthStateProvider = authStateProvider as ApiAuthenticationStateProvider 
                ?? throw new ArgumentException("AuthenticationStateProvider debe ser ApiAuthenticationStateProvider");
        }

        public async Task HandleTokenExpiration()
        {
            if (_isHandlingExpiration)
                return;

            _isHandlingExpiration = true;

            try
            {
                // Verificar si realmente el token ha expirado
                var isExpired = await IsTokenExpired();
                if (!isExpired)
                {
                    _isHandlingExpiration = false;
                    return;
                }

                // Limpiar datos de autenticación
                await ClearAuthenticationData();

                // Mostrar notificación al usuario
                _toastService.ShowWarning("Su sesión ha expirado. Será redirigido al login.");

                // Esperar un momento para que el usuario vea la notificación
                await Task.Delay(2000);

                // Redirigir al login
                _navigationManager.NavigateTo("/login", true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error manejando expiración de token: {ex.Message}");
                // En caso de error, redirigir al login de todas formas
                _navigationManager.NavigateTo("/login", true);
            }
            finally
            {
                _isHandlingExpiration = false;
            }
        }

        public async Task<bool> IsTokenExpired()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                
                if (string.IsNullOrWhiteSpace(token))
                    return true;

                // Usar el método existente para validar el token
                return !await _apiAuthStateProvider.ValidateToken();
            }
            catch
            {
                return true;
            }
        }

        public void StartPeriodicTokenCheck()
        {
            // Verificar el token cada 30 segundos
            _tokenCheckTimer = new Timer(async _ => await CheckTokenPeriodically(), null, 
                TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

        public void StopPeriodicTokenCheck()
        {
            _tokenCheckTimer?.Dispose();
            _tokenCheckTimer = null;
        }

        private async Task CheckTokenPeriodically()
        {
            try
            {
                var isExpired = await IsTokenExpired();
                if (isExpired && !_isHandlingExpiration)
                {
                    await HandleTokenExpiration();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en verificación periódica de token: {ex.Message}");
            }
        }

        private async Task ClearAuthenticationData()
        {
            try
            {
                // Limpiar token del localStorage
                await _localStorage.RemoveItemAsync("authToken");

                // Marcar usuario como no autenticado
                _apiAuthStateProvider.MarkUserAsLoggedOut();

                // Limpiar el caché del AuthService si existe
                var authService = GetAuthService();
                authService?.ClearUserCache();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error limpiando datos de autenticación: {ex.Message}");
            }
        }

        private AuthService? GetAuthService()
        {
            try
            {
                // Intentar obtener el AuthService del DI container usando reflexión
                var serviceProviderProperty = _navigationManager.GetType()
                    .GetProperty("ServiceProvider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (serviceProviderProperty?.GetValue(_navigationManager) is IServiceProvider serviceProvider)
                {
                    return serviceProvider.GetService(typeof(AuthService)) as AuthService;
                }
            }
            catch
            {
                // Si no se puede obtener, no es crítico
            }
            return null;
        }

        public void Dispose()
        {
            StopPeriodicTokenCheck();
        }
    }
}
