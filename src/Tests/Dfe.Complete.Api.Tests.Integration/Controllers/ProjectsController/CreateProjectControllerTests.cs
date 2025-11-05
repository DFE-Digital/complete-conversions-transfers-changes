using System.Globalization;
using System.Net;
using System.Security.Claims;
using AutoFixture;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using Dfe.Complete.Tests.Common.Customizations.Commands;
using Dfe.Complete.Tests.Common.Customizations.Models;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using GovUK.Dfe.CoreLibs.Testing.Mocks.WebApplicationFactory;
using GovUK.Dfe.CoreLibs.Testing.Mocks.WireMock;
using Microsoft.EntityFrameworkCore;
using GiasEstablishment = Dfe.Complete.Domain.Entities.GiasEstablishment;

namespace Dfe.Complete.Api.Tests.Integration.Controllers.ProjectsController;

public partial class ProjectsControllerTests
{
    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization))]
    public async Task CreateHandoverConversionProject_Async_ShouldCreateHandoverConversionProjectOnly(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();
        var testUser = await dbContext.Users.OrderBy(u => u.CreatedAt).FirstOrDefaultAsync();
        Assert.NotNull(testUser);

        var createHandoverConversionProjectCommand = GenerateCreateHandoverConversionCommand();
        testUser.Email = createHandoverConversionProjectCommand.CreatedByEmail;

        Assert.NotNull(factory.WireMockServer);
        var trustDto = fixture.Customize(new TrustDtoCustomization() { Ukprn = createHandoverConversionProjectCommand.IncomingTrustUkprn.ToString() }).Create<TrustDto>();
        factory.WireMockServer.AddGetWithJsonResponse(
            string.Format(TrustClientEndpointConstants.GetTrustByUkprn2Async, createHandoverConversionProjectCommand.IncomingTrustUkprn),
            trustDto);

        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);

        dbContext.Users.Update(testUser);
        var giasEstablishment = fixture
            .Customize(new GiasEstablishmentsCustomization()
            {
                LocalAuthority = localAuthority,
                Urn = new Domain.ValueObjects.Urn(createHandoverConversionProjectCommand.Urn!.Value)
            })
            .Create<GiasEstablishment>();
        await dbContext.GiasEstablishments.AddAsync(giasEstablishment);
        await dbContext.SaveChangesAsync();

        var projectCountBefore = await dbContext.Projects.CountAsync();
        var userCountBefore = await dbContext.Users.CountAsync();
        var conversionTaskDataCountBefore = await dbContext.ConversionTasksData.CountAsync();
        var projectGroupCountBefore = await dbContext.ProjectGroups.CountAsync();

        var result = await projectsClient.CreateHandoverConversionProjectAsync(createHandoverConversionProjectCommand);

        Assert.NotNull(result);
        Assert.IsType<ProjectId>(result);
        Assert.Equal(projectCountBefore + 1, await dbContext.Projects.CountAsync());
        Assert.Equal(userCountBefore, await dbContext.Users.CountAsync());
        Assert.Equal(conversionTaskDataCountBefore + 1, await dbContext.ConversionTasksData.CountAsync());
        Assert.Equal(projectGroupCountBefore, await dbContext.ProjectGroups.CountAsync());

        var createdProject = await dbContext.Projects.FirstOrDefaultAsync(p => p.Id == new Domain.ValueObjects.ProjectId(result.Value!.Value));
        Assert.NotNull(createdProject);
        Assert.Equal("Advisory board conditions", createdProject.AdvisoryBoardConditions);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization))]
    public async Task CreateHandoverConversionProject_Async_ShouldCreateProjectUserAndGroup(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();

        var createHandoverConversionProjectCommand = GenerateCreateHandoverConversionCommand();
        createHandoverConversionProjectCommand.GroupId = "GRP_99999999";

        Assert.NotNull(factory.WireMockServer);
        var trustDto = fixture.Customize(new TrustDtoCustomization() { Ukprn = createHandoverConversionProjectCommand.IncomingTrustUkprn.ToString() }).Create<TrustDto>();
        factory.WireMockServer.AddGetWithJsonResponse(
            string.Format(TrustClientEndpointConstants.GetTrustByUkprn2Async, createHandoverConversionProjectCommand.IncomingTrustUkprn),
            trustDto);

        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);

        var giasEstablishment = fixture
            .Customize(new GiasEstablishmentsCustomization()
            {
                LocalAuthority = localAuthority,
                Urn = new Domain.ValueObjects.Urn(createHandoverConversionProjectCommand.Urn!.Value)
            })
            .Create<GiasEstablishment>();
        await dbContext.GiasEstablishments.AddAsync(giasEstablishment);
        await dbContext.SaveChangesAsync();

        var projectCountBefore = await dbContext.Projects.CountAsync();
        var userCountBefore = await dbContext.Users.CountAsync();
        var conversionTaskDataCountBefore = await dbContext.ConversionTasksData.CountAsync();
        var projectGroupCountBefore = await dbContext.ProjectGroups.CountAsync();

        var result = await projectsClient.CreateHandoverConversionProjectAsync(createHandoverConversionProjectCommand);

        Assert.NotNull(result);
        Assert.IsType<ProjectId>(result);
        Assert.Equal(projectCountBefore + 1, await dbContext.Projects.CountAsync());
        Assert.Equal(userCountBefore + 1, await dbContext.Users.CountAsync());
        Assert.Equal(conversionTaskDataCountBefore + 1, await dbContext.ConversionTasksData.CountAsync());
        Assert.Equal(projectGroupCountBefore + 1, await dbContext.ProjectGroups.CountAsync());
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization))]
    public async Task CreateHandoverConversionProject_Async_NoEstablishmentForUrn_ShouldFailValidation(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();

        var createHandoverConversionProjectCommand = GenerateCreateHandoverConversionCommand();
        createHandoverConversionProjectCommand.GroupId = "GRP_99999999";

        Assert.NotNull(factory.WireMockServer);
        var trustDto = fixture.Customize(new TrustDtoCustomization() { Ukprn = createHandoverConversionProjectCommand.IncomingTrustUkprn.ToString() }).Create<TrustDto>();
        factory.WireMockServer.AddGetWithJsonResponse(
            string.Format(TrustClientEndpointConstants.GetTrustByUkprn2Async, createHandoverConversionProjectCommand.IncomingTrustUkprn),
            trustDto);

        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);

        var giasEstablishment = fixture
            .Customize(new GiasEstablishmentsCustomization()
            {
                LocalAuthority = localAuthority,
                Urn = new Domain.ValueObjects.Urn(111111)
            })
            .Create<GiasEstablishment>();
        await dbContext.GiasEstablishments.AddAsync(giasEstablishment);
        await dbContext.SaveChangesAsync();

        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateHandoverConversionProjectAsync(createHandoverConversionProjectCommand));

        Assert.Equal(HttpStatusCode.UnprocessableEntity, (HttpStatusCode)exception.StatusCode);

        var validationErrors = exception.Response;
        Assert.NotNull(validationErrors);
        Assert.Contains($"No Local authority could be found via Establishments for School Urn: {createHandoverConversionProjectCommand.Urn!.Value}.", validationErrors);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization))]
    public async Task CreateHandoverConversionProject_Async_GroupUkprnDoesNotMatch_ShouldFailValidation(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();

        var createHandoverConversionProjectCommand = GenerateCreateHandoverConversionCommand();
        createHandoverConversionProjectCommand.GroupId = "GRP_88888888";

        Assert.NotNull(factory.WireMockServer);
        var trustDto = fixture.Customize(new TrustDtoCustomization() { Ukprn = "00000000" }).Create<TrustDto>();
        factory.WireMockServer.AddGetWithJsonResponse(
            string.Format(TrustClientEndpointConstants.GetTrustByUkprn2Async, createHandoverConversionProjectCommand.IncomingTrustUkprn),
            trustDto);

        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);

        var giasEstablishment = fixture
            .Customize(new GiasEstablishmentsCustomization()
            {
                LocalAuthority = localAuthority,
                Urn = new Domain.ValueObjects.Urn(createHandoverConversionProjectCommand.Urn!.Value)
            })
            .Create<GiasEstablishment>();
        await dbContext.GiasEstablishments.AddAsync(giasEstablishment);
        await dbContext.ProjectGroups.AddAsync(new Domain.Entities.ProjectGroup
        {
            Id = new Domain.ValueObjects.ProjectGroupId(Guid.NewGuid()),
            GroupIdentifier = "GRP_88888888",
            TrustUkprn = new Domain.ValueObjects.Ukprn(11111111),
        });

        await dbContext.SaveChangesAsync();

        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateHandoverConversionProjectAsync(createHandoverConversionProjectCommand));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);

        var validationErrors = exception.Response;
        Assert.NotNull(validationErrors);

        Assert.Contains($"Trust UKPRN 12129999 is not the same as the group UKPRN for group GRP_88888888", validationErrors);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization))]
    public async Task CreateHandoverConversionProject_Async_UrnAlreadyExists_ShouldFailValidation(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();
        var testUser = await dbContext.Users.FirstAsync();

        var createHandoverConversionProjectCommand = GenerateCreateHandoverConversionCommand();
        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);

        var giasEstablishment = fixture
            .Customize(new GiasEstablishmentsCustomization()
            {
                LocalAuthority = localAuthority,
                Urn = new Domain.ValueObjects.Urn(createHandoverConversionProjectCommand.Urn!.Value)
            })
            .Create<GiasEstablishment>();
        await dbContext.GiasEstablishments.AddAsync(giasEstablishment);

        var project = fixture.Customize(new ProjectCustomization
        {
            RegionalDeliveryOfficerId = testUser.Id,
            CaseworkerId = testUser.Id,
            AssignedToId = testUser.Id,
            LocalAuthorityId = localAuthority.Id,
            Urn = giasEstablishment.Urn!
        })
            .Create<Domain.Entities.Project>();

        await dbContext.Projects.AddAsync(project);
        await dbContext.SaveChangesAsync();

        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateHandoverConversionProjectAsync(createHandoverConversionProjectCommand));

        Assert.Equal(HttpStatusCode.UnprocessableEntity, (HttpStatusCode)exception.StatusCode);

        var validationErrors = exception.Response;
        Assert.NotNull(validationErrors);

        Assert.Contains($"URN {giasEstablishment.Urn!.Value} already exists in active/inactive projects", validationErrors);
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

        Assert.Contains("Email must be @education.gov.uk", validationErrors);
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
        typeof(LocalAuthorityCustomization))]
    public async Task CreateHandoverMatConversionProject_Async_ShouldCreateHandoverConversionProjectOnly(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();
        var testUser = await dbContext.Users.OrderBy(u => u.CreatedAt).FirstOrDefaultAsync();
        Assert.NotNull(testUser);

        var createHandoverConversionProjectCommand = GenerateCreateHandoverConversionMatCommand();
        testUser.Email = createHandoverConversionProjectCommand.CreatedByEmail;

        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);

        dbContext.Users.Update(testUser);
        var giasEstablishment = fixture
            .Customize(new GiasEstablishmentsCustomization()
            {
                LocalAuthority = localAuthority,
                Urn = new Domain.ValueObjects.Urn(createHandoverConversionProjectCommand.Urn!.Value)
            })
            .Create<GiasEstablishment>();
        await dbContext.GiasEstablishments.AddAsync(giasEstablishment);
        await dbContext.SaveChangesAsync();

        var projectCountBefore = await dbContext.Projects.CountAsync();
        var userCountBefore = await dbContext.Users.CountAsync();
        var conversionTaskDataCountBefore = await dbContext.ConversionTasksData.CountAsync();

        var result = await projectsClient.CreateHandoverConversionMatProjectAsync(createHandoverConversionProjectCommand);

        Assert.NotNull(result);
        Assert.IsType<ProjectId>(result);
        Assert.Equal(projectCountBefore + 1, await dbContext.Projects.CountAsync());
        Assert.Equal(userCountBefore, await dbContext.Users.CountAsync());
        Assert.Equal(conversionTaskDataCountBefore + 1, await dbContext.ConversionTasksData.CountAsync());

        var createdProject = await dbContext.Projects.FirstOrDefaultAsync(p => p.Id == new Domain.ValueObjects.ProjectId(result.Value!.Value));
        Assert.NotNull(createdProject);
        Assert.Equal("Advisory board conditions", createdProject.AdvisoryBoardConditions);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization))]
    public async Task CreateHandoverMatConversionProject_Async_ShouldCreateProjectAndUser(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();

        var createHandoverConversionProjectCommand = GenerateCreateHandoverConversionMatCommand();

        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);

        var giasEstablishment = fixture
            .Customize(new GiasEstablishmentsCustomization()
            {
                LocalAuthority = localAuthority,
                Urn = new Domain.ValueObjects.Urn(createHandoverConversionProjectCommand.Urn!.Value)
            })
            .Create<GiasEstablishment>();
        await dbContext.GiasEstablishments.AddAsync(giasEstablishment);
        await dbContext.SaveChangesAsync();

        var projectCountBefore = await dbContext.Projects.CountAsync();
        var userCountBefore = await dbContext.Users.CountAsync();
        var conversionTaskDataCountBefore = await dbContext.ConversionTasksData.CountAsync();

        var result = await projectsClient.CreateHandoverConversionMatProjectAsync(createHandoverConversionProjectCommand);

        Assert.NotNull(result);
        Assert.IsType<ProjectId>(result);
        Assert.Equal(projectCountBefore + 1, await dbContext.Projects.CountAsync());
        Assert.Equal(userCountBefore + 1, await dbContext.Users.CountAsync());
        Assert.Equal(conversionTaskDataCountBefore + 1, await dbContext.ConversionTasksData.CountAsync());
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization))]
    public async Task CreateHandoverMatConversionProject_Async_UrnAlreadyExists_ShouldFailValidation(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();
        var testUser = await dbContext.Users.FirstAsync();

        var createHandoverConversionProjectCommand = GenerateCreateHandoverConversionMatCommand();
        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);

        var giasEstablishment = fixture
            .Customize(new GiasEstablishmentsCustomization()
            {
                LocalAuthority = localAuthority,
                Urn = new Domain.ValueObjects.Urn(createHandoverConversionProjectCommand.Urn!.Value)
            })
            .Create<GiasEstablishment>();
        await dbContext.GiasEstablishments.AddAsync(giasEstablishment);

        var project = fixture.Customize(new ProjectCustomization
        {
            RegionalDeliveryOfficerId = testUser.Id,
            CaseworkerId = testUser.Id,
            AssignedToId = testUser.Id,
            LocalAuthorityId = localAuthority.Id,
            Urn = giasEstablishment.Urn!
        })
            .Create<Domain.Entities.Project>();

        await dbContext.Projects.AddAsync(project);
        await dbContext.SaveChangesAsync();

        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateHandoverConversionMatProjectAsync(createHandoverConversionProjectCommand));

        Assert.Equal(HttpStatusCode.UnprocessableEntity, (HttpStatusCode)exception.StatusCode);

        var validationErrors = exception.Response;
        Assert.NotNull(validationErrors);

        Assert.Contains($"URN {giasEstablishment.Urn!.Value} already exists in active/inactive projects", validationErrors);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(CustomWebApplicationDbContextFactoryCustomization))]
    public async Task CreateHandoverMatConversionProject_WithEmptyBody_FailsValidation(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var createHandoverConversionProjectCommand = new CreateHandoverConversionMatProjectCommand();

        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateHandoverConversionMatProjectAsync(createHandoverConversionProjectCommand));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);

        var validationErrors = exception.Response;
        Assert.NotNull(validationErrors);

        Assert.Contains("The Urn field is required.", validationErrors);
        Assert.Contains("The PrepareId field is required.", validationErrors);
        Assert.Contains("The CreatedByEmail field is required.", validationErrors);
        Assert.Contains("The AdvisoryBoardDate field is required.", validationErrors);
        Assert.Contains("The CreatedByLastName field is required.", validationErrors);
        Assert.Contains("The CreatedByFirstName field is required.", validationErrors);
        Assert.Contains("The DirectiveAcademyOrder field is required.", validationErrors);
        Assert.Contains("The ProvisionalConversionDate field is required.", validationErrors);
        Assert.Contains("The NewTrustReferenceNumber field is required.", validationErrors);
        Assert.Contains("The NewTrustName field is required.", validationErrors);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization))]
    public async Task CreateHandoverTransferProject_Async_ShouldCreateHandoverTransferProjectOnly(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();
        var testUser = await dbContext.Users.OrderBy(u => u.CreatedAt).FirstOrDefaultAsync();
        Assert.NotNull(testUser);

        var createHandoverTransferProjectCommand = GenerateCreateHandoverTransferCommand();
        testUser.Email = createHandoverTransferProjectCommand.CreatedByEmail;

        Assert.NotNull(factory.WireMockServer);
        var incomingTrustDto = fixture.Customize(new TrustDtoCustomization() { Ukprn = createHandoverTransferProjectCommand.IncomingTrustUkprn.ToString() }).Create<TrustDto>();
        var outgoingTrustDto = fixture.Customize(new TrustDtoCustomization() { Ukprn = createHandoverTransferProjectCommand.OutgoingTrustUkprn.ToString() }).Create<TrustDto>();
        factory.WireMockServer.AddGetWithJsonResponse(
            string.Format(TrustClientEndpointConstants.GetTrustByUkprn2Async, createHandoverTransferProjectCommand.IncomingTrustUkprn),
            incomingTrustDto);
        factory.WireMockServer.AddGetWithJsonResponse(
            string.Format(TrustClientEndpointConstants.GetTrustByUkprn2Async, createHandoverTransferProjectCommand.OutgoingTrustUkprn),
            outgoingTrustDto);

        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);

        dbContext.Users.Update(testUser);
        var giasEstablishment = fixture
            .Customize(new GiasEstablishmentsCustomization()
            {
                LocalAuthority = localAuthority,
                Urn = new Domain.ValueObjects.Urn(createHandoverTransferProjectCommand.Urn!.Value)
            })
            .Create<GiasEstablishment>();
        await dbContext.GiasEstablishments.AddAsync(giasEstablishment);
        await dbContext.SaveChangesAsync();

        var projectCountBefore = await dbContext.Projects.CountAsync();
        var userCountBefore = await dbContext.Users.CountAsync();
        var transferTaskDataCountBefore = await dbContext.TransferTasksData.CountAsync();
        var projectGroupCountBefore = await dbContext.ProjectGroups.CountAsync();

        var result = await projectsClient.CreateHandoverTransferProjectAsync(createHandoverTransferProjectCommand);

        Assert.NotNull(result);
        Assert.IsType<ProjectId>(result);
        Assert.Equal(projectCountBefore + 1, await dbContext.Projects.CountAsync());
        Assert.Equal(userCountBefore, await dbContext.Users.CountAsync());
        Assert.Equal(transferTaskDataCountBefore + 1, await dbContext.TransferTasksData.CountAsync());
        Assert.Equal(projectGroupCountBefore, await dbContext.ProjectGroups.CountAsync());

        var createdProject = await dbContext.Projects.FirstOrDefaultAsync(p => p.Id == new Domain.ValueObjects.ProjectId(result.Value!.Value));
        Assert.NotNull(createdProject);
        Assert.Equal("Advisory board conditions", createdProject.AdvisoryBoardConditions);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization))]
    public async Task CreateHandoverTransferProject_Async_ShouldCreateProjectUserAndGroup(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();

        var createHandoverTransferProjectCommand = GenerateCreateHandoverTransferCommand();
        createHandoverTransferProjectCommand.GroupId = "GRP_99999999";

        Assert.NotNull(factory.WireMockServer);
        var incomingTrustDto = fixture.Customize(new TrustDtoCustomization() { Ukprn = createHandoverTransferProjectCommand.IncomingTrustUkprn.ToString() }).Create<TrustDto>();
        var outgoingTrustDto = fixture.Customize(new TrustDtoCustomization() { Ukprn = createHandoverTransferProjectCommand.OutgoingTrustUkprn.ToString() }).Create<TrustDto>();
        factory.WireMockServer.AddGetWithJsonResponse(
            string.Format(TrustClientEndpointConstants.GetTrustByUkprn2Async, createHandoverTransferProjectCommand.IncomingTrustUkprn),
            incomingTrustDto);
        factory.WireMockServer.AddGetWithJsonResponse(
            string.Format(TrustClientEndpointConstants.GetTrustByUkprn2Async, createHandoverTransferProjectCommand.OutgoingTrustUkprn),
            outgoingTrustDto);
        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);

        var giasEstablishment = fixture
            .Customize(new GiasEstablishmentsCustomization()
            {
                LocalAuthority = localAuthority,
                Urn = new Domain.ValueObjects.Urn(createHandoverTransferProjectCommand.Urn!.Value)
            })
            .Create<GiasEstablishment>();
        await dbContext.GiasEstablishments.AddAsync(giasEstablishment);
        await dbContext.SaveChangesAsync();

        var projectCountBefore = await dbContext.Projects.CountAsync();
        var userCountBefore = await dbContext.Users.CountAsync();
        var transferTaskDataCountBefore = await dbContext.TransferTasksData.CountAsync();
        var projectGroupCountBefore = await dbContext.ProjectGroups.CountAsync();

        var result = await projectsClient.CreateHandoverTransferProjectAsync(createHandoverTransferProjectCommand);

        Assert.NotNull(result);
        Assert.IsType<ProjectId>(result);
        Assert.Equal(projectCountBefore + 1, await dbContext.Projects.CountAsync());
        Assert.Equal(userCountBefore + 1, await dbContext.Users.CountAsync());
        Assert.Equal(transferTaskDataCountBefore + 1, await dbContext.TransferTasksData.CountAsync());
        Assert.Equal(projectGroupCountBefore + 1, await dbContext.ProjectGroups.CountAsync());
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization))]
    public async Task CreateHandoverTransferProject_Async_SameIncomingOutgoingUkprn_ShouldFailValidation(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var createHandoverTransferProjectCommand = GenerateCreateHandoverTransferCommand();
        createHandoverTransferProjectCommand.IncomingTrustUkprn = 12345678;
        createHandoverTransferProjectCommand.OutgoingTrustUkprn = 12345678;

        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateHandoverTransferProjectAsync(createHandoverTransferProjectCommand));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);

        var validationErrors = exception.Response;
        Assert.NotNull(validationErrors);

        Assert.Contains("Incoming trust UKPRN cannot be the same as the outgoing trust UKPRN", validationErrors);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(CustomWebApplicationDbContextFactoryCustomization))]
    public async Task CreateHandoverTransferProject_WithEmptyBody_FailsValidation(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var createHandoverTransferProjectCommand = new CreateHandoverTransferProjectCommand();

        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateHandoverTransferProjectAsync(createHandoverTransferProjectCommand));

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
        Assert.Contains("The OutgoingTrustUkprn field is required.", validationErrors);
        Assert.Contains("The InadequateOfsted field is required.", validationErrors);
        Assert.Contains("The FinancialSafeguardingGovernanceIssues field is required.", validationErrors);
        Assert.Contains("The OutgoingTrustToClose field is required.", validationErrors);
        Assert.Contains("The ProvisionalTransferDate field is required.", validationErrors);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
            typeof(DateOnlyCustomization),
            typeof(LocalAuthorityCustomization))]
    public async Task CreateHandoverTransferMatProject_Async_ShouldCreateHandoverTransferMatProjectOnly(
            CustomWebApplicationDbContextFactory<Program> factory,
            IProjectsClient projectsClient,
            IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();
        var testUser = await dbContext.Users.OrderBy(u => u.CreatedAt).FirstOrDefaultAsync();
        Assert.NotNull(testUser);

        var createHandoverTransferMatProjectCommand = GenerateCreateHandoverTransferMatCommand();
        testUser.Email = createHandoverTransferMatProjectCommand.CreatedByEmail;

        Assert.NotNull(factory.WireMockServer);
        var outgoingTrustDto = fixture.Customize(new TrustDtoCustomization() { Ukprn = createHandoverTransferMatProjectCommand.OutgoingTrustUkprn.ToString() }).Create<TrustDto>();
        factory.WireMockServer.AddGetWithJsonResponse(
            string.Format(TrustClientEndpointConstants.GetTrustByUkprn2Async, createHandoverTransferMatProjectCommand.OutgoingTrustUkprn),
            outgoingTrustDto);

        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);

        dbContext.Users.Update(testUser);
        var giasEstablishment = fixture
            .Customize(new GiasEstablishmentsCustomization()
            {
                LocalAuthority = localAuthority,
                Urn = new Domain.ValueObjects.Urn(createHandoverTransferMatProjectCommand.Urn!.Value)
            })
            .Create<GiasEstablishment>();
        await dbContext.GiasEstablishments.AddAsync(giasEstablishment);
        await dbContext.SaveChangesAsync();

        var projectCountBefore = await dbContext.Projects.CountAsync();
        var userCountBefore = await dbContext.Users.CountAsync();
        var transferTaskDataCountBefore = await dbContext.TransferTasksData.CountAsync();

        var result = await projectsClient.CreateHandoverTransferMatProjectAsync(createHandoverTransferMatProjectCommand);

        Assert.NotNull(result);
        Assert.IsType<ProjectId>(result);
        Assert.Equal(projectCountBefore + 1, await dbContext.Projects.CountAsync());
        Assert.Equal(userCountBefore, await dbContext.Users.CountAsync());
        Assert.Equal(transferTaskDataCountBefore + 1, await dbContext.TransferTasksData.CountAsync());

        var createdProject = await dbContext.Projects.FirstOrDefaultAsync(p => p.Id == new Domain.ValueObjects.ProjectId(result.Value!.Value));
        Assert.NotNull(createdProject);
        Assert.Equal("Advisory board conditions", createdProject.AdvisoryBoardConditions);
        Assert.Equal("TR98765", createdProject.NewTrustReferenceNumber);
        Assert.Equal("New Trust Ltd", createdProject.NewTrustName);
        Assert.True(createdProject.FormAMat);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization))]
    public async Task CreateHandoverTransferMatProject_Async_ShouldCreateProjectAndUser(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();

        var createHandoverTransferMatProjectCommand = GenerateCreateHandoverTransferMatCommand();

        Assert.NotNull(factory.WireMockServer);
        var outgoingTrustDto = fixture.Customize(new TrustDtoCustomization() { Ukprn = createHandoverTransferMatProjectCommand.OutgoingTrustUkprn.ToString() }).Create<TrustDto>();
        factory.WireMockServer.AddGetWithJsonResponse(
            string.Format(TrustClientEndpointConstants.GetTrustByUkprn2Async, createHandoverTransferMatProjectCommand.OutgoingTrustUkprn),
            outgoingTrustDto);

        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);

        var giasEstablishment = fixture
            .Customize(new GiasEstablishmentsCustomization()
            {
                LocalAuthority = localAuthority,
                Urn = new Domain.ValueObjects.Urn(createHandoverTransferMatProjectCommand.Urn!.Value)
            })
            .Create<GiasEstablishment>();
        await dbContext.GiasEstablishments.AddAsync(giasEstablishment);
        await dbContext.SaveChangesAsync();

        var projectCountBefore = await dbContext.Projects.CountAsync();
        var userCountBefore = await dbContext.Users.CountAsync();
        var transferTaskDataCountBefore = await dbContext.TransferTasksData.CountAsync();

        var result = await projectsClient.CreateHandoverTransferMatProjectAsync(createHandoverTransferMatProjectCommand);

        Assert.NotNull(result);
        Assert.IsType<ProjectId>(result);
        Assert.Equal(projectCountBefore + 1, await dbContext.Projects.CountAsync());
        Assert.Equal(userCountBefore + 1, await dbContext.Users.CountAsync());
        Assert.Equal(transferTaskDataCountBefore + 1, await dbContext.TransferTasksData.CountAsync());
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(LocalAuthorityCustomization))]
    public async Task CreateHandoverTransferMatProject_Async_UrnAlreadyExists_ShouldFailValidation(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];
        var dbContext = factory.GetDbContext<CompleteContext>();
        var testUser = await dbContext.Users.FirstAsync();

        var createHandoverTransferMatProjectCommand = GenerateCreateHandoverTransferMatCommand();
        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);

        var giasEstablishment = fixture
            .Customize(new GiasEstablishmentsCustomization()
            {
                LocalAuthority = localAuthority,
                Urn = new Domain.ValueObjects.Urn(createHandoverTransferMatProjectCommand.Urn!.Value)
            })
            .Create<GiasEstablishment>();
        await dbContext.GiasEstablishments.AddAsync(giasEstablishment);

        var project = fixture.Customize(new ProjectCustomization
        {
            RegionalDeliveryOfficerId = testUser.Id,
            CaseworkerId = testUser.Id,
            AssignedToId = testUser.Id,
            LocalAuthorityId = localAuthority.Id,
            Urn = giasEstablishment.Urn!
        })
            .Create<Domain.Entities.Project>();

        await dbContext.Projects.AddAsync(project);
        await dbContext.SaveChangesAsync();

        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateHandoverTransferMatProjectAsync(createHandoverTransferMatProjectCommand));

        Assert.Equal(HttpStatusCode.UnprocessableEntity, (HttpStatusCode)exception.StatusCode);

        var validationErrors = exception.Response;
        Assert.NotNull(validationErrors);

        Assert.Contains($"URN {giasEstablishment.Urn!.Value} already exists in active/inactive projects", validationErrors);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(CustomWebApplicationDbContextFactoryCustomization))]
    public async Task CreateHandoverTransferMatProject_WithEmptyBody_FailsValidation(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.WriteRole), new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        var createHandoverTransferMatProjectCommand = new CreateHandoverTransferMatProjectCommand();

        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateHandoverTransferMatProjectAsync(createHandoverTransferMatProjectCommand));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);

        var validationErrors = exception.Response;
        Assert.NotNull(validationErrors);

        Assert.Contains("The Urn field is required.", validationErrors);
        Assert.Contains("The PrepareId field is required.", validationErrors);
        Assert.Contains("The CreatedByEmail field is required.", validationErrors);
        Assert.Contains("The AdvisoryBoardDate field is required.", validationErrors);
        Assert.Contains("The CreatedByLastName field is required.", validationErrors);
        Assert.Contains("The CreatedByFirstName field is required.", validationErrors);
        Assert.Contains("The OutgoingTrustUkprn field is required.", validationErrors);
        Assert.Contains("The InadequateOfsted field is required.", validationErrors);
        Assert.Contains("The FinancialSafeguardingGovernanceIssues field is required.", validationErrors);
        Assert.Contains("The OutgoingTrustToClose field is required.", validationErrors);
        Assert.Contains("The ProvisionalTransferDate field is required.", validationErrors);
        Assert.Contains("The NewTrustReferenceNumber field is required.", validationErrors);
        Assert.Contains("The NewTrustName field is required.", validationErrors);
    }

    private static CreateHandoverConversionProjectCommand GenerateCreateHandoverConversionCommand() => new()
    {
        Urn = 121999,
        IncomingTrustUkprn = 12129999,
        AdvisoryBoardDate = DateTime.Parse("2025-05-02", CultureInfo.InvariantCulture),
        ProvisionalConversionDate = DateTime.Parse("2025-05-01", CultureInfo.InvariantCulture),
        CreatedByEmail = "test@education.gov.uk",
        CreatedByFirstName = "Test",
        CreatedByLastName = "User",
        PrepareId = 123,
        DirectiveAcademyOrder = true,
        AdvisoryBoardConditions = "Advisory board conditions"
    };

    private static CreateHandoverConversionMatProjectCommand GenerateCreateHandoverConversionMatCommand() => new()
    {
        Urn = 121999,
        AdvisoryBoardDate = DateTime.Parse("2025-05-02", CultureInfo.InvariantCulture),
        ProvisionalConversionDate = DateTime.Parse("2025-05-01", CultureInfo.InvariantCulture),
        CreatedByEmail = "test@education.gov.uk",
        CreatedByFirstName = "Test",
        CreatedByLastName = "User",
        PrepareId = 123,
        DirectiveAcademyOrder = true,
        AdvisoryBoardConditions = "Advisory board conditions",
        NewTrustReferenceNumber = "TR98765",
        NewTrustName = "New Trust Ltd"
    };

    private static CreateHandoverTransferProjectCommand GenerateCreateHandoverTransferCommand() => new()
    {
        Urn = 121999,
        IncomingTrustUkprn = 12129999,
        OutgoingTrustUkprn = 12120000,
        AdvisoryBoardDate = DateTime.Parse("2025-05-02", CultureInfo.InvariantCulture),
        ProvisionalTransferDate = DateTime.Parse("2025-05-01", CultureInfo.InvariantCulture),
        CreatedByEmail = "test@education.gov.uk",
        CreatedByFirstName = "Test",
        CreatedByLastName = "User",
        PrepareId = 123,
        AdvisoryBoardConditions = "Advisory board conditions",
        FinancialSafeguardingGovernanceIssues = false,
        InadequateOfsted = false,
        OutgoingTrustToClose = false
    };

    private static CreateHandoverTransferMatProjectCommand GenerateCreateHandoverTransferMatCommand() => new()
    {
        Urn = 121999,
        OutgoingTrustUkprn = 12120000,
        AdvisoryBoardDate = DateTime.Parse("2025-05-02", CultureInfo.InvariantCulture),
        ProvisionalTransferDate = DateTime.Parse("2025-05-01", CultureInfo.InvariantCulture),
        CreatedByEmail = "test@education.gov.uk",
        CreatedByFirstName = "Test",
        CreatedByLastName = "User",
        PrepareId = 123,
        AdvisoryBoardConditions = "Advisory board conditions",
        NewTrustReferenceNumber = "TR98765",
        NewTrustName = "New Trust Ltd",
        FinancialSafeguardingGovernanceIssues = false,
        InadequateOfsted = false,
        OutgoingTrustToClose = false
    };
}
