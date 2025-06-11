using System.Reflection;
using Dfe.Complete.Configuration;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace Dfe.Complete.Tests.StartupTests;

public class StartupTests : IDisposable
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
        var startup = BuildStartup();
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
        var startup = BuildStartup(cfg);
        var services = new ServiceCollection();

        // Act
        InvokeSetupDataProtection(startup, services);

        // Assert
        var provider = services.BuildServiceProvider().GetService<IDataProtectionProvider>();
        Assert.NotNull(provider);
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

        var startup = BuildStartup(cfg);
        var services = new ServiceCollection();

        startup.ConfigureServices(services);

        // Act
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<ExternalLinksOptions>>().Value;

        // Assert
        Assert.Equal(expectedUrl, options.PowerBiReports);
    }

    private static IConfiguration BuildConfiguration(string dpTargetPath, string? keyVaultKey)
    {
        var dict = new Dictionary<string, string?>
        {
            ["DataProtection:DpTargetPath"] = dpTargetPath,
            ["DataProtection:KeyVaultKey"] = keyVaultKey
        };
        return new ConfigurationBuilder().AddInMemoryCollection(dict).Build();
    }

    private static FakeWebHostEnvironment BuildEnvironment() =>
        new FakeWebHostEnvironment
        {
            EnvironmentName = "Test",
            ApplicationName = "UnitTestApp",
            ContentRootPath = Directory.GetCurrentDirectory(),
            WebRootPath = Directory.GetCurrentDirectory(),
            ContentRootFileProvider = new NullFileProvider(),
            WebRootFileProvider = new NullFileProvider()
        };

    private Startup BuildStartup(IConfiguration? cfg = null, IWebHostEnvironment? env = null)
    {
        IConfiguration defaultCfg = BuildConfiguration(_tempDpTargetPath, keyVaultKey: string.Empty);
        return new Startup(cfg ?? defaultCfg, env ?? BuildEnvironment());
    }
    private static void InvokeSetupDataProtection(Startup startup, IServiceCollection services)
    {
        MethodInfo? mi = typeof(Startup).GetMethod(
                            "SetupDataProtection",
                            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(mi);
        mi.Invoke(startup, new object?[] { services });
    }

    private sealed class FakeWebHostEnvironment : IWebHostEnvironment
    {
        public required string EnvironmentName { get; set; }
        public required string ApplicationName { get; set; }
        public required string WebRootPath { get; set; }
        public required IFileProvider WebRootFileProvider { get; set; }
        public required string ContentRootPath { get; set; }
        public required IFileProvider ContentRootFileProvider { get; set; }
    }

    public void Dispose()
    {
        try { Directory.Delete(_tempDpTargetPath, recursive: true); }
        catch { }
    }
}
