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
        services.AddScoped<IReporteAdminService, ReporteAdminService>();
        services.AddScoped<IAuditoriaService, AuditoriaService>();
        services.AddScoped<IExternalDataService, ExternalDataService>();
        services.AddScoped<IValidacionAscensoService, ValidacionAscensoService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ITTHHService, TTHHService>();
        services.AddScoped<IImageService, ImageService>();
        
        // Servicios para obras académicas
        services.AddScoped<IObrasAcademicasService, ObrasAcademicasService>();
        services.AddScoped<INotificationService, NotificationService>();
        
        // Servicio de email
        services.AddScoped<IEmailService, EmailService>();
        
        // Servicios de notificaciones en tiempo real
        services.AddScoped<INotificacionTiempoRealService, NotificacionTiempoRealService>();
        
        // Servicios para certificados de capacitación
        services.AddScoped<ICertificadosCapacitacionService, CertificadosCapacitacionService>();
        
        // Servicios para evidencias de investigación
        services.AddScoped<IEvidenciasInvestigacionService, EvidenciasInvestigacionService>();
        
        // Servicio de compresión de PDFs
        services.AddScoped<IPDFCompressionService, PDFCompressionService>();
        
        // Servicio de conversión de documentos para solicitudes de ascenso
        services.AddScoped<DocumentoConversionService>();
        
        // Servicio de configuración dinámica de requisitos y títulos académicos
        services.AddScoped<IRequisitosDinamicosService, RequisitosDinamicosService>();
        services.AddScoped<IConfiguracionRequisitoService, ConfiguracionRequisitoService>();
        services.AddScoped<ITituloAcademicoService, TituloAcademicoService>();
        
        // Configurar AutoMapper
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        
        // Configurar MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        
        // Configurar FluentValidation
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        
        return services;
    }
}
