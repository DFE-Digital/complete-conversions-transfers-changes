namespace Dfe.Complete.Configuration;

public class ApplicationInsightsOptions
{
    public const string ConfigurationSection = "ApplicationInsights";
    public string? ConnectionString { get; set; }
    public string? EnableBrowserAnalytics { get; set; }
}
