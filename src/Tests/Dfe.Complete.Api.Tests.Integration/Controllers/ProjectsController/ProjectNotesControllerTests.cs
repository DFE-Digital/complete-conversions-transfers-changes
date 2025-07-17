using System.Security.Claims;
using AutoFixture;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using DfE.CoreLibs.Testing.Mocks.WebApplicationFactory;
using Microsoft.EntityFrameworkCore;
using Project = Dfe.Complete.Domain.Entities.Project;
using GiasEstablishment = Dfe.Complete.Domain.Entities.GiasEstablishment;
using LocalAuthority = Dfe.Complete.Domain.Entities.LocalAuthority;
using Note = Dfe.Complete.Domain.Entities.Note;
using Dfe.Complete.Domain.Constants;

namespace Dfe.Complete.Api.Tests.Integration.Controllers.ProjectsController;

public partial class ProjectsControllerTests
{

    [Theory]
    [CustomAutoData(
        typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(OmitCircularReferenceCustomization),
        typeof(ProjectCustomization),
        typeof(NoteCustomization))]
    public async Task GetNotesByProjectIdAsync_ShouldReturnList(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = [new Claim(ClaimTypes.Role, ApiRoles.ReadRole)];

        // Arrange
        var dbContext = factory.GetDbContext<CompleteContext>();
        var testUser = await dbContext.Users.FirstAsync();

        var establishments = fixture.Customize(new GiasEstablishmentsCustomization()).CreateMany<GiasEstablishment>(50)
            .ToList();

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

        var firstProjectId = projects.First().Id;
        var notesForProject = fixture.Customize(new NoteCustomization { ProjectId = firstProjectId, UserId = testUser.Id }).CreateMany<Note>(10).ToList();
        var notesForOtherProject = fixture.Customize(new NoteCustomization { ProjectId = projects[1].Id, UserId = testUser.Id }).CreateMany<Note>(5).ToList();

        notesForProject[7].NotableId = new Guid();
        notesForProject[8].NotableType = "A type";
        notesForProject[9].TaskIdentifier = "A task identifier";

        await dbContext.Notes.AddRangeAsync(notesForProject);
        await dbContext.Notes.AddRangeAsync(notesForOtherProject);
        await dbContext.Projects.AddRangeAsync(projects);
        await dbContext.SaveChangesAsync();

        // Act
        var results = await projectsClient.GetNotesByProjectIdAsync(firstProjectId.Value);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(7, results.Count);
        Assert.All(results, x => Assert.Equivalent(firstProjectId, x.ProjectId));
    }

    [Theory]
    [CustomAutoData(
        typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(OmitCircularReferenceCustomization),
        typeof(ProjectCustomization))]
    public async Task CreateProjectNoteAsync_ShouldCreateProjectNote(
        CustomWebApplicationDbContextFactory<Program> factory,
        IFixture fixture,
        IProjectsClient projectsClient)
    {
        // Arrange
        factory.TestClaims = [
            new Claim(ClaimTypes.Role, ApiRoles.WriteRole),
            new Claim(ClaimTypes.Role, ApiRoles.ReadRole)
        ];

        var dbContext = factory.GetDbContext<CompleteContext>();

        var testUser = await dbContext.Users.FirstOrDefaultAsync();
        var establishment = fixture.Create<GiasEstablishment>();
        var localAuthority = fixture.Create<LocalAuthority>();
        var project = fixture.Create<Project>();
        project.Urn = establishment.Urn ?? project.Urn;
        project.LocalAuthorityId = localAuthority.Id;
        project.RegionalDeliveryOfficerId = testUser!.Id;
        project.CaseworkerId = testUser!.Id;
        project.AssignedToId = testUser!.Id;
        await dbContext.LocalAuthorities.AddAsync(localAuthority);
        await dbContext.GiasEstablishments.AddAsync(establishment);
        await dbContext.Projects.AddAsync(project);
        await dbContext.SaveChangesAsync();

        Assert.NotNull(testUser);

        await dbContext.SaveChangesAsync();

        var command = fixture.Create<CreateNoteCommand>();
        command.ProjectId = new ProjectId { Value = project.Id.Value };
        command.UserId = new UserId { Value = testUser.Id.Value };
        command.Body = "Test note body";

        // Act
        var result = await projectsClient.CreateProjectNoteAsync(command);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NoteId>(result);

        var notes = await dbContext.Notes.ToListAsync();
        var note = notes.FirstOrDefault(n => n.Id.Value == result.Value);

        Assert.NotNull(note);
        Assert.Equal("Test note body", note!.Body);
    }

