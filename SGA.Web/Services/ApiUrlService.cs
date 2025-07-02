using Blazored.LocalStorage;
using Microsoft.Extensions.Configuration;

namespace SGA.Web.Services;

/// <summary>
/// Implementación del servicio para gestión de URLs del API
/// </summary>
public class ApiUrlService : IApiUrlService
{
    private readonly IConfiguration _configuration;
    private readonly ILocalStorageService _localStorage;
    private readonly string _apiBaseUrl;

    public ApiUrlService(IConfiguration configuration, ILocalStorageService localStorage)
    {
        _configuration = configuration;
        _localStorage = localStorage;
        
        // Configurar URL base del API - usar configuración o valor por defecto
        _apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5115/";
        
        // Asegurar que termine con /
        if (!_apiBaseUrl.EndsWith("/"))
        {
            _apiBaseUrl += "/";
        }
    }

    public string GetApiBaseUrl()
    {
        return _apiBaseUrl;
    }

    public string BuildApiUrl(string relativeEndpoint)
    {
        if (string.IsNullOrWhiteSpace(relativeEndpoint))
        {
            return _apiBaseUrl;
        }

        // Remover / inicial si existe
        if (relativeEndpoint.StartsWith("/"))
        {
            relativeEndpoint = relativeEndpoint.Substring(1);
        }

        return $"{_apiBaseUrl}{relativeEndpoint}";
    }

    public async Task<string> BuildAuthorizedApiUrlAsync(string relativeEndpoint)
    {
        var baseUrl = BuildApiUrl(relativeEndpoint);
        
        try
        {
            // Obtener token de autorización del localStorage
            var token = await _localStorage.GetItemAsync<string>("authToken");
            
            if (!string.IsNullOrWhiteSpace(token))
            {
                // Agregar token como parámetro de consulta para iframe
                var separator = baseUrl.Contains("?") ? "&" : "?";
                return $"{baseUrl}{separator}access_token={Uri.EscapeDataString(token)}";
            }
            
            return baseUrl;
        }
        catch (Exception)
        {
            // En caso de error, retornar URL sin token
            return baseUrl;
        }
    }
}
