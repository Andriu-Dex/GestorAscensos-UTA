using Microsoft.Extensions.DependencyInjection;
using SGA.Application.Interfaces;
using SGA.Application.Services;

namespace SGA.Application;

public static class DependencyInjection
{    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registrar servicios de aplicación
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDocenteService, DocenteService>();
        services.AddScoped<ISolicitudService, SolicitudService>();
        services.AddScoped<IDocumentoService, DocumentoService>();
        services.AddScoped<IReporteService, ReporteService>();
        services.AddScoped<IAuditoriaService, AuditoriaService>();
        services.AddScoped<IExternalDataService, ExternalDataService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ITTHHService, TTHHService>();
        services.AddScoped<ITTHHService, TTHHService>();
        services.AddScoped<ITTHHService, TTHHService>();
        
        // Configurar AutoMapper
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        
        return services;
    }
}