    [Theory]
    [CustomAutoData(
        typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(OmitCircularReferenceCustomization),
        typeof(ProjectCustomization),
        typeof(NoteCustomization))]
    public async Task UpdateProjectNoteAsync_ShouldUpdateProjectNote(
        CustomWebApplicationDbContextFactory<Program> factory,
        IFixture fixture,
        IProjectsClient projectsClient)
    {
        // Arrange
        var dbContext = factory.GetDbContext<CompleteContext>();

        var testUser = await dbContext.Users.FirstOrDefaultAsync();

        factory.TestClaims = [
            new Claim(ClaimTypes.Role, ApiRoles.WriteRole),
            new Claim(ClaimTypes.Role, ApiRoles.ReadRole),
            new Claim(ClaimTypes.Role, ApiRoles.UpdateRole),
            new Claim(CustomClaimTypeConstants.UserId, testUser!.Id.Value.ToString()),
        ];

        var establishment = fixture.Create<GiasEstablishment>();
        var localAuthority = fixture.Create<LocalAuthority>();
        var project = fixture.Create<Project>();
        var newNote = fixture.Create<Note>();

        newNote.ProjectId = project.Id;
        newNote.UserId = testUser.Id;
        project.Urn = establishment.Urn ?? project.Urn;
        project.LocalAuthorityId = localAuthority.Id;
        project.RegionalDeliveryOfficerId = testUser!.Id;
        project.CaseworkerId = testUser!.Id;
        project.AssignedToId = testUser!.Id;
        await dbContext.LocalAuthorities.AddAsync(localAuthority);
        await dbContext.GiasEstablishments.AddAsync(establishment);
        await dbContext.Notes.AddAsync(newNote);
        await dbContext.Projects.AddAsync(project);
        await dbContext.SaveChangesAsync();

        Assert.NotNull(testUser);

        await dbContext.SaveChangesAsync();

        var command = fixture.Create<UpdateNoteCommand>();
        command.NoteId = new NoteId { Value = newNote.Id.Value };
        command.Body = "My new test note body";

        // Act
        var result = await projectsClient.UpdateProjectNoteAsync(command);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NoteId>(result);
        Assert.Equivalent(newNote.Id, result);

        dbContext.ChangeTracker.Clear();
        var notes = await dbContext.Notes.ToListAsync();
        var note = notes.FirstOrDefault(n => n.Id.Value == result.Value);

        Assert.NotNull(note);
        Assert.Equal("My new test note body", note!.Body);
    }

    [Theory]
    [CustomAutoData(
        typeof(CustomWebApplicationDbContextFactoryCustomization),
        typeof(DateOnlyCustomization),
        typeof(OmitCircularReferenceCustomization),
        typeof(ProjectCustomization),
        typeof(NoteCustomization))]
    public async Task DeleteProjectNoteAsync_ShouldDeleteProjectNote(
        CustomWebApplicationDbContextFactory<Program> factory,
        IFixture fixture,
        IProjectsClient projectsClient)
    {
        // Arrange
        var dbContext = factory.GetDbContext<CompleteContext>();

        var testUser = await dbContext.Users.FirstOrDefaultAsync();

        factory.TestClaims = [
            new Claim(ClaimTypes.Role, ApiRoles.WriteRole),
            new Claim(ClaimTypes.Role, ApiRoles.ReadRole),
            new Claim(ClaimTypes.Role, ApiRoles.UpdateRole),
            new Claim(ClaimTypes.Role, ApiRoles.DeleteRole),
            new Claim(CustomClaimTypeConstants.UserId, testUser!.Id.Value.ToString()),
        ];

        var establishment = fixture.Create<GiasEstablishment>();
        var localAuthority = fixture.Create<LocalAuthority>();
        var project = fixture.Create<Project>();
        var newNote = fixture.Create<Note>();

        newNote.ProjectId = project.Id;
        newNote.UserId = testUser.Id;
        project.Urn = establishment.Urn ?? project.Urn;
        project.LocalAuthorityId = localAuthority.Id;
        project.RegionalDeliveryOfficerId = testUser!.Id;
        project.CaseworkerId = testUser!.Id;
        project.AssignedToId = testUser!.Id;
        await dbContext.LocalAuthorities.AddAsync(localAuthority);
        await dbContext.GiasEstablishments.AddAsync(establishment);
        await dbContext.Notes.AddAsync(newNote);
        await dbContext.Projects.AddAsync(project);
        await dbContext.SaveChangesAsync();

        Assert.NotNull(testUser);

        await dbContext.SaveChangesAsync();

        var command = fixture.Create<RemoveNoteCommand>();
        command.NoteId = new NoteId { Value = newNote.Id.Value };

        // Act
        var result = await projectsClient.DeleteProjectNoteAsync(command);

        // Assert
        Assert.True(result);

        dbContext.ChangeTracker.Clear();
        var notes = await dbContext.Notes.ToListAsync();
        var note = notes.FirstOrDefault(n => n.Id.Value == newNote.Id.Value);

        Assert.Null(note);
    }
}