using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Infrastructure.Database;

public static class DatabaseSeederExtensions
{
    /// <summary>
    /// Seeds the database with all data (development environment only)
    /// </summary>
    /// <param name="host">The application host</param>
    /// <param name="force">If true, will clear existing data before seeding</param>
    public static async Task SeedDatabaseAsync(this IHost host, bool force = false)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var environment = services.GetRequiredService<IHostEnvironment>();
        
        // Only allow seeding in development environment
        if (!environment.IsDevelopment())
        {
            var logger = services.GetRequiredService<ILogger<DatabaseSeeder>>();
            logger.LogWarning("Database seeding is only allowed in Development environment. Current: {Environment}", environment.EnvironmentName);
            return;
        }
        
        try
        {
            var context = services.GetRequiredService<CompleteContext>();
            var logger = services.GetRequiredService<ILogger<DatabaseSeeder>>();
            var seeder = new DatabaseSeeder(context, logger);

            // Ensure database exists
            await context.Database.EnsureCreatedAsync();

            // Check if already seeded (unless forcing)
            if (!force && await seeder.IsSeededAsync())
            {
                logger.LogInformation("Database is already seeded. Use force=true to re-seed.");
                return;
            }

            // Seed all data for development
            await seeder.SeedAsync(force);
        }
        catch (Exception ex)
        {
            var catchLogger = services.GetRequiredService<ILogger<DatabaseSeeder>>();
            catchLogger.LogError(ex, "Database seeding failed for host in {Environment} environment", services.GetRequiredService<IHostEnvironment>().EnvironmentName);
            throw new InvalidOperationException("Host database seeding operation failed. See inner exception for details.", ex);
        }
    }

    /// <summary>
    /// Seeds the database using a scoped context (development only)
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    /// <param name="force">If true, will clear existing data before seeding</param>
    public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider, bool force = false)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        
        var environment = services.GetRequiredService<IHostEnvironment>();
        
        // Only allow seeding in development environment
        if (!environment.IsDevelopment())
        {
            var catchLogger = services.GetRequiredService<ILogger<DatabaseSeeder>>();
            catchLogger.LogWarning("Database seeding is only allowed in Development environment. Current: {Environment}", environment.EnvironmentName);
            return;
        }
        
        var context = services.GetRequiredService<CompleteContext>();
        var logger = services.GetRequiredService<ILogger<DatabaseSeeder>>();
        var seeder = new DatabaseSeeder(context, logger);

        await context.Database.EnsureCreatedAsync();
        if (!force && await seeder.IsSeededAsync())
        {
            logger.LogInformation("Database is already seeded. Use force=true to re-seed.");
            return;
        }
        await seeder.SeedAsync(force);
    }

    /// <summary>
    /// Adds database seeder services to the dependency injection container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDatabaseSeeder(this IServiceCollection services)
    {
        services.AddTransient<DatabaseSeeder>();
        return services;
    }
}