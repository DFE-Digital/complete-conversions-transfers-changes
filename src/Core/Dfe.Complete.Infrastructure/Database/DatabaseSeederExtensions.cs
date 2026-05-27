using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Infrastructure.Database;

public static class DatabaseSeederExtensions
{
    /// <summary>
    /// Seeds the database using a scoped context (development only)
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
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
        // Seed all data for development (idempotent)
        await seeder.SeedAsync();
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