using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Dfe.Complete.Infrastructure.Database;

public class DatabaseSeeder(CompleteContext context, ILogger<DatabaseSeeder> logger, IConfiguration configuration)
{
    private readonly CompleteContext _context = context;
    private readonly ILogger<DatabaseSeeder> _logger = logger;
    private readonly IConfiguration _configuration = configuration;

    private static DateTime Now => DateTime.UtcNow;

    /// <summary>
    /// Seeds the database with all reference and sample data for development
    /// </summary>
    public async Task SeedAsync()
    {
        // Only run if DB is local/dev
        var connStr = _context.Database.GetConnectionString();
        if (string.IsNullOrEmpty(connStr) || !(connStr.Contains("localhost") || connStr.Contains("127.0.0.1") || connStr.Contains("localdb", StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine("[Seeder] Skipping all seeding: not a local DB connection");
            _logger.LogInformation("Skipping all seeding: not a local DB connection");
            return;
        }
        try
        {
            Console.WriteLine("[Seeder] Starting database seeding...");
            _logger.LogInformation("Starting database seeding...");

            // The order of seeding matters due to foreign key relationships, so we seed in a specific sequence. Each method is idempotent and checks for existing data before seeding.
            await SeedUsersAsync();
            await SeedLocalAuthoritiesAsync();
            await SeedSignificantDateHistoryReasonsAsync();
            await SeedProjectsAsync();
            await SeedGiasEstablishmentsAsync();

            Console.WriteLine("[Seeder] Database seeding completed successfully");
            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Seeder] ERROR: {ex.Message}\n{ex}");
            _logger.LogError(ex, "Database seeding failed during {Phase}", "initialization");
            throw new InvalidOperationException("Database seeding operation failed. See inner exception for details.", ex);
        }
    }

    private async Task SeedSignificantDateHistoryReasonsAsync()
    {
        if (await _context.SignificantDateHistoryReasons.AnyAsync())
        {
            Console.WriteLine("[Seeder] Significant date history reasons already seeded, skipping...");
            _logger.LogInformation("Significant date history reasons already seeded, skipping...");
            return;
        }

        Console.WriteLine("[Seeder] Seeding significant date history reasons...");
        _logger.LogInformation("Seeding significant date history reasons...");

        var reasons = new List<SignificantDateHistoryReason>
        {
            new() { Id = new SignificantDateHistoryReasonId(Guid.NewGuid()), ReasonType = "Legal requirements", CreatedAt = Now },
            new() { Id = new SignificantDateHistoryReasonId(Guid.NewGuid()), ReasonType = "Academy readiness", CreatedAt = Now },
            new() { Id = new SignificantDateHistoryReasonId(Guid.NewGuid()), ReasonType = "Stuck in the upside-down", CreatedAt = Now },
            new() { Id = new SignificantDateHistoryReasonId(Guid.NewGuid()), ReasonType = "Stakeholder consultation", CreatedAt = Now },
            new() { Id = new SignificantDateHistoryReasonId(Guid.NewGuid()), ReasonType = "Financial planning", CreatedAt = Now },
            new() { Id = new SignificantDateHistoryReasonId(Guid.NewGuid()), ReasonType = "Due diligence", CreatedAt = Now },
            new() { Id = new SignificantDateHistoryReasonId(Guid.NewGuid()), ReasonType = "Demogorgon attack", CreatedAt = Now },
            new() { Id = new SignificantDateHistoryReasonId(Guid.NewGuid()), ReasonType = "External factors", CreatedAt = Now },
            new() { Id = new SignificantDateHistoryReasonId(Guid.NewGuid()), ReasonType = "Other", CreatedAt = Now }
        };

        await _context.SignificantDateHistoryReasons.AddRangeAsync(reasons);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded {Count} significant date history reasons", reasons.Count);
    }

    private async Task SeedLocalAuthoritiesAsync()
    {
        if (await _context.LocalAuthorities.AnyAsync())
        {
            Console.WriteLine("[Seeder] Local authorities already seeded, skipping...");
            _logger.LogInformation("Local authorities already seeded, skipping...");
            return;
        }

        Console.WriteLine("[Seeder] Seeding local authorities...");
        _logger.LogInformation("Seeding local authorities...");

        await _context.LocalAuthorities.AddRangeAsync(LocalAuthorities);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} local authorities", LocalAuthorities.Count);
    }

