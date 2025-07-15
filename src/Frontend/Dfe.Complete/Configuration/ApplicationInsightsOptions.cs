namespace Dfe.Complete.Configuration;

public class ApplicationInsightsOptions
{
    public const string ConfigurationSection = "ApplicationInsights";
    public string? ConnectionString { get; init; } = string.Empty;
    public string EnableBrowserAnalytics { get; init; } = string.Empty;
}
