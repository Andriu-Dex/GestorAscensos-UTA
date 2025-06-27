using Microsoft.Extensions.DependencyInjection;
using SGA.Application.Interfaces;
using SGA.Application.Services;
using FluentValidation;

namespace SGA.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registrar servicios de aplicación
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDocenteService, DocenteService>();
        services.AddScoped<ISolicitudService, SolicitudService>();
        services.AddScoped<IDocumentoService, DocumentoService>();
        services.AddScoped<IReporteService, ReporteService>();
        services.AddScoped<IAuditoriaService, AuditoriaService>();
        services.AddScoped<IExternalDataService, ExternalDataService>();
        services.AddScoped<IValidacionAscensoService, ValidacionAscensoService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ITTHHService, TTHHService>();
        
        // Servicios para obras académicas
        services.AddScoped<IObrasAcademicasService, ObrasAcademicasService>();
        services.AddScoped<INotificationService, NotificationService>();
        
        // Configurar AutoMapper
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        
        // Configurar MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        
        // Configurar FluentValidation
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        
        return services;
    }
}
