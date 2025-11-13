using AutoFixture;
using Dfe.Complete.Api.Tests.Integration.Customizations;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Tests.Common.Constants;
using Dfe.Complete.Tests.Common.Customizations.Models;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.Mocks.WebApplicationFactory;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using GiasEstablishment = Dfe.Complete.Domain.Entities.GiasEstablishment;
using Project = Dfe.Complete.Domain.Entities.Project;

namespace Dfe.Complete.Api.Tests.Integration.Controllers.ProjectsController;

public partial class ProjectsControllerTests
{
    [Theory]
    [CustomAutoData(typeof(CustomWebApplicationDbContextFactoryCustomization), typeof(GiasEstablishmentsCustomization))]
    public async Task RemoveProjectsShouldRemoveConversionProjectAndChildren(
        CustomWebApplicationDbContextFactory<Program> factory,
        IProjectsClient projectsClient,
        IFixture fixture)
    {
        factory.TestClaims = new[] { ApiRoles.ReadRole, ApiRoles.WriteRole, ApiRoles.DeleteRole, ApiRoles.UpdateRole }
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
        factory.TestClaims = new[] { ApiRoles.ReadRole, ApiRoles.WriteRole, ApiRoles.DeleteRole, ApiRoles.UpdateRole }
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
        factory.TestClaims = new[] { ApiRoles.ReadRole, ApiRoles.WriteRole, ApiRoles.DeleteRole, ApiRoles.UpdateRole }
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
}