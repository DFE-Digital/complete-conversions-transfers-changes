using System.Net;
using System.Security.Claims;
using AutoFixture;
using AutoMapper.Configuration.Annotations;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Customizations.Commands;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using DfE.CoreLibs.Testing.Mocks.WebApplicationFactory;
using DfE.CoreLibs.Testing.Mocks.WireMock;
using Microsoft.EntityFrameworkCore;
using Moq;
using Project = Dfe.Complete.Domain.Entities.Project;
using Ukprn = Dfe.Complete.Domain.ValueObjects.Ukprn;

namespace Dfe.Complete.Api.Tests.Integration.Controllers;

public class ProjectsControllerTests
{
    private const string ReadRole = "API.Read";
    private const string WriteRole = "API.Write";
    private const string DeleteRole = "API.Delete";
    private const string UpdateRole = "API.Update";

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

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(LocalAuthorityCustomization),
        typeof(GiasEstablishmentsCustomization))]
    public async Task CountAllProjects_Async_ShouldReturnCorrectNumber(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ReadRole)];

        var dbContext = factory.GetDbContext<CompleteContext>();

        var testUser = await dbContext.Users.FirstAsync();

        var giasEstablishment = fixture.Create<GiasEstablishment>();
        await dbContext.GiasEstablishments.AddAsync(giasEstablishment);

        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);

        var projects = fixture.Customize(new ProjectCustomization
        {
            RegionalDeliveryOfficerId = testUser.Id,
            CaseworkerId = testUser.Id,
            AssignedToId = testUser.Id,
            LocalAuthorityId = localAuthority.Id,
            Urn = giasEstablishment.Urn
        })
            .CreateMany<Project>(50).ToList();


        projects.ForEach(x => x.LocalAuthorityId = localAuthority.Id);

        await dbContext.Projects.AddRangeAsync(projects);
        await dbContext.SaveChangesAsync();

        var result = await projectsClient.CountAllProjectsAsync(null, null);

        Assert.Equal(50, result);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(GiasEstablishmentsCustomization))]
    public async Task ListAllProjects_Async_ShouldReturnList(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ReadRole)];

        // Arrange
        var dbContext = factory.GetDbContext<CompleteContext>();
        var testUser = await dbContext.Users.FirstAsync();
        var urn = 100000;

        var establishments = fixture.Customize(new GiasEstablishmentsCustomization()).CreateMany<GiasEstablishment>(50).ToList();
        // .Select(establishment =>
        // {
        //     establishment.Urn = new Domain.ValueObjects.Urn(urn++);
        //     return establishment;
        // })
        await dbContext.GiasEstablishments.AddRangeAsync(establishments);
        var projects = establishments.Select(establishment =>
        {
            var project = fixture.Customize(new ProjectCustomization
            {
                RegionalDeliveryOfficerId = testUser.Id,
                CaseworkerId = testUser.Id,
                AssignedToId = testUser.Id
            })
                .Create<Project>();
            project.Urn = establishment.Urn ?? project.Urn;
            return project;
        }).ToList();

        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);
        projects.ForEach(x => x.LocalAuthorityId = localAuthority.Id);

        await dbContext.Projects.AddRangeAsync(projects);
        await dbContext.SaveChangesAsync();

        // Act
        var results = await projectsClient.ListAllProjectsAsync(
            null, null, 0, 50);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(50, results.Count);
        foreach (var result in results)
        {
            var project = projects.Find(p => p.Id.Value == result.ProjectId?.Value);
            var establishment = establishments.Find(e => e.Urn?.Value == result.Urn?.Value);

            Assert.NotNull(result.Urn);
            Assert.Equal(project?.Urn.Value, result.Urn.Value);
            Assert.Equal(establishment?.Urn?.Value, result.Urn.Value);

            Assert.NotNull(result.EstablishmentName);
            Assert.Equal(establishment?.Name, result.EstablishmentName);

            Assert.NotNull(result.ProjectId);
            Assert.Equal(project?.Id.Value, result.ProjectId.Value);

            Assert.NotNull(result.ConversionOrTransferDate);
            Assert.Equal(project?.SignificantDate, new DateOnly(result.ConversionOrTransferDate.Value.Year,
                result.ConversionOrTransferDate.Value.Month, result.ConversionOrTransferDate.Value.Day));

            Assert.NotNull(result.State);
            Assert.Equal(project?.State.ToString(), result.State.ToString());

            Assert.NotNull(result.ProjectType);
            Assert.Equal(project?.Type?.ToString(), result.ProjectType.Value.ToString());

            Assert.Equal(project?.FormAMat, result.IsFormAMAT);

            Assert.NotNull(result.AssignedToFullName);
            Assert.Equal($"{project?.AssignedTo?.FirstName} {project?.AssignedTo?.LastName}",
                result.AssignedToFullName);
        }
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(GiasEstablishmentsCustomization))]
    public async Task ListAllProjectsCompletedState_Async_ShouldReturnList(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ReadRole)];

        // Arrange
        var dbContext = factory.GetDbContext<CompleteContext>();
        var testUser = await dbContext.Users.FirstAsync();
        var establishments = fixture.CreateMany<GiasEstablishment>(50).ToList();
        await dbContext.GiasEstablishments.AddRangeAsync(establishments);
        var projects = establishments.Select(establishment =>
        {
            var project = fixture.Customize(new ProjectCustomization
            {
                RegionalDeliveryOfficerId = testUser.Id,
                CaseworkerId = testUser.Id,
                AssignedToId = testUser.Id
            })
                .Create<Project>();
            project.Urn = establishment.Urn ?? project.Urn;
            return project;
        }).ToList();

        projects.ForEach(x => x.LocalAuthorityId = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid())?.Id);

        await dbContext.Projects.AddRangeAsync(projects);
        await dbContext.SaveChangesAsync();

        // Act
        var results = await projectsClient.ListAllProjectsAsync(
            ProjectState.Completed, null, 0, 50);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(17, results.Count);
        foreach (var result in results)
        {
            var project = projects.Find(p => p.Id.Value == result.ProjectId?.Value);
            var establishment = establishments.Find(e => e.Urn?.Value == result.Urn?.Value);

            Assert.NotNull(result.EstablishmentName);
            Assert.Equal(establishment?.Name, result.EstablishmentName);

            Assert.NotNull(result.ProjectId);
            Assert.Equal(project?.Id.Value, result.ProjectId.Value);

            Assert.NotNull(result.Urn);
            Assert.Equal(project?.Urn.Value, result.Urn.Value);
            Assert.Equal(establishment?.Urn?.Value, result.Urn.Value);

            Assert.NotNull(result.ConversionOrTransferDate);
            Assert.Equal(project?.SignificantDate, new DateOnly(result.ConversionOrTransferDate.Value.Year,
                result.ConversionOrTransferDate.Value.Month, result.ConversionOrTransferDate.Value.Day));

            Assert.NotNull(result.State);
            Assert.Equal(project?.State.ToString(), result.State.ToString());

            Assert.NotNull(result.ProjectType);
            Assert.Equal(project?.Type?.ToString(), result.ProjectType.Value.ToString());

            Assert.Equal(project?.FormAMat, result.IsFormAMAT);

            Assert.NotNull(result.AssignedToFullName);
            Assert.Equal($"{project?.AssignedTo?.FirstName} {project?.AssignedTo?.LastName}",
                result.AssignedToFullName);
        }
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(GiasEstablishmentsCustomization))]
    public async Task RemoveProjectsShouldRemoveConversionProjectAndChildren(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = new[] { ReadRole, WriteRole, DeleteRole, UpdateRole }
            .Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        var dbContext = factory.GetDbContext<CompleteContext>();

        var testUser = await dbContext.Users.FirstAsync();
        var establishment = fixture.Create<GiasEstablishment>();

        var taskData = fixture.Create<ConversionTasksData>();
        dbContext.ConversionTasksData.Add(taskData);

        dbContext.GiasEstablishments.Add(establishment);
        var project = fixture.Customize(new ProjectCustomization
        {
            RegionalDeliveryOfficerId = testUser.Id,
            CaseworkerId = testUser.Id,
            AssignedToId = testUser.Id,
            TasksDataId = taskData.Id,
            TasksDataType = Domain.Enums.TaskType.Conversion,
        })
            .Create<Project>();
        project.Urn = establishment.Urn ?? project.Urn;

        var localAuthority = await dbContext.LocalAuthorities.FirstOrDefaultAsync();
        Assert.NotNull(localAuthority);
        project.LocalAuthorityId = localAuthority.Id;

        dbContext.ConversionTasksData.Add(taskData);

        var note = fixture.Create<Domain.Entities.Note>();
        note.Id = new Domain.ValueObjects.NoteId(Guid.NewGuid());
        project.Notes.Add(note);
        note.UserId = testUser.Id;

        dbContext.Projects.Add(project);

        await dbContext.SaveChangesAsync();

        var existingProjectbefore = await dbContext.Projects.SingleAsync(x => x.Urn == project.Urn);

        Assert.NotNull(existingProjectbefore);

        var existingNoteBefore = await dbContext.Notes.SingleAsync(x => x.ProjectId == project.Id);

        Assert.NotNull(existingNoteBefore);

        await projectsClient.RemoveProjectAsync(new Urn { Value = project.Urn.Value });

        var existingProject = await dbContext.Projects.SingleOrDefaultAsync(x => x.Urn == project.Urn);

        Assert.Null(existingProject);

        var existingNote = await dbContext.Notes.SingleOrDefaultAsync(x => x.ProjectId == project.Id);

        Assert.Null(existingNote);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(GiasEstablishmentsCustomization))]
    public async Task RemoveProjectsShouldRemoveTransferProjectAndChildren(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = new[] { ReadRole, WriteRole, DeleteRole, UpdateRole }
            .Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        var dbContext = factory.GetDbContext<CompleteContext>();

        var testUser = await dbContext.Users.FirstAsync();
        var establishment = fixture.Create<GiasEstablishment>();

        var taskData = fixture.Create<TransferTasksData>();
        dbContext.TransferTasksData.Add(taskData);

        dbContext.GiasEstablishments.Add(establishment);
        var project = fixture.Customize(new ProjectCustomization
        {
            RegionalDeliveryOfficerId = testUser.Id,
            CaseworkerId = testUser.Id,
            AssignedToId = testUser.Id,
            TasksDataId = taskData.Id,
            TasksDataType = Domain.Enums.TaskType.Transfer,
        })
            .Create<Project>();
        project.Urn = establishment.Urn ?? project.Urn;

        var localAuthority = await dbContext.LocalAuthorities.FirstOrDefaultAsync();
        Assert.NotNull(localAuthority);
        project.LocalAuthorityId = localAuthority.Id;

        dbContext.TransferTasksData.Add(taskData);

        var note = fixture.Create<Domain.Entities.Note>();
        note.Id = new Domain.ValueObjects.NoteId(Guid.NewGuid());
        project.Notes.Add(note);
        note.UserId = testUser.Id;

        dbContext.Projects.Add(project);

        await dbContext.SaveChangesAsync();

        var existingProjectbefore = await dbContext.Projects.SingleAsync(x => x.Urn == project.Urn);

        Assert.NotNull(existingProjectbefore);

        var existingNoteBefore = await dbContext.Notes.SingleAsync(x => x.ProjectId == project.Id);

        Assert.NotNull(existingNoteBefore);

        await projectsClient.RemoveProjectAsync(new Urn { Value = project.Urn.Value });

        var existingProject = await dbContext.Projects.SingleOrDefaultAsync(x => x.Urn == project.Urn);

        Assert.Null(existingProject);

        var existingNote = await dbContext.Notes.SingleOrDefaultAsync(x => x.ProjectId == project.Id);

        Assert.Null(existingNote);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(GiasEstablishmentsCustomization))]
    public async Task RemoveProjectsShouldContinueIfNoProjectIsFound(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        factory.TestClaims = new[] { ReadRole, WriteRole, DeleteRole, UpdateRole }
            .Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        var dbContext = factory.GetDbContext<CompleteContext>();

        var existingProjectsBefore = await dbContext.Projects.CountAsync();
        var existingNotesBefore = await dbContext.Notes.CountAsync();

        await projectsClient.RemoveProjectAsync(new Urn { Value = 98765432 });

        var existingProjectsAfter = await dbContext.Projects.CountAsync();
        var existingNotesAfter = await dbContext.Notes.CountAsync();

        Assert.Equal(existingProjectsBefore, existingProjectsAfter);
        Assert.Equal(existingNotesBefore, existingNotesAfter);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(GiasEstablishmentsCustomization))]
    public async Task GetProjectByUrn_should_return_the_correct_project(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = new[] { ReadRole, WriteRole, DeleteRole, UpdateRole }
            .Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        var dbContext = factory.GetDbContext<CompleteContext>();
        var expected = fixture.Customize(new ProjectCustomization())
            .Create<Project>();

        var localAuthority = await dbContext.LocalAuthorities.FirstAsync();
        expected.LocalAuthorityId = localAuthority.Id;

        dbContext.Projects.Add(expected);

        await dbContext.SaveChangesAsync();

        var actual = await projectsClient.GetProjectAsync(expected.Urn.Value);

        Assert.Equivalent(expected.Id, actual.Id);
        Assert.Equivalent(expected.Urn, actual.Urn);
        Assert.Equivalent(expected.CreatedAt, actual.CreatedAt);
        Assert.Equivalent(expected.UpdatedAt, actual.UpdatedAt);
        Assert.Equivalent(expected.IncomingTrustUkprn, actual.IncomingTrustUkprn);
        Assert.Equivalent(expected.RegionalDeliveryOfficerId, actual.RegionalDeliveryOfficerId);
        Assert.Equivalent(expected.CaseworkerId, actual.CaseworkerId);
        Assert.Equivalent(expected.AssignedAt, actual.AssignedAt);
        Assert.Equivalent(expected.AdvisoryBoardDate, DateOnly.FromDateTime(actual.AdvisoryBoardDate!.Value));
        Assert.Equivalent(expected.AdvisoryBoardConditions, actual.AdvisoryBoardConditions);
        Assert.Equivalent(expected.EstablishmentSharepointLink, actual.EstablishmentSharepointLink);
        Assert.Equivalent(expected.CompletedAt, actual.CompletedAt);
        Assert.Equivalent(expected.IncomingTrustSharepointLink, actual.IncomingTrustSharepointLink);
        Assert.Equivalent(expected.Type.ToString(), actual.Type.ToString());
        Assert.Equivalent(expected.AssignedToId, actual.AssignedToId);
        Assert.Equivalent(expected.SignificantDate, DateOnly.FromDateTime(actual.SignificantDate!.Value));
        Assert.Equivalent(expected.SignificantDateProvisional, actual.SignificantDateProvisional);
        Assert.Equivalent(expected.DirectiveAcademyOrder, actual.DirectiveAcademyOrder);
        Assert.Equivalent(expected.Region.ToString(), actual.Region.ToString());
        Assert.Equivalent(expected.AcademyUrn, actual.AcademyUrn);
        Assert.Equivalent(expected.TasksDataId, actual.TasksDataId);
        Assert.Equivalent(expected.TasksDataType.ToString(), actual.TasksDataType.ToString());
        Assert.Equivalent(expected.OutgoingTrustUkprn, actual.OutgoingTrustUkprn);
        Assert.Equivalent(expected.Team.ToString(), actual.Team.ToString());
        Assert.Equivalent(expected.TwoRequiresImprovement, actual.TwoRequiresImprovement);
        Assert.Equivalent(expected.OutgoingTrustSharepointLink, actual.OutgoingTrustSharepointLink);
        Assert.Equivalent(expected.AllConditionsMet, actual.AllConditionsMet);
        Assert.Equivalent(expected.MainContactId, actual.MainContactId);
        Assert.Equivalent(expected.EstablishmentMainContactId, actual.EstablishmentMainContactId);
        Assert.Equivalent(expected.IncomingTrustMainContactId, actual.IncomingTrustMainContactId);
        Assert.Equivalent(expected.OutgoingTrustMainContactId, actual.OutgoingTrustMainContactId);
        Assert.Equivalent(expected.NewTrustReferenceNumber, actual.NewTrustReferenceNumber);
        Assert.Equivalent(expected.NewTrustName, actual.NewTrustName);
        Assert.Equivalent(expected.State.ToString(), actual.State.ToString());
        Assert.Equivalent(expected.PrepareId, actual.PrepareId);
        Assert.Equivalent(expected.LocalAuthorityMainContactId, actual.LocalAuthorityMainContactId);
        Assert.Equivalent(expected.GroupId, actual.GroupId);
        Assert.Equivalent(expected.AssignedTo, actual.AssignedTo);
        Assert.Equivalent(expected.Caseworker, actual.Caseworker);
        Assert.Equivalent(expected.RegionalDeliveryOfficer, actual.RegionalDeliveryOfficer);
        Assert.Equivalent(expected.Contacts, actual.Contacts);
        Assert.Equivalent(expected.Notes, actual.Notes);
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(GiasEstablishmentsCustomization))]
    public async Task ListAllProjectsByTrust_Async_ShouldReturnListOfNonFormAMatProjects_WhenTrustExists(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ReadRole)];

        // Arrange
        var dbContext = factory.GetDbContext<CompleteContext>();
        var testUser = await dbContext.Users.FirstAsync();

        // Add projects for 
        // Stub TrustV4Client GetTrustByUkprn2Async if not form a mat
        // Get NewTrustReferenceNumber if form a mat

        var establishments = fixture.Customize(new GiasEstablishmentsCustomization()).CreateMany<GiasEstablishment>(50).ToList();

        await dbContext.GiasEstablishments.AddRangeAsync(establishments);
        var projects = establishments.Select(establishment =>
        {
            var project = fixture.Customize(new ProjectCustomization
            {
                RegionalDeliveryOfficerId = testUser.Id,
                CaseworkerId = testUser.Id,
                AssignedToId = testUser.Id
            })
                .Create<Project>();
            project.Urn = establishment.Urn ?? project.Urn;
            project.IncomingTrustUkprn = new Ukprn(12345678);
            project.NewTrustReferenceNumber = null;
            project.NewTrustName = null;
            project.State = Domain.Enums.ProjectState.Active;
            return project;
        }).ToList();

        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);
        projects.ForEach(x => x.LocalAuthorityId = localAuthority.Id);

        var trustDto = fixture.Customize(new TrustDtoCustomization() { Ukprn = "12345678" }).Create<TrustDto>();

        Assert.NotNull(factory.WireMockServer);

        factory.WireMockServer.AddGetWithJsonResponse($"/v4/trust/{trustDto.Ukprn}", trustDto);

        await dbContext.Projects.AddRangeAsync(projects);
        await dbContext.SaveChangesAsync();

        // Act
        var results = await projectsClient.ListAllProjectsInTrustAsync(
            trustDto.Ukprn, false, 0, 50);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(50, results.Count);
        foreach (var result in results)
        {
            var project = projects.Find(p => p.Id.Value == result.ProjectId?.Value);
            var establishment = establishments.Find(e => e.Urn?.Value == result.Urn?.Value);

            Assert.NotNull(result.Urn);
            Assert.Equal(project?.Urn.Value, result.Urn.Value);
            Assert.Equal(establishment?.Urn?.Value, result.Urn.Value);

            Assert.NotNull(result.EstablishmentName);
            Assert.Equal(establishment?.Name, result.EstablishmentName);

            Assert.NotNull(result.ProjectId);
            Assert.Equal(project?.Id.Value, result.ProjectId.Value);

            Assert.NotNull(result.ConversionOrTransferDate);
            Assert.Equal(project?.SignificantDate, new DateOnly(result.ConversionOrTransferDate.Value.Year,
                result.ConversionOrTransferDate.Value.Month, result.ConversionOrTransferDate.Value.Day));

            Assert.NotNull(result.State);
            Assert.Equal(project?.State.ToString(), result.State.ToString());

            Assert.NotNull(result.ProjectType);
            Assert.Equal(project?.Type?.ToString(), result.ProjectType.Value.ToString());

            Assert.Equal(project?.FormAMat, result.IsFormAMAT);

            Assert.NotNull(result.AssignedToFullName);
            Assert.Equal($"{project?.AssignedTo?.FirstName} {project?.AssignedTo?.LastName}",
                result.AssignedToFullName);
        }
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(GiasEstablishmentsCustomization))]
    public async Task ListAllProjectsByTrust_Async_ShouldReturnListOfFormAMatProjects_WhenTrustExists(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ReadRole)];

        // Arrange
        var dbContext = factory.GetDbContext<CompleteContext>();
        var testUser = await dbContext.Users.FirstAsync();

        // Add projects for 
        // Stub TrustV4Client GetTrustByUkprn2Async if not form a mat
        // Get NewTrustReferenceNumber if form a mat

        var establishments = fixture.Customize(new GiasEstablishmentsCustomization()).CreateMany<GiasEstablishment>(50).ToList();

        await dbContext.GiasEstablishments.AddRangeAsync(establishments);
        var projects = establishments.Select(establishment =>
        {
            var project = fixture.Customize(new ProjectCustomization
            {
                RegionalDeliveryOfficerId = testUser.Id,
                CaseworkerId = testUser.Id,
                AssignedToId = testUser.Id
            })
                .Create<Project>();
            project.Urn = establishment.Urn ?? project.Urn;
            project.IncomingTrustUkprn = null;
            project.NewTrustReferenceNumber = "TR123456";
            project.NewTrustName = "New Trust";
            project.State = Domain.Enums.ProjectState.Active;
            return project;
        }).ToList();

        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);
        projects.ForEach(x => x.LocalAuthorityId = localAuthority.Id);

        await dbContext.Projects.AddRangeAsync(projects);
        await dbContext.SaveChangesAsync();

        // Act
        var results = await projectsClient.ListAllProjectsInTrustAsync(
            "TR123456", true, 0, 50);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(50, results.Count);
        foreach (var result in results)
        {
            var project = projects.Find(p => p.Id.Value == result.ProjectId?.Value);
            var establishment = establishments.Find(e => e.Urn?.Value == result.Urn?.Value);

            Assert.NotNull(result.Urn);
            Assert.Equal(project?.Urn.Value, result.Urn.Value);
            Assert.Equal(establishment?.Urn?.Value, result.Urn.Value);

            Assert.NotNull(result.EstablishmentName);
            Assert.Equal(establishment?.Name, result.EstablishmentName);

            Assert.NotNull(result.ProjectId);
            Assert.Equal(project?.Id.Value, result.ProjectId.Value);

            Assert.NotNull(result.ConversionOrTransferDate);
            Assert.Equal(project?.SignificantDate, new DateOnly(result.ConversionOrTransferDate.Value.Year,
                result.ConversionOrTransferDate.Value.Month, result.ConversionOrTransferDate.Value.Day));

            Assert.NotNull(result.State);
            Assert.Equal(project?.State.ToString(), result.State.ToString());

            Assert.NotNull(result.ProjectType);
            Assert.Equal(project?.Type?.ToString(), result.ProjectType.Value.ToString());

            Assert.Equal(project?.FormAMat, result.IsFormAMAT);

            Assert.NotNull(result.AssignedToFullName);
            Assert.Equal($"{project?.AssignedTo?.FirstName} {project?.AssignedTo?.LastName}",
                result.AssignedToFullName);
        }
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(GiasEstablishmentsCustomization))]
    public async Task ListAllProjectsForLocalAuthority_Async_ShouldReturnList(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ReadRole)];

        // Arrange
        var dbContext = factory.GetDbContext<CompleteContext>();
        var testUser = await dbContext.Users.FirstAsync();
        var localAuthorityCode = "123456";

        var allEstablishments = fixture
            .Customize(new GiasEstablishmentsCustomization())
            .CreateMany<GiasEstablishment>(50)
            .ToList();

        var matchingEstablishments = allEstablishments.Take(20).ToList();
        var otherEstablishments = allEstablishments.Skip(20).ToList();

        foreach (var establishment in matchingEstablishments)
            establishment.LocalAuthorityCode = localAuthorityCode;

        foreach (var establishment in otherEstablishments)
            establishment.LocalAuthorityCode = fixture.Create<string>();

        await dbContext.GiasEstablishments.AddRangeAsync(allEstablishments);

        var allProjects = allEstablishments.Select(establishment =>
        {
            var project = fixture.Customize(new ProjectCustomization
            {
                AssignedToId = testUser.Id
            }).Create<Project>();

            project.Urn = establishment.Urn ?? project.Urn;
            return project;
        }).ToList();

        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);
        allProjects.ForEach(p => p.LocalAuthorityId = localAuthority.Id);

        await dbContext.Projects.AddRangeAsync(allProjects);
        await dbContext.SaveChangesAsync();

        // Act
        var results = await projectsClient.ListAllProjectsForLocalAuthorityAsync(
            localAuthorityCode, null, null, 0, 50);

        var expectedProjects = allProjects
            .Where(p => p.State == Domain.Enums.ProjectState.Active)
            .Where(p => matchingEstablishments.Any(e => e.Urn?.Value == p.Urn.Value))
            .ToList();

        // Assert
        Assert.NotNull(results);
        Assert.Equal(expectedProjects.Count, results.Count);

        foreach (var result in results)
        {
            var project = expectedProjects.Find(p => p.Id.Value == result.ProjectId?.Value);
            var establishment = matchingEstablishments.Find(e => e.Urn?.Value == result.Urn?.Value);

            Assert.NotNull(result.Urn);
            Assert.Equal(project?.Urn.Value, result.Urn.Value);
            Assert.Equal(establishment?.Urn?.Value, result.Urn.Value);

            Assert.NotNull(result.EstablishmentName);
            Assert.Equal(establishment?.Name, result.EstablishmentName);

            Assert.NotNull(result.ProjectId);
            Assert.Equal(project?.Id.Value, result.ProjectId.Value);

            Assert.NotNull(result.ConversionOrTransferDate);
            Assert.Equal(project?.SignificantDate, new DateOnly(
                result.ConversionOrTransferDate.Value.Year,
                result.ConversionOrTransferDate.Value.Month,
                result.ConversionOrTransferDate.Value.Day));

            Assert.NotNull(result.State);
            Assert.Equal(project?.State.ToString(), result.State.ToString());

            Assert.NotNull(result.ProjectType);
            Assert.Equal(project?.Type?.ToString(), result.ProjectType.Value.ToString());

            Assert.Equal(project?.FormAMat, result.IsFormAMAT);

            Assert.NotNull(result.AssignedToFullName);
            Assert.Equal($"{project?.AssignedTo?.FirstName} {project?.AssignedTo?.LastName}", result.AssignedToFullName);
        }
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(GiasEstablishmentsCustomization))]
    public async Task ListAllProjectsForRegion_Async_ShouldReturnList(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ReadRole)];

        // Arrange
        var dbContext = factory.GetDbContext<CompleteContext>();
        var expectedRegion = Region.EastMidlands;

        var projects = fixture.Customize(new ProjectCustomization()).CreateMany<Project>(50).ToList();

        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);
        projects.ForEach(p => p.LocalAuthorityId = localAuthority.Id);

        await dbContext.Projects.AddRangeAsync(projects);
        await dbContext.SaveChangesAsync();

        // Act
        var results = await projectsClient.ListAllProjectsForRegionAsync(
            expectedRegion, null, null, AssignedToState.AssignedOnly, 0, 50);

        var expectedProjects = projects
            .Where(p => p.State == Domain.Enums.ProjectState.Active)
            .Where(p => p.Region != null && (Region)p.Region == expectedRegion)
            .ToList();

        // Assert
        Assert.NotNull(results);
        Assert.Equal(expectedProjects.Count, results.Count);
        Assert.All(results, project => Assert.Equal(project.Region, expectedRegion));
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(GiasEstablishmentsCustomization))]
    public async Task ListAllProjectsForRegionAsync_InvalidRegionSent_ShouldReturnList(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ReadRole)];

        // Act & Assert
        await Assert.ThrowsAsync<CompleteApiException>(() =>
            projectsClient.ListAllProjectsForRegionAsync(null, null, null, null, 0, 50));
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(GiasEstablishmentsCustomization))]
    public async Task ListAllProjectsForTeam_Async_ShouldReturnList(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ReadRole)];

        // Arrange
        var dbContext = factory.GetDbContext<CompleteContext>();
        var expectedTeam = ProjectTeam.BusinessSupport;

        var projects = fixture.Customize(new ProjectCustomization()).CreateMany<Project>(50).ToList();

        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);
        projects.ForEach(p => p.LocalAuthorityId = localAuthority.Id);

        await dbContext.Projects.AddRangeAsync(projects);
        await dbContext.SaveChangesAsync();

        // Act
        var results = await projectsClient.ListAllProjectsForTeamAsync(
            expectedTeam, null, null,AssignedToState.AssignedOnly, 0, 50);

        var expectedProjects = projects
            .Where(p => p.State == Domain.Enums.ProjectState.Active)
            .Where(p => p.Region != null && (ProjectTeam?)p.Team == expectedTeam)
            .ToList();

        // Assert
        Assert.NotNull(results);
        Assert.Equal(expectedProjects.Count, results.Count);
        Assert.All(results, project => Assert.Equal(project.Team, expectedTeam));
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(GiasEstablishmentsCustomization))]
    public async Task ListAllProjectsForTeamAsync_InvalidRegionSent_ShouldReturnList(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        // Arrange
        factory.TestClaims = [new Claim(ClaimTypes.Role, ReadRole)];

        // Act & Assert
        await Assert.ThrowsAsync<CompleteApiException>(() =>
            projectsClient.ListAllProjectsForTeamAsync(null, null, null, null, 0, 50));
    }

    [Theory(Skip = "Awaiting wiremock")]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(GiasEstablishmentsCustomization))]
    public async Task ListAllProjectsForUserAsync_ShouldReturnProjects(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture,
        Mock<ITrustsV4Client> mockTrustsClient)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ReadRole)];

        // // Arrange
        var dbContext = factory.GetDbContext<CompleteContext>();
        var testUser = await dbContext.Users.FirstAsync();
        var userAdId = "test-user-adid";
        var expectedUserId = testUser.Id;

        testUser.ActiveDirectoryUserId = userAdId;

        mockTrustsClient
            .Setup(tc => tc.GetByUkprnsAllAsync(It.IsAny<IEnumerable<string>?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new() { Ukprn = "12345678", Name = "Trust One" },
                new() { Ukprn = "87654321", Name = "Trust Two" }
            ]);

        var establishments = fixture.Customize(new GiasEstablishmentsCustomization()).CreateMany<GiasEstablishment>(50).ToList();
        var localAuthority = dbContext.LocalAuthorities.AsEnumerable().MinBy(_ => Guid.NewGuid());
        Assert.NotNull(localAuthority);

        await dbContext.GiasEstablishments.AddRangeAsync(establishments);

        var projects = establishments.Select((establishment, i) =>
        {
            var project = fixture.Customize(new ProjectCustomization
            {
                LocalAuthorityId = localAuthority.Id,
                IncomingTrustUkprn = "12345678",
                OutgoingTrustUkprn = "87654321",
            })
                .Create<Project>();
            project.Urn = establishment.Urn ?? project.Urn;
            if (i < 10) project.AssignedToId = testUser.Id;
            return project;
        }).ToList();

        await dbContext.Projects.AddRangeAsync(projects);
        await dbContext.SaveChangesAsync();

        // // Act
        var results = await projectsClient.ListAllProjectsForUserAsync(null, userAdId, null, 50);

        // // Assert
        Assert.NotNull(results);
        Assert.Equal(10, results.Count);
        Assert.All(results, project =>
        {
            Assert.Equal("Trust One", project.IncomingTrustName);
            Assert.Equal("Trust Two", project.OutgoingTrustName);
        });
    }

    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(GiasEstablishmentsCustomization))]
    public async Task ListAllProjectsForUserAsync_ShouldReturnBadRequest_IfUserNotFound(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ReadRole)];

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CompleteApiException>(() =>
            projectsClient.ListAllProjectsForUserAsync(null, "123", null, 50));

        Assert.Contains("User does not exist for provided UserAdId", exception.Response);
    }
}