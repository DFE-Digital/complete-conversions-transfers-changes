using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Infrastructure.Database;

public class DatabaseSeeder
{
    private readonly CompleteContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(CompleteContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Seeds the database with all reference and sample data for development
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Starting database seeding...");
            await SeedDaoRevocationReasonsAsync();
            await SeedSignificantDateHistoryReasonsAsync();
            await SeedLocalAuthoritiesAsync();
            await SeedUsersAsync();
            await SeedProjectDataAsync();
            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database seeding failed during {Phase}", "initialization");
            throw new InvalidOperationException("Database seeding operation failed. See inner exception for details.", ex);
        }
    }

    private async Task SeedDaoRevocationReasonsAsync()
    {
        if (await _context.DaoRevocationReasons.AnyAsync())
        {
            _logger.LogInformation("DAO revocation reasons already seeded, skipping...");
            return;
        }

        _logger.LogInformation("Seeding DAO revocation reasons...");

        var reasons = new List<DaoRevocationReason>
        {
            new() { Id = new DaoRevocationReasonId(Guid.NewGuid()), ReasonType = "Financial concerns" },
            new() { Id = new DaoRevocationReasonId(Guid.NewGuid()), ReasonType = "Governance issues" },
            new() { Id = new DaoRevocationReasonId(Guid.NewGuid()), ReasonType = "Educational performance" },
            new() { Id = new DaoRevocationReasonId(Guid.NewGuid()), ReasonType = "Safeguarding concerns" },
            new() { Id = new DaoRevocationReasonId(Guid.NewGuid()), ReasonType = "Compliance failures" },
            new() { Id = new DaoRevocationReasonId(Guid.NewGuid()), ReasonType = "Other" }
        };

        await _context.DaoRevocationReasons.AddRangeAsync(reasons);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Seeded {Count} DAO revocation reasons", reasons.Count);
    }

    private async Task SeedSignificantDateHistoryReasonsAsync()
    {
        if (await _context.SignificantDateHistoryReasons.AnyAsync())
        {
            _logger.LogInformation("Significant date history reasons already seeded, skipping...");
            return;
        }

        _logger.LogInformation("Seeding significant date history reasons...");

        var reasons = new List<SignificantDateHistoryReason>
        {
            new() { Id = new SignificantDateHistoryReasonId(Guid.NewGuid()), ReasonType = "Legal requirements", CreatedAt = DateTime.UtcNow },
            new() { Id = new SignificantDateHistoryReasonId(Guid.NewGuid()), ReasonType = "Academy readiness", CreatedAt = DateTime.UtcNow },
            new() { Id = new SignificantDateHistoryReasonId(Guid.NewGuid()), ReasonType = "Stakeholder consultation", CreatedAt = DateTime.UtcNow },
            new() { Id = new SignificantDateHistoryReasonId(Guid.NewGuid()), ReasonType = "Financial planning", CreatedAt = DateTime.UtcNow },
            new() { Id = new SignificantDateHistoryReasonId(Guid.NewGuid()), ReasonType = "Due diligence", CreatedAt = DateTime.UtcNow },
            new() { Id = new SignificantDateHistoryReasonId(Guid.NewGuid()), ReasonType = "External factors", CreatedAt = DateTime.UtcNow },
            new() { Id = new SignificantDateHistoryReasonId(Guid.NewGuid()), ReasonType = "Other", CreatedAt = DateTime.UtcNow }
        };

        await _context.SignificantDateHistoryReasons.AddRangeAsync(reasons);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Seeded {Count} significant date history reasons", reasons.Count);
    }

    private async Task SeedLocalAuthoritiesAsync()
    {
        if (await _context.LocalAuthorities.AnyAsync())
        {
            _logger.LogInformation("Local authorities already seeded, skipping...");
            return;
        }

        _logger.LogInformation("Seeding local authorities...");

        var localAuthorities = new List<LocalAuthority>
        {
            // Major local authorities - in production this would come from an external source
            new() { Code = "330", Name = "Birmingham", CreatedAt = DateTime.UtcNow },
            new() { Code = "370", Name = "Bradford", CreatedAt = DateTime.UtcNow },
            new() { Code = "380", Name = "Calderdale", CreatedAt = DateTime.UtcNow },
            new() { Code = "381", Name = "Kirklees", CreatedAt = DateTime.UtcNow },
            new() { Code = "382", Name = "Leeds", CreatedAt = DateTime.UtcNow },
            new() { Code = "383", Name = "Wakefield", CreatedAt = DateTime.UtcNow },
            new() { Code = "390", Name = "Gateshead", CreatedAt = DateTime.UtcNow },
            new() { Code = "391", Name = "Newcastle upon Tyne", CreatedAt = DateTime.UtcNow },
            new() { Code = "392", Name = "North Tyneside", CreatedAt = DateTime.UtcNow },
            new() { Code = "393", Name = "South Tyneside", CreatedAt = DateTime.UtcNow },
            new() { Code = "394", Name = "Sunderland", CreatedAt = DateTime.UtcNow },
            new() { Code = "201", Name = "City of London", CreatedAt = DateTime.UtcNow },
            new() { Code = "202", Name = "Camden", CreatedAt = DateTime.UtcNow },
            new() { Code = "204", Name = "Hackney", CreatedAt = DateTime.UtcNow },
            new() { Code = "309", Name = "Hartlepool", CreatedAt = DateTime.UtcNow },
            new() { Code = "310", Name = "Middlesbrough", CreatedAt = DateTime.UtcNow },
            new() { Code = "311", Name = "Redcar and Cleveland", CreatedAt = DateTime.UtcNow },
            new() { Code = "312", Name = "Stockton-on-Tees", CreatedAt = DateTime.UtcNow }
        };

        await _context.LocalAuthorities.AddRangeAsync(localAuthorities);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Seeded {Count} local authorities", localAuthorities.Count);
    }

    private async Task SeedUsersAsync()
    {
        if (await _context.Users.AnyAsync())
        {
            _logger.LogInformation("Users already seeded, skipping...");
            return;
        }

        _logger.LogInformation("Seeding default users...");

        var users = new List<User>
        {
            new()
            {
                Id = new UserId(Guid.NewGuid()),
                Email = "admin@education.gov.uk",
                FirstName = "System",
                LastName = "Administrator",
                Team = ProjectTeam.London.ToDescription(),
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = new UserId(Guid.NewGuid()),
                Email = "delivery.officer.north@education.gov.uk",
                FirstName = "Delivery",
                LastName = "Officer North",
                Team = ProjectTeam.NorthEast.ToDescription(),
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = new UserId(Guid.NewGuid()),
                Email = "delivery.officer.midlands@education.gov.uk",
                FirstName = "Delivery",
                LastName = "Officer Midlands",
                Team = ProjectTeam.WestMidlands.ToDescription(),
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = new UserId(Guid.NewGuid()),
                Email = "delivery.officer.southwest@education.gov.uk",
                FirstName = "Delivery",
                LastName = "Officer Southwest",
                Team = ProjectTeam.SouthWest.ToDescription(),
                CreatedAt = DateTime.UtcNow
            }
        };

        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Seeded {Count} default users", users.Count);
    }

    /// <summary>
    /// Seeds project data for development
    /// </summary>
    private async Task SeedProjectDataAsync()
    {
        if (await _context.ProjectGroups.AnyAsync())
        {
            _logger.LogInformation("Project groups already exist, skipping project data seeding...");
            return;
        }

        _logger.LogInformation("Seeding project data...");

        var projectGroup = new ProjectGroup
        {
            Id = new ProjectGroupId(Guid.NewGuid()),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.ProjectGroups.AddAsync(projectGroup);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Project data seeding completed");
    }
}