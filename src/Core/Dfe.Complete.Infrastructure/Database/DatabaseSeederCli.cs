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
            var msg = $"Database seeding is only allowed in local Development environment. Current environment: {environment.EnvironmentName}";
            logger.LogError(msg);
            Console.WriteLine(msg);
            return 1;
        }

        try
        {
            var helpRequested = args.Contains("--help") || args.Contains("-h") || args.Contains("-?");
            var forceRequested = args.Contains("--force") || args.Contains("-f");

            if (helpRequested)
            {
                ShowHelp(logger);
                return 0;
            }

            var startMsg = $"Starting database seeding for development environment. Force: {forceRequested}";
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(startMsg);
            }
            Console.WriteLine(startMsg);

            await serviceProvider.SeedDatabaseAsync(forceRequested);

            var doneMsg = "Database seeding completed successfully!";
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(doneMsg);
            }
            Console.WriteLine(doneMsg);
            return 0;
        }
        catch (Exception ex)
        {
            var errMsg = $"Database seeding failed in {environment.EnvironmentName} environment: {ex.Message}";
            logger.LogError(ex, errMsg);
            Console.WriteLine(errMsg);
            return 1;
        }
    }

    private static void ShowHelp(ILogger logger)
    {
        var helpText = @"Database Seeding Utility for Dfe.Complete (Development Only)

Usage: dotnet run -- seed-db [options]

Options:
    -h, --help      Show this help message
    -f, --force     Reset all seed data before seeding (idempotent, safe for dev)

Examples:
    dotnet run -- seed-db
    dotnet run -- seed-db --force

Note: Seeding is only available in Development environment";
        logger.LogInformation("{HelpText}", helpText);
        Console.WriteLine(helpText);
    }

}