using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Dfe.Complete.Infrastructure.Database
{
    [ExcludeFromCodeCoverage]
    public class GenericDbContextFactory<TContext> : IDesignTimeDbContextFactory<TContext> where TContext : DbContext
    {
        public TContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../../Api/Dfe.Complete.Api");

            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var services = new ServiceCollection();

            // Allow sensitive data logging in Development environment only, based on configuration
            bool enableSensitiveDataLogging = false;
            if (environmentName == "Development")
                enableSensitiveDataLogging = configuration.GetValue("EnableSensitiveDataLogging", false);

            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            optionsBuilder.UseCompleteSqlServer(connectionString!, configuration.GetValue("EnableSQLRetryOnFailure", false), enableSensitiveDataLogging);

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(ApplicationServiceCollectionExtensions).Assembly);
            });
            var serviceProvider = services.BuildServiceProvider();

            return (TContext)Activator.CreateInstance(
            typeof(TContext),
                optionsBuilder.Options,
                configuration, serviceProvider)!;
        }
    }
}
