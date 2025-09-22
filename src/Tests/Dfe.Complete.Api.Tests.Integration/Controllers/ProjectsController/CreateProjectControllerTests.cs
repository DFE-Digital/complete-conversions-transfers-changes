using System.Net;
using System.Security.Claims;
using AutoFixture;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using Dfe.Complete.Tests.Common.Customizations.Commands;
using Dfe.Complete.Tests.Common.Customizations.Models;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using GovUK.Dfe.CoreLibs.Testing.Mocks.WebApplicationFactory;
using Microsoft.EntityFrameworkCore;
using GiasEstablishment = Dfe.Complete.Domain.Entities.GiasEstablishment;

namespace Dfe.Complete.Api.Tests.Integration.Controllers.ProjectsController;

public partial class ProjectsControllerTests
{
    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization),
        typeof(CreateConversionProjectCommandCustomization))]
    public async Task CreateHandoverConversionProject_Async_ShouldCreateHandoverConversionProject(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();
        var testUser = await dbContext.Users.OrderBy(u => u.CreatedAt).FirstOrDefaultAsync();
        Assert.NotNull(testUser);

        var createHandoverConversionProjectCommand = new CreateHandoverConversionProjectCommand()
        {
            Urn = 121999,
            IncomingTrustUkprn = 12129999,
            AdvisoryBoardDate = DateTime.Parse("2025-05-02"),
            ProvisionalConversionDate = DateTime.Parse("2025-05-01"),
            CreatedByEmail = "testuseremail@education.gov.uk",
            CreatedByFirstName = "First name",
            CreatedByLastName = "Last name",
            PrepareId = 123,
            DirectiveAcademyOrder = true,
            AdvisoryBoardConditions = "Advisory board conditions"
        };

        testUser.Email = createHandoverConversionProjectCommand.CreatedByEmail;

        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);

        dbContext.Users.Update(testUser);
        var giasEstablishment = fixture
            .Customize(new GiasEstablishmentsCustomization()
            {
                LocalAuthority = localAuthority,
                Urn = new Domain.ValueObjects.Urn(createHandoverConversionProjectCommand.Urn.Value)
            })
            .Create<GiasEstablishment>();
        await dbContext.GiasEstablishments.AddAsync(giasEstablishment);
        await dbContext.SaveChangesAsync();

        var result = await projectsClient.CreateHandoverConversionProjectAsync(createHandoverConversionProjectCommand);

        Assert.NotNull(result);
        Assert.IsType<ProjectId>(result);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(CustomWebApplicationDbContextFactoryCustomization))]
    public async Task CreateHandoverConversionProject_WithBadGroupIdentifier_FailsValidation(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var createHandoverConversionProjectCommand = GenerateCreateHandoverConversionCommand();
        createHandoverConversionProjectCommand.GroupId = "invalid-id";

        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateHandoverConversionProjectAsync(createHandoverConversionProjectCommand));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);

        var validationErrors = exception.Response;
        Assert.NotNull(validationErrors);

        Assert.Contains("Group ID must match format GRP_XXXXXXXX (8 digits)", validationErrors);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(CustomWebApplicationDbContextFactoryCustomization))]
    public async Task CreateHandoverConversionProject_WithBadEmail_FailsValidation(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var createHandoverConversionProjectCommand = GenerateCreateHandoverConversionCommand();
        createHandoverConversionProjectCommand.CreatedByEmail = "invalid@notmail.com";

        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateHandoverConversionProjectAsync(createHandoverConversionProjectCommand));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);

        var validationErrors = exception.Response;
        Assert.NotNull(validationErrors);

        Assert.Contains("Created by email must be from @education.gov.uk domain", validationErrors);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(CustomWebApplicationDbContextFactoryCustomization))]
    public async Task CreateHandoverConversionProject_WithEmptyBody_FailsValidation(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var createHandoverConversionProjectCommand = new CreateHandoverConversionProjectCommand();

        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateHandoverConversionProjectAsync(createHandoverConversionProjectCommand));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);

        var validationErrors = exception.Response;
        Assert.NotNull(validationErrors);

        Assert.Contains("The Urn field is required.", validationErrors);
        Assert.Contains("The PrepareId field is required.", validationErrors);
        Assert.Contains("The CreatedByEmail field is required.", validationErrors);
        Assert.Contains("The AdvisoryBoardDate field is required.", validationErrors);
        Assert.Contains("The CreatedByLastName field is required.", validationErrors);
        Assert.Contains("The CreatedByFirstName field is required.", validationErrors);
        Assert.Contains("The IncomingTrustUkprn field is required.", validationErrors);
        Assert.Contains("The DirectiveAcademyOrder field is required.", validationErrors);
        Assert.Contains("The ProvisionalConversionDate field is required.", validationErrors);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization),
        typeof(CreateConversionProjectCommandCustomization))]
    public async Task CreateConversionProject_Async_ShouldCreateConversionProject(
        CustomWebApplicationDbContextFactory<Program> factory,
        CreateConversionProjectCommand createConversionProjectCommand,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();

        var testUser = await dbContext.Users.OrderBy(u => u.CreatedAt).FirstOrDefaultAsync();
        Assert.NotNull(testUser);
        testUser.ActiveDirectoryUserId = createConversionProjectCommand.UserAdId;

        var group = await dbContext.ProjectGroups.FirstOrDefaultAsync();
        Assert.NotNull(group);
        group.GroupIdentifier = createConversionProjectCommand.GroupReferenceNumber;

        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);

        dbContext.Users.Update(testUser);
        dbContext.ProjectGroups.Update(group);
        var giasEstablishment = fixture
            .Customize(new GiasEstablishmentsCustomization() { LocalAuthority = localAuthority })
            .Create<GiasEstablishment>();
        await dbContext.GiasEstablishments.AddAsync(giasEstablishment);

        createConversionProjectCommand.Urn = new Urn { Value = giasEstablishment.Urn?.Value };

        await dbContext.SaveChangesAsync();

        var result = await projectsClient.CreateConversionProjectAsync(createConversionProjectCommand);

        Assert.NotNull(result);
        Assert.IsType<ProjectId>(result);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(CustomWebApplicationDbContextFactoryCustomization))]
    public async Task CreateConversionProject_WithNullRequest_ThrowsException(
        CustomWebApplicationDbContextFactory<Program> factory,
        CreateConversionProjectCommand createConversionProjectCommand,
        IProjectsClient projectsClient)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        createConversionProjectCommand.Urn = null;

        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateConversionProjectAsync(createConversionProjectCommand));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization),
        typeof(CreateTransferProjectCommandCustomization))]
    public async Task CreateTransferProject_Async_ShouldCreateTransferProject(
        CustomWebApplicationDbContextFactory<Program> factory,
        CreateTransferProjectCommand createTransferProjectCommand,
        IProjectsClient projectsClient)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var dbContext = factory.GetDbContext<CompleteContext>();

        var testUser = await dbContext.Users.OrderBy(user => user.CreatedAt).FirstOrDefaultAsync();
        Assert.NotNull(testUser);
        testUser.ActiveDirectoryUserId = createTransferProjectCommand.UserAdId;

        var group = await dbContext.ProjectGroups.FirstOrDefaultAsync();
        Assert.NotNull(group);
        group.GroupIdentifier = createTransferProjectCommand.GroupReferenceNumber;

        dbContext.Users.Update(testUser);
        dbContext.ProjectGroups.Update(group);

        var giasEstablishment = await dbContext.GiasEstablishments.FirstOrDefaultAsync();

        createTransferProjectCommand.Urn = new Urn { Value = giasEstablishment?.Urn?.Value };

        await dbContext.SaveChangesAsync();

        var result = await projectsClient.CreateTransferProjectAsync(createTransferProjectCommand);

        Assert.NotNull(result);
        Assert.IsType<ProjectId>(result);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(CustomWebApplicationDbContextFactoryCustomization))]
    public async Task CreateTransferProject_WithNullRequest_ThrowsException(
        CustomWebApplicationDbContextFactory<Program> factory,
        CreateTransferProjectCommand createTransferProjectCommand,
        IProjectsClient projectsClient)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        createTransferProjectCommand.Urn = null;

        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateTransferProjectAsync(createTransferProjectCommand));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization),
        typeof(CreateMatConversionProjectCommandCustomization))]
    public async Task CreateMatConversionProject_Async_ShouldCreateMatConversionProject(
        CustomWebApplicationDbContextFactory<Program> factory,
        CreateMatConversionProjectCommand createMatConversionProjectCommand,
        IProjectsClient projectsClient)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var dbContext = factory.GetDbContext<CompleteContext>();

        var testUser = await dbContext.Users.OrderBy(user => user.CreatedAt).FirstOrDefaultAsync();
        Assert.NotNull(testUser);
        testUser.ActiveDirectoryUserId = createMatConversionProjectCommand.UserAdId;

        var group = await dbContext.ProjectGroups.FirstOrDefaultAsync();
        Assert.NotNull(group);

        dbContext.Users.Update(testUser);
        dbContext.ProjectGroups.Update(group);

        var giasEstablishment = await dbContext.GiasEstablishments.FirstOrDefaultAsync();

        createMatConversionProjectCommand.Urn = new Urn { Value = giasEstablishment?.Urn?.Value };

        await dbContext.SaveChangesAsync();

        var result = await projectsClient.CreateMatConversionProjectAsync(createMatConversionProjectCommand);

        Assert.NotNull(result);
        Assert.IsType<ProjectId>(result);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(CustomWebApplicationDbContextFactoryCustomization))]
    public async Task CreateMatConversionProject_WithNullRequest_ThrowsException(
        CustomWebApplicationDbContextFactory<Program> factory,
        CreateMatConversionProjectCommand createMatConversionProjectCommand,
        IProjectsClient projectsClient)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        createMatConversionProjectCommand.Urn = null;

        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateMatConversionProjectAsync(createMatConversionProjectCommand));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization),
        typeof(CreateMatTransferProjectCommandCustomization))]
    public async Task CreateMatTransferProject_Async_ShouldCreateMatTransferProject(
        CustomWebApplicationDbContextFactory<Program> factory,
        CreateMatTransferProjectCommand createMatTransferProjectCommand,
        IProjectsClient projectsClient)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var dbContext = factory.GetDbContext<CompleteContext>();

        var testUser = await dbContext.Users.OrderBy(user => user.CreatedAt).FirstOrDefaultAsync();
        Assert.NotNull(testUser);
        testUser.ActiveDirectoryUserId = createMatTransferProjectCommand.UserAdId;

        var group = await dbContext.ProjectGroups.FirstOrDefaultAsync();
        Assert.NotNull(group);

        dbContext.Users.Update(testUser);
        dbContext.ProjectGroups.Update(group);

        var giasEstablishment = await dbContext.GiasEstablishments.FirstOrDefaultAsync();

        createMatTransferProjectCommand.Urn = new Urn { Value = giasEstablishment?.Urn?.Value };

        await dbContext.SaveChangesAsync();

        var result = await projectsClient.CreateMatTransferProjectAsync(createMatTransferProjectCommand);

        Assert.NotNull(result);
        Assert.IsType<ProjectId>(result);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(CustomWebApplicationDbContextFactoryCustomization))]
    public async Task CreateMatTransferProject_WithNullRequest_ThrowsException(
        CustomWebApplicationDbContextFactory<Program> factory,
        CreateMatTransferProjectCommand createMatTransferProjectCommand,
        IProjectsClient projectsClient)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        createMatTransferProjectCommand.Urn = null;

        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateMatTransferProjectAsync(createMatTransferProjectCommand));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);
    }

    private static CreateHandoverConversionProjectCommand GenerateCreateHandoverConversionCommand() => new()
    {
        Urn = 121999,
        IncomingTrustUkprn = 12129999,
        AdvisoryBoardDate = DateTime.Parse("2025-05-02"),
        ProvisionalConversionDate = DateTime.Parse("2025-05-01"),
        CreatedByEmail = "test@education.gov.uk",
        CreatedByFirstName = "Test",
        CreatedByLastName = "User",
        PrepareId = 123,
        DirectiveAcademyOrder = true,
        AdvisoryBoardConditions = "Advisory board conditions"
    };
}