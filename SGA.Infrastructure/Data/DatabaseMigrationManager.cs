using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SGA.Infrastructure.Data;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace SGA.Infrastructure.Data
{
    /// <summary>
    /// Utility class for database migration operations
    /// </summary>
    public static class DatabaseMigrationManager
    {
        /// <summary>
        /// Applies any pending migrations to the database
        /// </summary>
        /// <param name="serviceProvider">The service provider to resolve dependencies</param>
        /// <param name="logger">Optional logger for operation logging</param>
        /// <returns>A task that represents the asynchronous migration operation</returns>
        public static async Task MigrateAsync(IServiceProvider serviceProvider, ILogger? logger = null)
        {
            try
            {
                // Use a transaction scope to ensure atomic operations
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // Get the DbContext from the service provider
                    using var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
                    
                    logger?.LogInformation("Starting database migration...");
                    
                    // Apply any pending migrations
                    await dbContext.Database.MigrateAsync();
                    
                    logger?.LogInformation("Database migration completed successfully");
                    
                    // Complete the transaction
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "An error occurred during database migration");
                throw;
            }
        }

        /// <summary>
        /// Checks if the database needs migration
        /// </summary>
        /// <param name="serviceProvider">The service provider to resolve dependencies</param>
        /// <returns>True if pending migrations exist, otherwise false</returns>
        public static async Task<bool> NeedsMigrationAsync(IServiceProvider serviceProvider)
        {
            using var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
            return await dbContext.Database.GetPendingMigrationsAsync().ContinueWith(t => t.Result.Any());
        }

        /// <summary>
        /// Ensures the database exists and is created
        /// </summary>
        /// <param name="serviceProvider">The service provider to resolve dependencies</param>
        /// <param name="logger">Optional logger for operation logging</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static async Task EnsureDatabaseCreatedAsync(IServiceProvider serviceProvider, ILogger? logger = null)
        {
            try
            {
                using var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
                
                logger?.LogInformation("Ensuring database exists...");
                
                // Check if database exists, if not create it
                bool created = await dbContext.Database.EnsureCreatedAsync();
                
                if (created)
                {
                    logger?.LogInformation("Database was created successfully");
                }
                else
                {
                    logger?.LogInformation("Database already exists");
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "An error occurred while ensuring database exists");
                throw;
            }
        }
    }
}
