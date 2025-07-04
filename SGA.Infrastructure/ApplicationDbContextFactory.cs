using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SGA.Infrastructure.Data;

namespace SGA.Infrastructure;

/// <summary>
/// Factory para crear ApplicationDbContext en tiempo de diseño (para migraciones)
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // Usar la cadena de conexión del proyecto
        optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=SGA_Main;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true");
        
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
