using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGA.Application.Interfaces;
using SGA.Application.Interfaces.Repositories;
using SGA.Infrastructure.Data;
using SGA.Infrastructure.Data.External;
using SGA.Infrastructure.Repositories;
using SGA.Infrastructure.Services;

namespace SGA.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Helper para obtener cadenas de conexión desde variables de entorno
        string GetConnectionString(string connectionName, string envVariable)
        {
            return Environment.GetEnvironmentVariable(envVariable) 
                   ?? configuration.GetConnectionString(connectionName)
                   ?? throw new InvalidOperationException($"Connection string '{connectionName}' not found. Set {envVariable} environment variable.");
        }

        // Base de datos principal
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(GetConnectionString("DefaultConnection", "SGA_DB_CONNECTION")));

        // Registrar IApplicationDbContext
        services.AddScoped<IApplicationDbContext>(provider => 
            provider.GetRequiredService<ApplicationDbContext>());

        // Bases de datos externas
        services.AddDbContext<TTHHDbContext>(options =>
            options.UseSqlServer(GetConnectionString("TTHHConnection", "SGA_TTHH_CONNECTION")));

        services.AddDbContext<DACDbContext>(options =>
            options.UseSqlServer(GetConnectionString("DACConnection", "SGA_DAC_CONNECTION")));

        services.AddDbContext<DITICDbContext>(options =>
            options.UseSqlServer(GetConnectionString("DITICConnection", "SGA_DITIC_CONNECTION")));

        services.AddDbContext<DIRINVDbContext>(options =>
            options.UseSqlServer(GetConnectionString("DIRINVConnection", "SGA_DIRINV_CONNECTION")));

        // Repositorios
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IDocenteRepository, DocenteRepository>();
        services.AddScoped<ISolicitudAscensoRepository, SolicitudAscensoRepository>();
        services.AddScoped<IDocumentoRepository, DocumentoRepository>();
        services.AddScoped<INotificacionRepository, NotificacionRepository>();
        services.AddScoped<ILogAuditoriaRepository, LogAuditoriaRepository>();
        services.AddScoped<ITTHHRepository, TTHHRepository>();
        services.AddScoped<IObraAcademicaRepository, ObraAcademicaRepository>();
        services.AddScoped<ISolicitudEvidenciaInvestigacionRepository, SolicitudEvidenciaInvestigacionRepository>();
        
        // Repositorio genérico para obras académicas
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        
        // Servicio de datos de DIRINV
        services.AddScoped<IDIRINVDataService, DIRINVDataService>();

        return services;
    }
}
