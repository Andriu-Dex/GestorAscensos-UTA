using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGA.Application.Interfaces.Repositories;
using SGA.Infrastructure.Data;
using SGA.Infrastructure.Data.External;
using SGA.Infrastructure.Repositories;

namespace SGA.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Base de datos principal
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Bases de datos externas
        services.AddDbContext<TTHHDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("TTHHConnection")));

        services.AddDbContext<DACDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DACConnection")));

        services.AddDbContext<DITICDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DITICConnection")));

        services.AddDbContext<DIRINVDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DIRINVConnection")));

        // Repositorios
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IDocenteRepository, DocenteRepository>();
        services.AddScoped<ISolicitudAscensoRepository, SolicitudAscensoRepository>();
        services.AddScoped<IDocumentoRepository, DocumentoRepository>();
        services.AddScoped<ILogAuditoriaRepository, LogAuditoriaRepository>();
        services.AddScoped<ITTHHRepository, TTHHRepository>();

        return services;
    }
}
