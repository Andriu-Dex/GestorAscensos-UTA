using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SGA.Infrastructure.Data;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SGA.MigrationTool
{
    /// <summary>
    /// Console application for applying database migrations manually
    /// </summary>
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Database Migration Tool for SGA");
            Console.WriteLine("================================");
            
            try
            {
                // Create service provider with DbContext
                var serviceProvider = CreateServiceProvider();
                
                // Configure logging
                var loggerFactory = LoggerFactory.Create(builder => 
                {
                    builder
                        .AddConsole()
                        .SetMinimumLevel(LogLevel.Information);
                });
                
                var logger = loggerFactory.CreateLogger<Program>();
                
                // Check if migrations are needed
                bool needsMigration = await DatabaseMigrationManager.NeedsMigrationAsync(serviceProvider);
                
                if (needsMigration)
                {
                    Console.WriteLine("Database needs migration. Proceed? (y/n)");
                    var response = Console.ReadLine()?.ToLower();
                    
                    if (response == "y")
                    {
                        Console.WriteLine("Applying migrations...");
                        await DatabaseMigrationManager.MigrateAsync(serviceProvider, logger);
                        Console.WriteLine("Migrations applied successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Migration canceled.");
                    }
                }
                else
                {
                    Console.WriteLine("Database is up to date. No migrations needed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        
        /// <summary>
        /// Creates a service provider with DbContext configured
        /// </summary>
        private static IServiceProvider CreateServiceProvider()
        {
            // Build configuration
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            // Create service collection and configure services
            var services = new ServiceCollection();
            
            // Add logging
            services.AddLogging(builder => 
            {
                builder.AddConsole();
            });
            
            // Configure DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
                
            return services.BuildServiceProvider();
        }
    }
}
