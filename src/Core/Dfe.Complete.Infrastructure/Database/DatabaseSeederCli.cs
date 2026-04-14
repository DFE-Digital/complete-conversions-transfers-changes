using Dfe.Complete.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Infrastructure.Database;

/// <summary>
/// Command-line interface for database seeding operations (development only)
/// Can be called from Program.cs with command line arguments or used as a separate tool
/// </summary>
public static class DatabaseSeederCli
{
    /// <summary>
    /// Configures and executes database seeding commands from command line arguments (development only)
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <param name="serviceProvider">Service provider for dependency injection</param>
    public static async Task<int> ExecuteAsync(string[] args, IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<DatabaseSeeder>>();
        
        // Check environment first
        var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
        if (!environment.IsDevelopment())
        {
            logger.LogError("Database seeding is only allowed in Development environment. Current environment: {Environment}", environment.EnvironmentName);
            return 1;
        }
        
        try
        {
            var force = args.Contains("--force") || args.Contains("-f");
            var helpRequested = args.Contains("--help") || args.Contains("-h") || args.Contains("-?");

            if (helpRequested)
            {
                ShowHelp(logger);
                return 0;
            }

            logger.LogInformation("Starting database seeding for development environment. Force: {Force}", force);
            await serviceProvider.SeedDatabaseAsync(force);
            logger.LogInformation("Database seeding completed successfully!");
            return 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database seeding failed in {Environment} environment", environment.EnvironmentName);
            return 1;
        }
    }

    private static void ShowHelp(ILogger logger)
    {
        var helpText = @"Database Seeding Utility for Dfe.Complete (Development Only)

Usage: dotnet run [options]

Options:
  -f, --force     Force seeding (clears existing data)
  -h, --help      Show this help message

Examples:
  dotnet run -- seed-db
  dotnet run -- seed-db --force

Note: Seeding is only available in Development environment";
        
        logger.LogInformation("{HelpText}", helpText);
    }

    /// <summary>
    /// Simple seeding method for development environment only
    /// </summary>
    /// <param name="serviceProvider">Service provider for dependency injection</param>
    public static async Task SeedForDevelopmentAsync(IServiceProvider serviceProvider)
    {
        var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
        var logger = serviceProvider.GetRequiredService<ILogger<DatabaseSeeder>>();
        
        if (!environment.IsDevelopment())
        {
            logger.LogWarning("Development seeding skipped. Current environment: {Environment}", environment.EnvironmentName);
            return;
        }
        
        try
        {
            logger.LogInformation("Seeding database for development environment...");
            
            await serviceProvider.SeedDatabaseAsync(force: false);
            
            logger.LogInformation("Development database seeding completed!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Development database seeding failed for environment: {Environment}", environment.EnvironmentName);
            throw new InvalidOperationException("Development database seeding operation failed. See inner exception for details.", ex);
        }
    }
}