using Microsoft.ApplicationInsights.Extensibility;
using Serilog; 

namespace Dfe.Complete;

public static class Program
{
    public static void Main(string[] args)
    {
        Log.Information("Starting web host");
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
           .UseSerilog((context, services, loggerConfiguration) =>
           {
               loggerConfiguration
                .MinimumLevel.Information()
                .ReadFrom.Configuration(context.Configuration)
                .WriteTo.ApplicationInsights(
                    services.GetRequiredService<TelemetryConfiguration>(),
                    TelemetryConverter.Traces,
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information) // <-- Add this line
                .Enrich.FromLogContext();
           })
           .ConfigureAppConfiguration((_, configuration) => configuration.AddEnvironmentVariables())
           .ConfigureWebHostDefaults(webBuilder =>
           {
               webBuilder.UseStartup<Startup>();
               webBuilder.UseKestrel(options =>
                {
                  options.AddServerHeader = false;
              });
           });
    }
}
