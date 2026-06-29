using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
        // Check environment first
        var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
        if (!environment.IsDevelopment())
        {
            Console.WriteLine($"Database seeding is only allowed in local Development environment. Current environment: {environment.EnvironmentName}");
            return 1;
        }

        try
        {
            var helpRequested = args.Contains("--help") || args.Contains("-h") || args.Contains("-?");
            var forceRequested = args.Contains("--force") || args.Contains("-f");

            if (helpRequested)
            {
                ShowHelp();
                return 0;
            }

            Console.WriteLine($"Starting database seeding for development environment. Force: {forceRequested}");

            await serviceProvider.SeedDatabaseAsync(forceRequested);

            Console.WriteLine("Database seeding completed successfully!");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database seeding failed in {environment.EnvironmentName} environment: {ex.Message}");
            return 1;
        }
    }

    private static void ShowHelp()
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
        Console.WriteLine(helpText);
    }

}