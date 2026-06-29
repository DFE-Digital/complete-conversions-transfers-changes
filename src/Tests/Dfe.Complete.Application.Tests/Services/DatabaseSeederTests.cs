using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Dfe.Complete.Application.Tests.Services;

public sealed class DatabaseSeederTests : IDisposable
{
    private readonly List<string> _databaseFiles = [];

    [Fact]
    public async Task SeedAsync_NonLocalConnection_DoesNotSeedAnyData()
    {
        var connectionString = CreateConnectionString(isLocal: false);
        var configuration = BuildConfiguration();

        await using var context = await CreateContextAsync(connectionString);
        var sut = new DatabaseSeeder(context, configuration);

        await sut.SeedAsync();

        Assert.Equal(0, await context.Users.CountAsync());
        Assert.Equal(0, await context.LocalAuthorities.CountAsync());
        Assert.Equal(0, await context.Projects.CountAsync());
    }

    [Fact]
    public async Task SeedAsync_LocalConnection_SeedsExpectedData()
    {
        var connectionString = CreateConnectionString(isLocal: true);
        var configuration = BuildConfiguration();

        await using var context = await CreateContextAsync(connectionString);
        var sut = new DatabaseSeeder(context, configuration);

        await sut.SeedAsync();

        Assert.Equal(30, await context.Users.CountAsync());
        Assert.Equal(21, await context.LocalAuthorities.CountAsync());
        Assert.Equal(9, await context.SignificantDateHistoryReasons.CountAsync());
        Assert.Equal(57, await context.GiasEstablishments.CountAsync());
        Assert.Equal(57, await context.Projects.CountAsync());
        Assert.Equal(15, await context.ConversionTasksData.CountAsync());
        Assert.Equal(42, await context.TransferTasksData.CountAsync());
        Assert.Equal(1, await context.ProjectGroups.CountAsync());
        Assert.Equal(57, await context.KeyContacts.CountAsync());
    }

    [Fact]
    public async Task SeedAsync_RunTwiceWithoutForce_DoesNotDuplicateData()
    {
        var connectionString = CreateConnectionString(isLocal: true);
        var configuration = BuildConfiguration();

        await using (var firstContext = await CreateContextAsync(connectionString))
        {
            var firstRun = new DatabaseSeeder(firstContext, configuration);
            await firstRun.SeedAsync();
        }

        await using (var secondContext = await CreateContextAsync(connectionString))
        {
            var secondRun = new DatabaseSeeder(secondContext, configuration);
            await secondRun.SeedAsync();

            Assert.Equal(30, await secondContext.Users.CountAsync());
            Assert.Equal(21, await secondContext.LocalAuthorities.CountAsync());
            Assert.Equal(9, await secondContext.SignificantDateHistoryReasons.CountAsync());
            Assert.Equal(57, await secondContext.Projects.CountAsync());
        }
    }

    [Fact]
    public async Task SeedAsync_ForceTrue_ResetsAndReseedsData()
    {
        var connectionString = CreateConnectionString(isLocal: true);
        var configuration = BuildConfiguration();
        Guid firstProjectGroupId;

        await using (var firstContext = await CreateContextAsync(connectionString))
        {
            var firstRun = new DatabaseSeeder(firstContext, configuration);
            await firstRun.SeedAsync();
            firstProjectGroupId = (await firstContext.ProjectGroups.SingleAsync()).Id.Value;
        }

        await using (var secondContext = await CreateContextAsync(connectionString))
        {
            var secondRun = new DatabaseSeeder(secondContext, configuration);
            await secondRun.SeedAsync(force: true);

            var secondProjectGroupId = (await secondContext.ProjectGroups.SingleAsync()).Id.Value;

            Assert.NotEqual(firstProjectGroupId, secondProjectGroupId);
            Assert.Equal(57, await secondContext.Projects.CountAsync());
            Assert.Equal(15, await secondContext.ConversionTasksData.CountAsync());
            Assert.Equal(42, await secondContext.TransferTasksData.CountAsync());
        }
    }

    [Fact]
    public async Task SeedAsync_WithLocalSeedUserEmails_SeedsOnlyValidNewUsers()
    {
        var connectionString = CreateConnectionString(isLocal: true);
        var configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["LocalSeed:UserEmails"] = "new.person@education.gov.uk;joyce.byers@education.gov.uk;invalid@other.com;badformat@education.gov.uk;new.person@education.gov.uk"
        });

        await using var context = await CreateContextAsync(connectionString);
        var sut = new DatabaseSeeder(context, configuration);

        await sut.SeedAsync();

        Assert.Equal(31, await context.Users.CountAsync());
        Assert.Equal(1, await context.Users.CountAsync(u => u.Email == "new.person@education.gov.uk"));
    }

    public void Dispose()
    {
        foreach (var dbFile in _databaseFiles.Where(File.Exists))
        {
            try
            {
                File.Delete(dbFile);
            }
            catch (IOException)
            {
                // SQLite can hold short-lived file handles after context disposal; cleanup is best-effort.
            }
        }
    }

    private static async Task<CompleteContext> CreateContextAsync(string connectionString)
    {
        var options = new DbContextOptionsBuilder<CompleteContext>()
            .UseSqlite(connectionString)
            .Options;

        var context = new CompleteContext(options);
        await context.Database.EnsureCreatedAsync();
        return context;
    }

    private string CreateConnectionString(bool isLocal)
    {
        var marker = isLocal ? "localhost" : "remote";
        var dbFile = Path.Combine(Path.GetTempPath(), $"complete-seeder-{marker}-{Guid.NewGuid():N}.db");
        _databaseFiles.Add(dbFile);
        return $"Data Source={dbFile};Pooling=False";
    }

    private static IConfiguration BuildConfiguration(Dictionary<string, string?>? seedValues = null)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(seedValues ?? new Dictionary<string, string?>())
            .Build();
    }
}