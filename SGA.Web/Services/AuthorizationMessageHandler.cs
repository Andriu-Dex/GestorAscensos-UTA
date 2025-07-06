using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.Toast.Services;

namespace SGA.Web.Services
{
    public class AuthorizationMessageHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IServiceProvider _serviceProvider;
        private static bool _isHandlingTokenExpiration = false;
        private static readonly object _lockObject = new object();

        public AuthorizationMessageHandler(ILocalStorageService localStorage, IServiceProvider serviceProvider)
        {
            _localStorage = localStorage;
            _serviceProvider = serviceProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            // Verificar si la respuesta es 401 (Unauthorized) y manejar la expiración del token
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                lock (_lockObject)
                {
                    if (!_isHandlingTokenExpiration)
                    {
                        _isHandlingTokenExpiration = true;
                        // Ejecutar en segundo plano para no bloquear la respuesta HTTP
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await HandleTokenExpiration();
                            }
                            finally
                            {
                                _isHandlingTokenExpiration = false;
                            }
                        });
                    }
                }
            }

            return response;
        }

        private async Task HandleTokenExpiration()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var toastService = scope.ServiceProvider.GetService<IToastService>();
                var authStateProvider = scope.ServiceProvider.GetService<AuthenticationStateProvider>() as ApiAuthenticationStateProvider;
                var navigationManager = scope.ServiceProvider.GetService<NavigationManager>();
                var authService = scope.ServiceProvider.GetService<AuthService>();

                // Limpiar datos de autenticación
                await _localStorage.RemoveItemAsync("authToken");
                authStateProvider?.MarkUserAsLoggedOut();
                authService?.ClearUserCache();

                // Mostrar notificación al usuario con configuración personalizada
                if (toastService != null)
                {
                    toastService.ShowError("🔐 Su sesión ha expirado por motivos de seguridad. Será redirigido automáticamente al login en unos segundos...", settings =>
                    {
                        settings.Timeout = 5;
                        settings.ShowCloseButton = false;
                        settings.ShowProgressBar = true;
                        settings.Position = Blazored.Toast.Configuration.ToastPosition.TopCenter;
                    });
                }

                // Esperar un momento para que el usuario vea la notificación
                await Task.Delay(4000);

                // Redirigir al login
                navigationManager?.NavigateTo("/login", true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error manejando expiración de token: {ex.Message}");
                
                // En caso de error, intentar redirigir al login de todas formas
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var navigationManager = scope.ServiceProvider.GetService<NavigationManager>();
                    navigationManager?.NavigateTo("/login", true);
                }
                catch
                {
                    // Si todo falla, al menos limpiar el token
                    await _localStorage.RemoveItemAsync("authToken");
                }
            }
        }
    }
}
