using Dfe.Complete.Configuration;
using Dfe.Complete.Validators;
using GovUK.Dfe.CoreLibs.Security.Antiforgery;
using GovUK.Dfe.CoreLibs.Security.Cypress;
using GovUK.Dfe.CoreLibs.Security.Enums;
using GovUK.Dfe.CoreLibs.Security.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.StartupTests;

public sealed class StartupTests : IDisposable
{
    private readonly string _tempDpTargetPath;

    public StartupTests()
    {
        _tempDpTargetPath = Path.Combine(Path.GetTempPath(), "dp-" + Guid.NewGuid());
        Directory.CreateDirectory(_tempDpTargetPath);
    }

    [Fact]
    public void SetupDataProtection_PersistsKeys_ToConfiguredFolder()
    {
        // Arrange
        IConfiguration cfg = BuildConfiguration(_tempDpTargetPath, keyVaultKey: string.Empty);
        var startup = new Startup(cfg);
        var services = new ServiceCollection();

        // Act
        InvokeSetupDataProtection(startup, services);
        var sp = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(sp.GetService<IDataProtectionProvider>());
        Assert.True(Directory.Exists(_tempDpTargetPath));

    }

    [Fact]
    public void SetupDataProtection_UsesKeyVault_WhenKeyConfigured()
    {
        // Arrange
        var fakeKeyVaultUri = "https://contoso.vault.azure.net/keys/dp-unit-test/0000000000000000";
        IConfiguration cfg = BuildConfiguration(_tempDpTargetPath, fakeKeyVaultUri);
        var startup = new Startup(cfg);
        var services = new ServiceCollection();

        // Act
        InvokeSetupDataProtection(startup, services);

        // Assert
        var provider = services.BuildServiceProvider().GetService<IDataProtectionProvider>();
        Assert.NotNull(provider);
    }

    [Fact]
    public void SetupDataProtection_ValidateConfigureCustomAntiforgery()
    {
        // Arrange
        IConfiguration cfg = BuildConfiguration(_tempDpTargetPath, keyVaultKey: string.Empty);
        var startup = new Startup(cfg);
        var services = new ServiceCollection();

        // Act
        InvokeSetupDataProtection(startup, services);
        startup.ConfigureServices(services);
        var sp = services.BuildServiceProvider();

        // Assert 
        var configuredOptions = sp.GetRequiredService<IOptions<CustomAwareAntiForgeryOptions>>().Value;
        Assert.NotNull(configuredOptions.CheckerGroups);

        Assert.Contains(services, d => d.ServiceType == typeof(ICustomRequestChecker) && d.ImplementationType == typeof(CypressRequestChecker));
        Assert.Contains(services, d => d.ServiceType == typeof(ICustomRequestChecker) && d.ImplementationType == typeof(HasHeaderKeyExistsInRequestValidator));

        Assert.Single(configuredOptions.CheckerGroups);
        Assert.Equal(2, configuredOptions.CheckerGroups.First().TypeNames.Length);
        Assert.Equal(nameof(HasHeaderKeyExistsInRequestValidator), configuredOptions.CheckerGroups[0].TypeNames[0]);
        Assert.Equal(nameof(CypressRequestChecker), configuredOptions.CheckerGroups[0].TypeNames[1]);
        Assert.Equal(CheckerOperator.Or, configuredOptions.CheckerGroups[0].CheckerOperator);
    }

    [Fact]
    public void ConfigureServices_Binds_ExternalLinksOptions_From_Config()
    {
        // Arrange
        const string expectedUrl = "https://powerbi.contoso/awesome-report";
        var dict = new Dictionary<string, string?>
        {
            ["ExternalLinks:PowerBiReports"] = expectedUrl,
            ["DataProtection:DpTargetPath"] = Path.GetTempPath(),
            ["DataProtection:KeyVaultKey"] = string.Empty
        };

        IConfiguration cfg = new ConfigurationBuilder()
            .AddInMemoryCollection(dict)
            .Build();

        var startup = new Startup(cfg);
        var services = new ServiceCollection();

        startup.ConfigureServices(services);

        // Act
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<ExternalLinksOptions>>().Value;

        // Assert
        Assert.Equal(expectedUrl, options.PowerBiReports);
    }

