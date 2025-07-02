namespace SGA.Web.Services;

/// <summary>
/// Servicio para gestión centralizada de URLs del API
/// </summary>
public interface IApiUrlService
{
    /// <summary>
    /// Obtiene la URL base del API
    /// </summary>
    string GetApiBaseUrl();
    
    /// <summary>
    /// Construye una URL completa del API a partir de un endpoint relativo
    /// </summary>
    /// <param name="relativeEndpoint">Endpoint relativo (ej: "api/certificados-capacitacion/admin/ver/123")</param>
    /// <returns>URL completa del API</returns>
    string BuildApiUrl(string relativeEndpoint);
    
    /// <summary>
    /// Construye una URL completa del API con token de autorización para visualización en iframe
    /// </summary>
    /// <param name="relativeEndpoint">Endpoint relativo</param>
    /// <returns>URL completa con token incluido como parámetro</returns>
    Task<string> BuildAuthorizedApiUrlAsync(string relativeEndpoint);
}
