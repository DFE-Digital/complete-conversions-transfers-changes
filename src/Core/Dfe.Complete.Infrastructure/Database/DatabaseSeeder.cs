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
    /// <param name="force">If true, will delete existing data and re-seed. Use with caution.</param>
    public async Task SeedAsync(bool force = false)
    {
        try
        {
            _logger.LogInformation("Starting database seeding...");

            if (force)
            {
                _logger.LogWarning("Force seeding enabled - clearing existing data");
                Console.WriteLine("Force seeding enabled - clearing existing data");
                await ClearExistingDataAsync();
            }

            Console.WriteLine("Seeding reference data...");
            await SeedDaoRevocationReasonsAsync();
            Console.WriteLine("Seeding reference data...");
            await SeedSignificantDateHistoryReasonsAsync();
            Console.WriteLine("Seeding reference data...");
            await SeedLocalAuthoritiesAsync();
            Console.WriteLine("Seeding reference data...");
            await SeedUsersAsync();
            Console.WriteLine("Seeding reference data...");
            await SeedProjectDataAsync();
            Console.WriteLine("Seeding reference data..."); 
            
            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database seeding failed during {Phase}", "initialization");
            throw new InvalidOperationException("Database seeding operation failed. See inner exception for details.", ex);
        }
    }

    private async Task ClearExistingDataAsync()
    {
        _logger.LogInformation("Clearing existing data using raw SQL to handle all tables...");
        
        try
        {
            // Disable foreign key constraints temporarily (SQL Server syntax)
            await _context.Database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");
            
            // Clear all tables that might have data (including unmapped ones like projectstest)
            var tablesToClear = new[]
            {
                "[complete].[significant_date_histories]",
                "[complete].[notes]", 
                "[complete].[conversion_tasks_data]",
                "[complete].[transfer_tasks_data]",
                "[complete].[projects]",
                "[complete].[dao_revocations]",
                "[complete].[key_contacts]", 
                "[complete].[contacts]",
                "[complete].[project_groups]",
                "[complete].[users]",
                "[complete].[gias_groups]",
                "[complete].[gias_establishments]",
                "[complete].[significant_date_history_reasons]",
                "[complete].[dao_revocation_reasons]",
                "[complete].[local_authorities]" // Clear this last
            };

            foreach (var table in tablesToClear)
            {
                try
                {
                    Console.WriteLine($"Clearing table {table}...");
                    await _context.Database.ExecuteSqlRawAsync($"DELETE FROM {table}");
                    _logger.LogDebug("Cleared table {Table}", table);
                }
                catch (Exception tableEx)
                {
                    // Log but continue - table might not exist or be empty
                    _logger.LogWarning("Could not clear table {Table}: {Error}", table, tableEx.Message);
                }
            }

            // Re-enable foreign key constraints (SQL Server syntax)
            Console.WriteLine("Re-enabling foreign key constraints...");
            await _context.Database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL'");
            Console.WriteLine("Finished clearing existing data.");
            _logger.LogInformation("Successfully cleared all existing data using raw SQL");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error occurred while clearing existing data: " + ex.Message);
            // Try to re-enable constraints even if clearing failed
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL'");
            }
            catch
            {
                // Ignore errors when trying to restore constraints
            }
            
            _logger.LogError(ex, "Failed to clear existing data using raw SQL");
            throw new InvalidOperationException("Failed to clear existing data. Database might have additional constraints or tables not handled by the seeder.", ex);
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

    /// <summary>
    /// Checks if the database has been seeded with reference and project data
    /// </summary>
    public async Task<bool> IsSeededAsync()
    {
        var hasReasons = await _context.DaoRevocationReasons.AnyAsync();
        var hasLocalAuthorities = await _context.LocalAuthorities.AnyAsync();
        var hasUsers = await _context.Users.AnyAsync();
        var hasProjectGroups = await _context.ProjectGroups.AnyAsync();
        
        return hasReasons && hasLocalAuthorities && hasUsers && hasProjectGroups;
    }
}