    [Fact]
    public void ConfigureServices_TempDataCookie_HasHttpOnlyEnabled()
    {
        // Arrange
        IConfiguration cfg = BuildConfiguration(_tempDpTargetPath, keyVaultKey: string.Empty);
        var startup = new Startup(cfg);
        var services = new ServiceCollection();

        // Act
        startup.ConfigureServices(services);
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<CookieTempDataProviderOptions>>().Value;

        // Assert
        Assert.True(options.Cookie.HttpOnly, "TempData cookie should have HttpOnly enabled to prevent XSS attacks");
    }

    [Fact]
    public void ConfigureServices_TempDataCookie_IsMarkedAsEssential()
    {
        // Arrange
        IConfiguration cfg = BuildConfiguration(_tempDpTargetPath, keyVaultKey: string.Empty);
        var startup = new Startup(cfg);
        var services = new ServiceCollection();

        // Act
        startup.ConfigureServices(services);
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<CookieTempDataProviderOptions>>().Value;

        // Assert
        Assert.True(options.Cookie.IsEssential, "TempData cookie should be marked as essential");
    }

    [Fact]
    public void ConfigureServices_TempDataCookie_HasCorrectName()
    {
        // Arrange
        IConfiguration cfg = BuildConfiguration(_tempDpTargetPath, keyVaultKey: string.Empty);
        var startup = new Startup(cfg);
        var services = new ServiceCollection();

        // Act
        startup.ConfigureServices(services);
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<CookieTempDataProviderOptions>>().Value;

        // Assert
        Assert.Equal(".Complete.TempData", options.Cookie.Name);
    }

    [Fact]
    public void ConfigureServices_TempDataCookie_HasSameSiteLax()
    {
        // Arrange
        IConfiguration cfg = BuildConfiguration(_tempDpTargetPath, keyVaultKey: string.Empty);
        var startup = new Startup(cfg);
        var services = new ServiceCollection();

        // Act
        startup.ConfigureServices(services);
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<CookieTempDataProviderOptions>>().Value;

        // Assert
        Assert.Equal(SameSiteMode.Lax, options.Cookie.SameSite);
    }

    [Fact]
    public void ConfigureServices_TempDataCookie_HasSecurePolicyAlways_WhenCINotSet()
    {
        // Arrange
        var dict = new Dictionary<string, string?>
        {
            ["DataProtection:DpTargetPath"] = _tempDpTargetPath,
            ["DataProtection:KeyVaultKey"] = string.Empty,
            ["ApplicationInsights:EnableBrowserAnalytics"] = "false"
            // CI key is not set, simulating production environment
        };
        IConfiguration cfg = new ConfigurationBuilder().AddInMemoryCollection(dict).Build();
        var startup = new Startup(cfg);
        var services = new ServiceCollection();

        // Act
        startup.ConfigureServices(services);
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<CookieTempDataProviderOptions>>().Value;

        // Assert
        Assert.Equal(CookieSecurePolicy.Always, options.Cookie.SecurePolicy);
    }

    [Fact]
    public void ConfigureServices_TempDataCookie_AllowsFlexibleSecurePolicy_WhenCISet()
    {
        // Arrange
        var dict = new Dictionary<string, string?>
        {
            ["DataProtection:DpTargetPath"] = _tempDpTargetPath,
            ["DataProtection:KeyVaultKey"] = string.Empty,
            ["ApplicationInsights:EnableBrowserAnalytics"] = "false",
            ["CI"] = "true" // Simulating CI/development environment
        };
        IConfiguration cfg = new ConfigurationBuilder().AddInMemoryCollection(dict).Build();
        var startup = new Startup(cfg);
        var services = new ServiceCollection();

        // Act
        startup.ConfigureServices(services);
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<CookieTempDataProviderOptions>>().Value;

        // Assert
        Assert.NotEqual(CookieSecurePolicy.Always, options.Cookie.SecurePolicy);
    }

    private static IConfiguration BuildConfiguration(string dpTargetPath, string? keyVaultKey)
    {
        var dict = new Dictionary<string, string?>
        {
            ["DataProtection:DpTargetPath"] = dpTargetPath,
            ["DataProtection:KeyVaultKey"] = keyVaultKey,
            ["ApplicationInsights:EnableBrowserAnalytics"] = "false"
        };
        return new ConfigurationBuilder().AddInMemoryCollection(dict).Build();
    }

    private static void InvokeSetupDataProtection(Startup startup, IServiceCollection services)
    {
        MethodInfo? mi = typeof(Startup).GetMethod(
                            "SetupDataProtection",
                            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(mi);
        mi.Invoke(startup, [services]);
    }
    
    public void Dispose()
    {
        try { Directory.Delete(_tempDpTargetPath, recursive: true); }
        catch { }
    }
}
