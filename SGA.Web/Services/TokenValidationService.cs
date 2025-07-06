using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace SGA.Web.Services
{
    public interface ITokenValidationService : IDisposable
    {
        Task StartPeriodicValidation();
        void StopPeriodicValidation();
        Task<bool> IsTokenValid();
        event Func<Task>? OnTokenExpired;
    }

    public class TokenValidationService : ITokenValidationService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly ApiAuthenticationStateProvider _authStateProvider;
        private Timer? _validationTimer;
        private bool _isValidating = false;

        public event Func<Task>? OnTokenExpired;

        public TokenValidationService(
            ILocalStorageService localStorage,
            AuthenticationStateProvider authStateProvider)
        {
            _localStorage = localStorage;
            _authStateProvider = authStateProvider as ApiAuthenticationStateProvider 
                ?? throw new ArgumentException("AuthenticationStateProvider debe ser ApiAuthenticationStateProvider");
        }

        public async Task StartPeriodicValidation()
        {
            // Verificar el token cada 60 segundos
            _validationTimer = new Timer(async _ => await ValidateTokenPeriodically(), 
                null, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60));
            
            // Verificar inmediatamente
            await ValidateTokenPeriodically();
        }

        public void StopPeriodicValidation()
        {
            _validationTimer?.Dispose();
            _validationTimer = null;
        }

        public async Task<bool> IsTokenValid()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                
                if (string.IsNullOrWhiteSpace(token))
                    return false;

                // Usar el método existente para validar el token
                return await _authStateProvider.ValidateToken();
            }
            catch
            {
                return false;
            }
        }

        private async Task ValidateTokenPeriodically()
        {
            if (_isValidating)
                return;

            _isValidating = true;

            try
            {
                var isValid = await IsTokenValid();
                
                if (!isValid)
                {
                    // Token expirado, disparar evento
                    if (OnTokenExpired != null)
                    {
                        await OnTokenExpired.Invoke();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en validación periódica de token: {ex.Message}");
            }
            finally
            {
                _isValidating = false;
            }
        }

        public void Dispose()
        {
            StopPeriodicValidation();
        }
    }
}
