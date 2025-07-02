using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;

namespace SGA.Api.Middleware;

/// <summary>
/// Middleware que permite autenticación JWT mediante token en query string
/// para endpoints específicos como visualización de PDFs en iframes
/// </summary>
public class QueryStringAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<QueryStringAuthenticationMiddleware> _logger;

    // Endpoints que soportan autenticación por query string
    private static readonly HashSet<string> _allowedPaths = new(StringComparer.OrdinalIgnoreCase)
    {
        "/api/certificados-capacitacion/admin/ver",
        "/api/documentos/ver",
        "/api/documentos/preview"
    };

    public QueryStringAuthenticationMiddleware(RequestDelegate next, ILogger<QueryStringAuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Verificar si el endpoint soporta autenticación por query string
        if (ShouldProcessRequest(context.Request))
        {
            ProcessTokenFromQueryString(context);
        }

        await _next(context);
    }

    private bool ShouldProcessRequest(HttpRequest request)
    {
        // Solo procesar si no hay token en header Authorization
        if (request.Headers.ContainsKey("Authorization"))
        {
            return false;
        }

        // Verificar si la ruta está en la lista de permitidas
        return _allowedPaths.Any(path => request.Path.StartsWithSegments(path, StringComparison.OrdinalIgnoreCase));
    }

    private void ProcessTokenFromQueryString(HttpContext context)
    {
        try
        {
            var token = context.Request.Query["access_token"].FirstOrDefault();
            
            if (!string.IsNullOrWhiteSpace(token))
            {
                // Validar formato básico del token
                if (IsValidJwtFormat(token))
                {
                    // Agregar token al header Authorization
                    context.Request.Headers["Authorization"] = $"Bearer {token}";
                    
                    _logger.LogDebug("Token JWT agregado desde query string para ruta: {Path}", context.Request.Path);
                }
                else
                {
                    _logger.LogWarning("Token inválido en query string para ruta: {Path}", context.Request.Path);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error procesando token desde query string");
        }
    }

    private bool IsValidJwtFormat(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            return handler.CanReadToken(token);
        }
        catch
        {
            return false;
        }
    }
}
