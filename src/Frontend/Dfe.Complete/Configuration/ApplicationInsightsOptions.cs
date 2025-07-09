namespace Dfe.Complete.Configuration;

public class ApplicationInsightsOptions
{
    public const string ConfigurationSection = "ApplicationInsights";
    public string ConnectionString { get; init; }
    public string EnableBrowserAnalytics { get; init; }
}
