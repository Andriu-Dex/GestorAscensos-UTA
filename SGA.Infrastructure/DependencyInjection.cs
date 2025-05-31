using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGA.Infrastructure.Data;
using SGA.Infrastructure.Repositories;

namespace SGA.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configurar DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

            // Registrar repositorios
            services.AddScoped<IDocenteRepository, DocenteRepository>();
            services.AddScoped<IDocumentoRepository, DocumentoRepository>();
            services.AddScoped<ISolicitudAscensoRepository, SolicitudAscensoRepository>();
            
            return services;
        }
    }
}
