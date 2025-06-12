using Microsoft.Extensions.DependencyInjection;
using SGA.Application.Services;
using SGA.Infrastructure.Repositories;

namespace SGA.Application
{
    public static class DependencyInjectionTemp
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {            
            // Registrar servicios
            services.AddScoped<IDocenteService, DocenteService>();
            services.AddScoped<IDocumentoService, DocumentoService>();
            services.AddScoped<ISolicitudAscensoService, SolicitudAscensoService>();
            services.AddScoped<IDatosTTHHService, DatosTTHHService>();
            services.AddScoped<IValidacionAscensoService, ValidacionAscensoService>();
            services.AddScoped<IReporteService, ReporteService>();
            services.AddScoped<ICryptoService, CryptoService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IServicioExternoService, ServicioExternoService>();
            
            return services;
        }
    }
}
