using AutoFixture;
using AutoFixture.AutoMoq;
using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using Moq;
using Entities = Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Project;

public class UpdatePrimaryContactAtOrganisationCommandTests
{
    private readonly IFixture fixture = new Fixture().Customize(new CompositeCustomization(new AutoMoqCustomization(), new ProjectIdCustomization(), new DateOnlyCustomization(), new IgnoreVirtualMembersCustomisation()));
    private readonly Mock<ICompleteRepository<Entities.Project>> mockProjectRepository;
    private readonly UpdatePrimaryContactAtOrganisation handler;


    public UpdatePrimaryContactAtOrganisationCommandTests()
    {
        mockProjectRepository = new Mock<ICompleteRepository<Entities.Project>>();        
        handler = new UpdatePrimaryContactAtOrganisation(mockProjectRepository.Object);
    }

    [Theory]
    [InlineData("5f7f01d0-9a2f-46a6-a971-cdf153c0df88", null, null, null, "5f7f01d0-9a2f-46a6-a971-cdf153c0df88", true, ContactCategory.SchoolOrAcademy)]
    [InlineData("5f7f01d0-9a2f-46a6-a971-cdf153c0df88", null, null, null, "5f7f01d0-9a2f-46a6-a971-cdf153c0df88", false, ContactCategory.SchoolOrAcademy)]
    [InlineData(null, "5f7f01d0-9a2f-46a6-a971-cdf153c0df88", null, null, "5f7f01d0-9a2f-46a6-a971-cdf153c0df88", true, ContactCategory.IncomingTrust)]
    [InlineData(null, "5f7f01d0-9a2f-46a6-a971-cdf153c0df88", null, null, "5f7f01d0-9a2f-46a6-a971-cdf153c0df88", false, ContactCategory.IncomingTrust)]
    [InlineData(null, null, "5f7f01d0-9a2f-46a6-a971-cdf153c0df88", null, "5f7f01d0-9a2f-46a6-a971-cdf153c0df88", true, ContactCategory.OutgoingTrust)]
    [InlineData(null, null, "5f7f01d0-9a2f-46a6-a971-cdf153c0df88", null, "5f7f01d0-9a2f-46a6-a971-cdf153c0df88", false, ContactCategory.OutgoingTrust)]
    [InlineData(null, null, null, "5f7f01d0-9a2f-46a6-a971-cdf153c0df88", "5f7f01d0-9a2f-46a6-a971-cdf153c0df88", true, ContactCategory.LocalAuthority)]
    [InlineData(null, null, null, "5f7f01d0-9a2f-46a6-a971-cdf153c0df88", "5f7f01d0-9a2f-46a6-a971-cdf153c0df88", false, ContactCategory.LocalAuthority)]
    public async Task Handle_ShouldUpdateProjectPrimaryContact_ValidData_WhenProjectExists(
        string EstablishmentMainContactId,
        string IncomingTrustMainContactId,
        string OutgoingTrustMainContactId,
        string LocalAuthorityMainContactId,
        string ContactIdValue,
        bool PrimaryContact,
        ContactCategory ContactCategory        
        )
    {
        // Arrange    
        ProjectId projectId = fixture.Create<ProjectId>();
        ContactId contactId = new ContactId(Guid.Parse(ContactIdValue));

        var project = fixture.Build<Entities.Project>()
               .With(t => t.Id, projectId)
               .With(t => t.EstablishmentMainContactId, EstablishmentMainContactId == null ? fixture.Create<ContactId>() : new ContactId(Guid.Parse(EstablishmentMainContactId)))
               .With(t => t.IncomingTrustMainContactId, IncomingTrustMainContactId == null ? fixture.Create<ContactId>() : new ContactId(Guid.Parse(IncomingTrustMainContactId)))
               .With(t => t.OutgoingTrustMainContactId, OutgoingTrustMainContactId == null ? fixture.Create<ContactId>() : new ContactId(Guid.Parse(OutgoingTrustMainContactId)))
               .With(t => t.LocalAuthorityMainContactId, LocalAuthorityMainContactId == null ? fixture.Create<ContactId>() : new ContactId(Guid.Parse(LocalAuthorityMainContactId)))
               .Create();

        var contact = fixture.Build<Entities.Contact>()
               .With(t => t.Id, contactId)
               .With(t => t.Category, ContactCategory)
               .With(t => t.ProjectId, projectId)
               .Create();       

        mockProjectRepository.Setup(repo => repo.FindAsync(projectId, It.IsAny<CancellationToken>()))
               .ReturnsAsync(project);
        
        var command = new UpdatePrimaryContactAtOrganisationCommand(projectId, PrimaryContact, contact);      

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert                             
        mockProjectRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Entities.Project>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotThrowOrUpdate_WhenProjectNotFound()
    {
        // Arrange
        var nonExistentProjectId = new ProjectId(Guid.NewGuid());
        var contactId = new ContactId(Guid.NewGuid());

        var contact = fixture.Build<Entities.Contact>()
               .With(t => t.Id, contactId)
               .With(t => t.Category, ContactCategory.Other)
               .With(t => t.ProjectId, nonExistentProjectId)
               .Create();

        mockProjectRepository.Setup(repo => repo.FindAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
              .ReturnsAsync((Entities.Project?)null);
        
        var command = new UpdatePrimaryContactAtOrganisationCommand(nonExistentProjectId, true, contact);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert        
        mockProjectRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.Project>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldNotThrowOrUpdate_WhenContactProject_ProjectPassed_NotSame()
    {
        // Arrange
        var contactProjectId = new ProjectId(Guid.NewGuid());
        var requestedProjectId = new ProjectId(Guid.NewGuid());
        var contactId = new ContactId(Guid.NewGuid());

        var contact = fixture.Build<Domain.Entities.Contact>()
               .With(t => t.Id, contactId)
               .With(t => t.Category, ContactCategory.Other)
               .With(t => t.ProjectId, contactProjectId)
               .Create();
        
        var command = new UpdatePrimaryContactAtOrganisationCommand(requestedProjectId, true, contact);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert        
        mockProjectRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.Project>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
