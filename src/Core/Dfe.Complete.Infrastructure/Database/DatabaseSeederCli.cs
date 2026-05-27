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
            var helpRequested = args.Contains("--help") || args.Contains("-h") || args.Contains("-?");

            if (helpRequested)
            {
                ShowHelp(logger);
                return 0;
            }

            logger.LogInformation("Starting database seeding for development environment.");
            await serviceProvider.SeedDatabaseAsync();
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
    -h, --help      Show this help message

Example:
    dotnet run -- seed-db

Note: Seeding is only available in Development environment";
        
        logger.LogInformation("{HelpText}", helpText);
    }

}