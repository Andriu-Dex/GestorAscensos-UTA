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
            
            // Verificar si el token está expirado antes de enviarlo
            if (!string.IsNullOrWhiteSpace(token) && IsTokenExpired(token))
            {
                Console.WriteLine("[TOKEN DEBUG] Token expirado detectado antes de la petición");
                // Token expirado, manejarlo antes de la petición
                await HandleTokenExpiration();
                // Continuar con la petición sin token para que el servidor responda 401
                token = null;
            }
            
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            // Verificar si la respuesta es 401 (Unauthorized) y manejar la expiración del token
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Console.WriteLine("[TOKEN DEBUG] Respuesta 401 detectada");
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
                Console.WriteLine("[TOKEN DEBUG] Manejando expiración de token desde MessageHandler...");

                // Usar el AuthService para el manejo centralizado
                using var scope = _serviceProvider.CreateScope();
                var authService = scope.ServiceProvider.GetService<AuthService>();
                
                if (authService != null)
                {
                    Console.WriteLine("[TOKEN DEBUG] Delegando a AuthService.HandleTokenExpiration");
                    await authService.HandleTokenExpiration();
                }
                else
                {
                    Console.WriteLine("[TOKEN DEBUG] AuthService no disponible, fallback manual");
                    // Fallback manual si AuthService no está disponible
                    var authStateProvider = scope.ServiceProvider.GetService<AuthenticationStateProvider>() as ApiAuthenticationStateProvider;
                    var navigationManager = scope.ServiceProvider.GetService<NavigationManager>();
                    
                    // Limpiar datos de autenticación
                    await _localStorage.RemoveItemAsync("authToken");
                    authStateProvider?.MarkUserAsLoggedOut();
                    
                    // Redirigir al login
                    navigationManager?.NavigateTo("/login", true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TOKEN DEBUG] Error manejando expiración de token: {ex.Message}");
                
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

        private bool IsTokenExpired(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return true;

                // Parsear el JWT para obtener la fecha de expiración
                var parts = token.Split('.');
                if (parts.Length != 3)
                    return true;

                var payload = parts[1];
                // Agregar padding si es necesario
                switch (payload.Length % 4)
                {
                    case 2: payload += "=="; break;
                    case 3: payload += "="; break;
                }

                var jsonBytes = Convert.FromBase64String(payload);
                var jsonString = System.Text.Encoding.UTF8.GetString(jsonBytes);
                
                // Buscar el campo exp en el JSON
                if (jsonString.Contains("\"exp\""))
                {
                    var expIndex = jsonString.IndexOf("\"exp\":");
                    if (expIndex >= 0)
                    {
                        var expStart = jsonString.IndexOf(':', expIndex) + 1;
                        var expEnd = jsonString.IndexOfAny(new char[] { ',', '}' }, expStart);
                        if (expEnd > expStart)
                        {
                            var expString = jsonString.Substring(expStart, expEnd - expStart).Trim();
                            if (long.TryParse(expString, out var exp))
                            {
                                var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp);
                                Console.WriteLine($"[TOKEN DEBUG] Token expira en: {expirationTime}, Ahora: {DateTimeOffset.UtcNow}");
                                return expirationTime <= DateTimeOffset.UtcNow;
                            }
                        }
                    }
                }
                
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TOKEN DEBUG] Error verificando expiración: {ex.Message}");
                return true;
            }
        }
    }
}
