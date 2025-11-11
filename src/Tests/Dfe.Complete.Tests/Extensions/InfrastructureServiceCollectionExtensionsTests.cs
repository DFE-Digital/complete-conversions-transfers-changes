using Dfe.Complete.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Dfe.Complete.Tests.Extensions;

public class InfrastructureServiceCollectionExtensionsTests
{
    [Fact]
    public void AddInfrastructureDependencyGroup_WithRedisEnabled_RegistersRedisCache()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"ConnectionStrings:DefaultConnection", "Server=(localdb)\\mssqllocaldb;Database=TestDb;Trusted_Connection=True;"},
            {"Redis:Enable", "true"},
            {"Redis:Host", "localhost"},
            {"Redis:Port", "6379"},
            {"Redis:Password", "testpassword"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var services = new ServiceCollection();

        // Act
        InfrastructureServiceCollectionExtensions.AddInfrastructureDependencyGroup(services, configuration);
        var provider = services.BuildServiceProvider();

        // Assert
        // The redis cache should be registered.
        var distributedCache = provider.GetService<IDistributedCache>();
        Assert.NotNull(distributedCache);
        Assert.Contains("RedisCache", distributedCache.GetType().ToString());
    }

    [Fact]
    public void AddInfrastructureDependencyGroup_WithRedisDisabled_DoesNotRegisterRedisCache()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"ConnectionStrings:DefaultConnection", "Server=(localdb)\\mssqllocaldb;Database=TestDb;Trusted_Connection=True;"},
            {"Redis:Enable", "false"}  // Redis disabled.
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var services = new ServiceCollection();

        // Act
        InfrastructureServiceCollectionExtensions.AddInfrastructureDependencyGroup(services, configuration);
        var provider = services.BuildServiceProvider();

        // Assert
        // When Redis is disabled, IDistributedCache registration might be absent or register another caching provider.
        // Adjust assertion based on your design. Here we simply verify that redis cache is not present.
        var distributedCache = provider.GetService<IDistributedCache>();
        Assert.NotNull(distributedCache);
        Assert.IsType<MemoryDistributedCache>(distributedCache);
    }
}