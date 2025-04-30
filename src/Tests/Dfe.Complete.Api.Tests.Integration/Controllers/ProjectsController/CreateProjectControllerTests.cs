using System.Net;
using System.Security.Claims;
using AutoFixture;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Customizations.Commands;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using DfE.CoreLibs.Testing.Mocks.WebApplicationFactory;
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
    public async Task CreateConversionProject_Async_ShouldCreateConversionProject(
        CustomWebApplicationDbContextFactory<Program> factory,
        CreateConversionProjectCommand createConversionProjectCommand,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, WriteRole), new Claim(ClaimTypes.Role, ReadRole)];

        var dbContext = factory.GetDbContext<CompleteContext>();

        var testUser = await dbContext.Users.FirstOrDefaultAsync();
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
        factory.TestClaims = [new Claim(ClaimTypes.Role, WriteRole), new Claim(ClaimTypes.Role, ReadRole)];

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
        factory.TestClaims = [new Claim(ClaimTypes.Role, WriteRole), new Claim(ClaimTypes.Role, ReadRole)];

        var dbContext = factory.GetDbContext<CompleteContext>();

        var testUser = await dbContext.Users.FirstOrDefaultAsync();
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
        factory.TestClaims = [new Claim(ClaimTypes.Role, WriteRole), new Claim(ClaimTypes.Role, ReadRole)];

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
        factory.TestClaims = [new Claim(ClaimTypes.Role, WriteRole), new Claim(ClaimTypes.Role, ReadRole)];

        var dbContext = factory.GetDbContext<CompleteContext>();

        var testUser = await dbContext.Users.FirstOrDefaultAsync();
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
        factory.TestClaims = [new Claim(ClaimTypes.Role, WriteRole), new Claim(ClaimTypes.Role, ReadRole)];

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
        factory.TestClaims = [new Claim(ClaimTypes.Role, WriteRole), new Claim(ClaimTypes.Role, ReadRole)];

        var dbContext = factory.GetDbContext<CompleteContext>();

        var testUser = await dbContext.Users.FirstOrDefaultAsync();
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
        factory.TestClaims = [new Claim(ClaimTypes.Role, WriteRole), new Claim(ClaimTypes.Role, ReadRole)];

        createMatTransferProjectCommand.Urn = null;

        var exception = await Assert.ThrowsAsync<CompleteApiException>(async () =>
            await projectsClient.CreateMatTransferProjectAsync(createMatTransferProjectCommand));

        Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)exception.StatusCode);
    }
}