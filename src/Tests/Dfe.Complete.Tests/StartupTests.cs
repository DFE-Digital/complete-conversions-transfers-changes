using System.Reflection;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dfe.Complete.Tests;

public class StartupTests
{
    [Fact]
    public void SetupDataProtection_RegistersDataProtectionProvider()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"DataProtection:KeyVaultKey", string.Empty}
        };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var env = new FakeWebHostEnvironment
        {
            EnvironmentName = "Development",
            ApplicationName = "TestApp",
            ContentRootPath = Directory.GetCurrentDirectory(),
            WebRootPath = Directory.GetCurrentDirectory(),
            WebRootFileProvider = new Microsoft.Extensions.FileProviders.NullFileProvider(),
            ContentRootFileProvider = new Microsoft.Extensions.FileProviders.NullFileProvider()
        };

        var startup = new Startup(configuration, env);
        var services = new ServiceCollection();

        // Act
        MethodInfo? setupMethod = typeof(Startup).GetMethod("SetupDataProtection", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(setupMethod);
        setupMethod.Invoke(startup, [services]);

        var sp = services.BuildServiceProvider();

        // Assert
        var provider = sp.GetService<IDataProtectionProvider>();
        Assert.NotNull(provider);
    }

    private class FakeWebHostEnvironment : IWebHostEnvironment
    {
        public required string EnvironmentName { get; set; }
        public required string ApplicationName { get; set; }
        public required string WebRootPath { get; set; }
        public required Microsoft.Extensions.FileProviders.IFileProvider WebRootFileProvider { get; set; }
        public required string ContentRootPath { get; set; }
        public required Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; }
    }
}