    private async Task SeedGiasEstablishmentsAsync()
    {
        if (await _context.GiasEstablishments.AnyAsync())
        {
            Console.WriteLine("[Seeder] GIAS establishments already seeded, skipping...");
            _logger.LogInformation("GIAS establishments already seeded, skipping...");
            return;
        }

        Console.WriteLine("[Seeder] Seeding GIAS establishments...");
        _logger.LogInformation("Seeding GIAS establishments...");

        var giasEstablishments = new List<GiasEstablishment>();
        for (var i = 0; i < Urns.Count; i++)
        {
            var localAuthority = RandomFromList(LocalAuthorities);

            giasEstablishments.Add(new GiasEstablishment
            {
                Id = new GiasEstablishmentId(Guid.NewGuid()),
                Ukprn = Ukprns[i % Ukprns.Count],
                Urn = Urns[i],
                Name = EstablishmentNames[i % EstablishmentNames.Count],
                LocalAuthorityName = localAuthority.Name,
                LocalAuthorityCode = localAuthority.Code,

                CreatedAt = Now
            });
        }
        await _context.GiasEstablishments.AddRangeAsync(giasEstablishments);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} GIAS establishments", giasEstablishments.Count);
    }

    private async Task SeedUsersAsync()
    {
        Console.WriteLine("[Seeder] Seeding default users...");
        // Always ensure default users exist (idempotent)
        foreach (var user in DefaultUsers)
        {
            if (!await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                await _context.Users.AddAsync(user);
            }
        }
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded default users if missing");
        Console.WriteLine("[Seeder] Seeded default users if missing");

        // Local developer user seeding using user secrets
        await SeedLocalDeveloperUsersAsync();
    }

    private async Task SeedLocalDeveloperUsersAsync()
    {
        // Try to get user secrets via configuration
        var emailsRaw = _configuration["LocalSeed:UserEmails"];
        if (string.IsNullOrWhiteSpace(emailsRaw))
        {
            Console.WriteLine("[Seeder] No LocalSeed:UserEmails secret set");
            _logger.LogInformation("No LocalSeed:UserEmails secret set");
            return;
        }

        var emails = emailsRaw.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(e => e.ToLowerInvariant())
            .Distinct()
            .ToList();

        var validEmails = new List<(string Email, string First, string Last)>();
        foreach (var email in emails)
        {
            // Validate format: firstname.lastname@education.gov.uk
            if (!email.EndsWith("@education.gov.uk"))
            {
                Console.WriteLine($"[Seeder] Skipping invalid email (wrong domain): {email}");
                _logger.LogWarning("Skipping invalid email (wrong domain): {Email}", email);
                continue;
            }
            var local = email[..^"@education.gov.uk".Length];
            var parts = local.Split('.');
            if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
            {
                Console.WriteLine($"[Seeder] Skipping invalid email (not firstname.lastname): {email}");
                _logger.LogWarning("Skipping invalid email (not firstname.lastname): {Email}", email);
                continue;
            }
            var first = Capitalize(parts[0]);
            var last = Capitalize(parts[1]);
            validEmails.Add((Email: email, First: first, Last: last));
        }

        int seeded = 0;
        foreach (var (email, first, last) in validEmails)
        {
            if (!await _context.Users.AnyAsync(u => u.Email == email))
            {
                var user = CreateUser(first, last, email, ProjectTeam.ServiceSupport);
                await _context.Users.AddAsync(user);
                seeded++;
            }
        }
        if (seeded > 0)
        {
            await _context.SaveChangesAsync();
            Console.WriteLine($"[Seeder] Seeded {seeded} local developer users from user secrets");
            _logger.LogInformation("Seeded {Count} local developer users from user secrets", seeded);
        }
        else
        {
            Console.WriteLine("[Seeder] No new local developer users to seed");
            _logger.LogInformation("No new local developer users to seed");
        }

        static string Capitalize(string s) => string.IsNullOrEmpty(s) ? s : char.ToUpperInvariant(s[0]) + s[1..];
    }

    private async Task SeedProjectsAsync()
    {
        if (await _context.ProjectGroups.AnyAsync())
        {
            Console.WriteLine("[Seeder] Project groups already exist, skipping project data seeding...");
            _logger.LogInformation("Project groups already exist, skipping project data seeding...");
            return;
        }

        Console.WriteLine("[Seeder] Seeding project data...");
        _logger.LogInformation("Seeding project data...");

        // Fetch users and local authorities from the database for correct IDs
        var users = await _context.Users.ToListAsync();
        var localAuthorities = await _context.LocalAuthorities.ToListAsync();

        var (projectGroup, conversionProjects, conversionTasks) = SetupConversionProjects(users, localAuthorities);
        var (transferProjects, transferTasks) = SetupTransferProjects(users, localAuthorities);

        var allProjects = conversionProjects.Concat(transferProjects).ToList();
        var keyContacts = allProjects
            .Select(proj => new KeyContact
            {
                Id = new KeyContactId(Guid.NewGuid()),
                ProjectId = proj.Id,
                CreatedAt = Now,
                UpdatedAt = Now
            })
            .ToList();

        _context.ConversionTasksData.AddRange(conversionTasks);
        _context.TransferTasksData.AddRange(transferTasks);
        _context.ProjectGroups.Add(projectGroup);
        _context.Projects.AddRange(allProjects);
        _context.KeyContacts.AddRange(keyContacts);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Seeder] ERROR seeding projects: {ex.Message}\n{ex}");
            _logger.LogError(ex, "Error occurred while seeding projects");
            throw;
        }

        Console.WriteLine("[Seeder] Project data seeding completed");
        _logger.LogInformation("Project data seeding completed");
    }

    private static (ProjectGroup, List<Project>, List<ConversionTasksData>) SetupConversionProjects(List<User> users, List<LocalAuthority> localAuthorities)
    {
        var projectGroup = new ProjectGroup
        {
            Id = new ProjectGroupId(Guid.NewGuid()),
            GroupIdentifier = "GRP_00000001",
            TrustUkprn = Ukprns[0],
            CreatedAt = Now,
            UpdatedAt = Now
        };

        List<Project> projects = [];
        List<ConversionTasksData> tasks = [];
        for (var i = 0; i < 15; i++)
        {
            // Create task entity
            var taskListItem = new ConversionTasksData(new TaskDataId(Guid.NewGuid()), Now, Now);

            var localAuthority = RandomFromList(localAuthorities);
            var createdByUser = RandomFromList(users);
            var assignedToUser = RandomFromList(users);
            var caseworkerUser = RandomFromList(users);

            var conversion =
                Project.CreateConversionProject(new CreateConversionProjectParams(
                new ProjectId(Guid.NewGuid()),
                Urns[i],
                taskListItem.Id.Value,
                DateOnly.FromDateTime(Now.AddDays(i)),
                Ukprns[i % Ukprns.Count],
                RandomFromList(Regions),
                i % 2 == 0,
                DateOnly.FromDateTime(Now.AddDays(-(i + 10))),
                i % 3 == 0 ? "Advisory board conditions" : null,
                i % Ukprns.Count == 0 ? projectGroup.Id : null,
                createdByUser.Id,
                localAuthority.Id.Value
            ));

            conversion.AssignedAt = new DateTimeOffset(Now.AddDays(-i)).DateTime;
            conversion.AssignedToId = assignedToUser.Id;
            conversion.CaseworkerId = caseworkerUser.Id;

            if (i >= 10)
            {
                conversion.NewTrustReferenceNumber = $"TR{10000 + i}";
                conversion.NewTrustName = $"The {assignedToUser.FirstName} {caseworkerUser.LastName} Trust";
                conversion.IncomingTrustUkprn = null;
            }

            if (i < 7 || i > 12) conversion.State = ProjectState.Active;
            projects.Add(conversion);
            tasks.Add(taskListItem);
        }
        return (projectGroup, projects, tasks);
    }

    private static (List<Project>, List<TransferTasksData>) SetupTransferProjects(List<User> users, List<LocalAuthority> localAuthorities)
    {
        List<Project> projects = [];
        List<TransferTasksData> tasks = [];
        for (var i = 15; i < Urns.Count; i++)
        {
            // Create task entity
            var taskListItem = new TransferTasksData(new TaskDataId(Guid.NewGuid()), Now, Now, false, false, false);

            var localAuthority = RandomFromList(localAuthorities);
            var createdByUser = RandomFromList(users);
            var assignedToUser = RandomFromList(users);
            var caseworkerUser = RandomFromList(users);

            // Create project entity
            var transfer =
                Project.CreateTransferProject(new CreateTransferProjectParams(
                new ProjectId(Guid.NewGuid()),
                Urns[i],
                taskListItem.Id.Value,
                DateOnly.FromDateTime(Now.AddDays(i)),
                Ukprns[(i + 2) % Ukprns.Count],
                Ukprns[(i + 5) % Ukprns.Count],
                RandomFromList(Regions),
                DateOnly.FromDateTime(Now.AddDays(-(i + 10))),
                i % 3 == 0 ? "Advisory board conditions" : null,
                null,
                createdByUser.Id,
                localAuthority.Id.Value
            ));

            transfer.AssignedAt = new DateTimeOffset(Now.AddDays(-i)).DateTime;
            transfer.AssignedToId = assignedToUser.Id;
            transfer.CaseworkerId = caseworkerUser.Id;

            if (i >= 25)
            {
                transfer.NewTrustReferenceNumber = $"TR{10000 + i}";
                transfer.NewTrustName = $"The {assignedToUser.FirstName} {caseworkerUser.LastName} Trust";
                transfer.IncomingTrustUkprn = null;
            }

            if (i < 22 || i > 27) transfer.State = ProjectState.Active;
            projects.Add(transfer);
            tasks.Add(taskListItem);
        }

        return (projects, tasks);
    }

    private static IReadOnlyList<User> DefaultUsers => [
        CreateUser("Joyce", "Byers", "joyce.byers@education.gov.uk", ProjectTeam.London),
        CreateUser("Jim", "Hopper", "jim.hopper@education.gov.uk", ProjectTeam.London),
        CreateUser("Mike", "Wheeler", "mike.wheeler@education.gov.uk", ProjectTeam.London),
        CreateUser("Jane", "Hopper", "jane.hopper11@education.gov.uk", ProjectTeam.London),

        CreateUser("Dustin", "Henderson", "dustin.henderson@education.gov.uk", ProjectTeam.SouthEast),
        CreateUser("Lucas", "Sinclair", "lucas.sinclair@education.gov.uk", ProjectTeam.SouthEast),
        CreateUser("Nancy", "Wheeler", "nancy.wheeler@education.gov.uk", ProjectTeam.SouthEast),
        CreateUser("Jonathan", "Byers", "jonathan.byers@education.gov.uk", ProjectTeam.SouthEast),

        CreateUser("Karen", "Wheeler", "karen.wheeler@education.gov.uk", ProjectTeam.EastMidlands),
        CreateUser("Martin", "Brenner", "martin.brenner@education.gov.uk", ProjectTeam.EastMidlands),
        CreateUser("Will", "Byers", "will.byers@education.gov.uk", ProjectTeam.EastMidlands),
        CreateUser("Max", "Mayfield", "max.mayfield@education.gov.uk", ProjectTeam.EastMidlands),

        CreateUser("Steve", "Harrington", "steve.harrington@education.gov.uk", ProjectTeam.WestMidlands),
        CreateUser("Billy", "Hargrove", "billy.hargrove@education.gov.uk", ProjectTeam.WestMidlands),
        CreateUser("Sam", "Owens", "sam.owens@education.gov.uk", ProjectTeam.WestMidlands),
        CreateUser("Bob", "Newby", "bob.newby@education.gov.uk", ProjectTeam.WestMidlands),

        CreateUser("Robin", "Buckley", "robin.buckley@education.gov.uk", ProjectTeam.NorthWest),
        CreateUser("Erica", "Sinclair", "erica.sinclair@education.gov.uk", ProjectTeam.NorthWest),
        CreateUser("Murray", "Bauman", "murray.bauman@education.gov.uk", ProjectTeam.NorthWest),
        CreateUser("Kay", "Unknown", "kay.unknown@education.gov.uk", ProjectTeam.NorthWest),

        CreateUser("Henry", "Creel", "henry.creel@education.gov.uk", ProjectTeam.NorthEast),
        CreateUser("Holly", "Wheeler", "holly.wheeler@education.gov.uk", ProjectTeam.NorthEast),
    ];

    private static IReadOnlyList<LocalAuthority> LocalAuthorities =>
    [
        LocalAuthority.Create(NewLocalAuthorityId(), "Birmingham City Council", "301", CreateAddress("1 Main Street", "Birmingham", "West Midlands", "B1 1AA"), Now),
        LocalAuthority.Create(NewLocalAuthorityId(), "Hawkins Council", "901", CreateAddress("1 Hopper Lane", "Hawkins", "Indiana", "HW1 1AA"), Now),
        LocalAuthority.Create(NewLocalAuthorityId(), "Lenora Hills Council", "902", CreateAddress("22 Roller Rink Road", "Lenora Hills", "California", "LH2 2BB"), Now),
        LocalAuthority.Create(NewLocalAuthorityId(), "Roane County Council", "903", CreateAddress("4 Byers Street", "Roane County", "Indiana", "RC3 3CC"), Now),
        LocalAuthority.Create(NewLocalAuthorityId(), "Starcourt District Council", "904", CreateAddress("45 Starcourt Mall", "Hawkins", "Indiana", "SD4 4DD"), Now),
        LocalAuthority.Create(NewLocalAuthorityId(), "Mirkwood Borough Council", "905", CreateAddress("12 Mirkwood Drive", "Hawkins", "Indiana", "MB5 5EE"), Now),
        LocalAuthority.Create(NewLocalAuthorityId(), "Lovers Lake Council", "906", CreateAddress("7 Lakeside Avenue", "Lovers Lake", "Indiana", "LL6 6FF"), Now),
        LocalAuthority.Create(NewLocalAuthorityId(), "Forest Hills Council", "907", CreateAddress("18 Forest Hills Park", "Hawkins", "Indiana", "FH7 7GG"), Now),
        LocalAuthority.Create(NewLocalAuthorityId(), "Pennhurst District Council", "908", CreateAddress("3 Pennhurst Road", "Kerley County", "Indiana", "PD8 8HH"), Now),
        LocalAuthority.Create(NewLocalAuthorityId(), "Sunnydale Borough Council", "909", CreateAddress("99 Hellmouth Road", "Sunnydale", "California", "SB9 9II"), Now),
        LocalAuthority.Create(NewLocalAuthorityId(), "Pawnee City Council", "910", CreateAddress("100 Parks Department Way", "Pawnee", "Indiana", "PC1 0JJ"), Now),
        LocalAuthority.Create(NewLocalAuthorityId(), "Springfield Council", "911", CreateAddress("742 Evergreen Terrace", "Springfield", "Unknown", "SC1 1KK"), Now),
        LocalAuthority.Create(NewLocalAuthorityId(), "Hill Valley Council", "912", CreateAddress("88 Clock Tower Square", "Hill Valley", "California", "HV1 2LL"), Now),
        LocalAuthority.Create(NewLocalAuthorityId(), "Twin Peaks Council", "913", CreateAddress("315 Black Lodge Road", "Twin Peaks", "Washington", "TP1 3MM"), Now),
        LocalAuthority.Create(NewLocalAuthorityId(), "Stars Hollow Council", "914", CreateAddress("5 Dragonfly Avenue", "Stars Hollow", "Connecticut", "SH1 4NN"), Now),
        LocalAuthority.Create(NewLocalAuthorityId(), "Riverdale Council", "915", CreateAddress("12 Pop Tate Street", "Riverdale", "New York", "RD1 5OO"), Now),
        LocalAuthority.Create(NewLocalAuthorityId(), "Greendale Council", "916", CreateAddress("101 Community College Road", "Greendale", "Colorado", "GD1 6PP"), Now),
        LocalAuthority.Create(NewLocalAuthorityId(), "Smallville Council", "917", CreateAddress("33 Kent Farm Road", "Smallville", "Kansas", "SV1 7QQ"), Now),
        LocalAuthority.Create(NewLocalAuthorityId(), "Amity Island Council", "918", CreateAddress("2 Shark View", "Amity Island", "Massachusetts", "AI1 8RR"), Now),
        LocalAuthority.Create(NewLocalAuthorityId(), "Bedrock Council", "919", CreateAddress("1 Quarry Road", "Bedrock", "Stone County", "BR1 9SS"), Now),
        LocalAuthority.Create(NewLocalAuthorityId(), "Bikini Bottom Council", "920", CreateAddress("124 Conch Street", "Bikini Bottom", "Pacific Ocean", "BB2 0TT"), Now)
    ];

    public static IReadOnlyList<Ukprn> Ukprns => [new Ukprn(10031575), new Ukprn(10034759), new Ukprn(10037395), new Ukprn(10039603), new Ukprn(10042780), new Ukprn(10034858), new Ukprn(10046414), new Ukprn(10054033), new Ukprn(10054313), new Ukprn(10055361)];

    public static IReadOnlyList<Urn> Urns => [ new Urn(100006), new Urn(100127), new Urn(100129), new Urn(100204), new Urn(100283), new Urn(100352), new Urn(100468), new Urn(102041), new Urn(102776), new Urn(103748), new Urn(103835), new Urn(103839), new Urn(103844), new Urn(103847), new Urn(103886), new Urn(103888), new Urn(104801), new Urn(105601), new Urn(105603), new Urn(105944), new Urn(106859), new Urn(106982), new Urn(107778), new Urn(107780), new Urn(107781), new Urn(107786), new Urn(107793), new Urn(107794), new Urn(108410), new Urn(108590)];
    
    public static IReadOnlyList<string> EstablishmentNames => ["Heath School", "Gordon Primary School", "Haimo Primary School", "King's Oak School", "The Skinners' Company's School for Girls", "St Peter's Primary School", "Colebrooke School", "Latymer All Saints CofE Primary School", "Little Ilford School", "Davenport Lodge School", "Church of the Ascension CofE Primary School", "Oldswinford C of E Primary School", "St Chad's Catholic Primary School", "Halesowen CofE Primary School", "Batmans Hill Nursery School", "Batmans Hill Unit", "St Peter's CofE Primary School", "Abbey College Manchester", "Bollin Cross School", "St Philip's CofE Primary School", "Aston Fence Junior and Infant School", "Abbey Lane Primary School", "Spen Valley High School", "Whitcliffe Mount School", "Gomersal Church of England Voluntary Controlled Middle School", "Huddersfield Grammar School", "Islamia Girls' High School", "Madni Academy", "Kingsmeadow Community School", "Coquet Park First School"];

    public static List<Region> Regions => EnumExtensions.ToList<Region>();

    private static User CreateUser(
        string firstName,
        string lastName,
        string email,
        ProjectTeam team)
    {
        return new User
        {
            Id = new UserId(Guid.NewGuid()),
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Team = team.ToDescription(),
            CreatedAt = Now
        };
    }

    private static AddressDetails CreateAddress(string address1, string town, string county, string postcode) => new(address1, null, null, town, county, postcode);

    private static LocalAuthorityId NewLocalAuthorityId() => new(Guid.NewGuid());

    private static T RandomFromList<T>(IReadOnlyList<T> list)
    {
        var rand = new Random();
        return list[rand.Next(0, list.Count)];
    }
}