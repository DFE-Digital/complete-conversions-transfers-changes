using System;
using System.Reflection;
using Dfe.Complete.Validators;
using DfE.CoreLibs.Security.Antiforgery;
using DfE.CoreLibs.Security.Cypress;
using DfE.CoreLibs.Security.Enums;
using DfE.CoreLibs.Security.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Moq;

namespace Dfe.Complete.Tests;

public sealed class StartupDataProtectionTests : IDisposable
{
    private readonly string _tempDpTargetPath;

    public StartupDataProtectionTests()
    {
        _tempDpTargetPath = Path.Combine(Path.GetTempPath(), "dp-" + Guid.NewGuid());
        Directory.CreateDirectory(_tempDpTargetPath);
    }

    [Fact]
    public void SetupDataProtection_PersistsKeys_ToConfiguredFolder()
    {
        // Arrange
        IConfiguration cfg = BuildConfiguration(_tempDpTargetPath, keyVaultKey: string.Empty);
        var env = BuildEnvironment();
        var startup = new Startup(cfg, env);
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
        var env = BuildEnvironment();
        var startup = new Startup(cfg, env);
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
        var env = BuildEnvironment();
        var startup = new Startup(cfg, env);
        var services = new ServiceCollection();
        // Register the checkers
        //services.AddCustomRequestCheckerProvider<CypressRequestChecker>();
        //services.AddCustomRequestCheckerProvider<HasHeaderKeyExistsInRequestValidator>(); 
        //var httpContextAccessor = new Mock<IHttpContextAccessor>();
        //httpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        //services.AddSingleton(httpContextAccessor.Object);
        //services.AddScoped(sp => sp.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session ?? throw new InvalidOperationException("Session is not available."));

        //// Register the antiforgery options with the expected CheckerGroups
        //services.AddControllersWithViews().AddCustomAntiForgeryHandling(opts =>
        //{
        //    opts.CheckerGroups =
        //    [
        //        new()
        //    {
        //        TypeNames = [nameof(HasHeaderKeyExistsInRequestValidator), nameof(CypressRequestChecker)],
        //        CheckerOperator = CheckerOperator.Or
        //    }
        //    ];
        //}); 

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
        new()
        {
            EnvironmentName = "Test",
            ApplicationName = "UnitTestApp",
            ContentRootPath = Directory.GetCurrentDirectory(),
            WebRootPath = Directory.GetCurrentDirectory(),
            ContentRootFileProvider = new NullFileProvider(),
            WebRootFileProvider = new NullFileProvider()
        };

    private static void InvokeSetupDataProtection(Startup startup, IServiceCollection services)
    {
        MethodInfo? mi = typeof(Startup).GetMethod(
                            "SetupDataProtection",
                            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(mi);
        mi.Invoke(startup, [services]);
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
