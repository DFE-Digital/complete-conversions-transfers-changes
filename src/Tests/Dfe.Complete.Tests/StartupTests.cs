// ------------------------------------------------------------
//  InfrastructureRedisTests.cs
//  Unit‑tests that exercise the Redis branch inside
//  Dfe.Complete.Infrastructure.InfrastructureServiceCollectionExtensions
// ------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Dfe.Complete.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Xunit;

namespace Dfe.Complete.Tests;

/// <summary>
///   Focused tests around the "Redis" section inside
///   <see cref="InfrastructureServiceCollectionExtensions.AddInfrastructureDependencyGroup"/>.
///   We verify both the disabled and enabled paths without touching a real Redis
///   instance.  The enabled‑path uses <c>port = 1</c> so the TCP connect fails
///   immediately (connection‑refused) and StackExchange.Redis returns almost
///   instantaneously thanks to <c>AbortOnConnectFail = false</c> in production
///   code.  This keeps the test runtime &lt; 1 second on typical CI agents.
/// </summary>
public sealed class InfrastructureRedisTests
{
    /* --------------------------------------------------------------------
     *  🧪  Tests
     * ------------------------------------------------------------------*/

    [Fact]
    public void AddInfrastructureDependencyGroup_RedisDisabled_DoesNotRegisterRedisCache()
    {
        // Arrange
        IConfiguration cfg = BuildConfiguration(enable: false);
        var services = new ServiceCollection();

        // Act
        services.AddInfrastructureDependencyGroup(cfg);

        // Assert – no Redis‑backed IDistributedCache was added.
        Assert.DoesNotContain(
            services,
            d => d.ServiceType == typeof(IDistributedCache) &&
                 d.ImplementationFactory is not null);
    }

    [Fact]
    public void AddInfrastructureDependencyGroup_RedisEnabled_RegistersRedisCache_WithExpectedOptions()
    {
        // Arrange – port 1 gives an immediate connection‑refused (fast!).
        IConfiguration cfg = BuildConfiguration(enable: true, host: "localhost", port: 1, password: string.Empty);
        var services = new ServiceCollection();

        // Act
        services.AddInfrastructureDependencyGroup(cfg);

        // Assert – exactly one IDistributedCache registered via an ImplementationFactory.
        var redisDescriptor = Assert.Single(
            services.Where(d => d.ServiceType == typeof(IDistributedCache)));
        Assert.NotNull(redisDescriptor.ImplementationFactory);

        // Retrieve the ConfigurationOptions captured in the closure so we can
        // assert against the values set inside AddInfrastructureDependencyGroup
        var opts = ExtractConfigurationOptions(redisDescriptor);
        Assert.NotNull(opts);

        Assert.True(opts!.Ssl);
        Assert.Equal("Dfe.Complete", opts.ClientName);
        Assert.Equal("localhost:1", opts.EndPoints.Single().ToString(), ignoreCase: true);
        Assert.Equal(6, opts.DefaultVersion.Major);
        Assert.Equal(0, opts.DefaultVersion.Minor);
    }

    /* --------------------------------------------------------------------
     *  🛠️  Helpers
     * ------------------------------------------------------------------*/

    private static IConfiguration BuildConfiguration(
        bool   enable,
        string host     = "localhost",
        int    port     = 1,
        string password = "")
    {
        var dict = new Dictionary<string, string?>
        {
            ["Redis:Enable"]   = enable.ToString(),
            ["Redis:Host"]     = host,
            ["Redis:Port"]     = port.ToString(),
            ["Redis:Password"] = password,
            // Minimum viable connection‑string so AddDbContext doesn’t complain.
            ["ConnectionStrings:DefaultConnection"] = "Server=(local);Database=UnitTest;Trusted_Connection=True;"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(dict)
            .Build();
    }

    /// <summary>
    ///   Peeks inside the closure that AddStackExchangeRedisCache generated so
    ///   we can grab the <see cref="ConfigurationOptions"/> instance built in
    ///   production code without triggering any network activity.
    /// </summary>
    private static ConfigurationOptions? ExtractConfigurationOptions(ServiceDescriptor desc)
    {
        if (desc.ImplementationFactory is null)
            return null;

        var closure = desc.ImplementationFactory.Target;
        if (closure is null)
            return null;

        return closure.GetType()
                       .GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                       .FirstOrDefault(f => f.FieldType == typeof(ConfigurationOptions))?
                       .GetValue(closure) as ConfigurationOptions;
    }
}
