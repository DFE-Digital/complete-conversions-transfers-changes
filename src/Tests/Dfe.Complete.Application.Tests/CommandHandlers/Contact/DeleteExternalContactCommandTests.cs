using AutoFixture;
using AutoFixture.AutoMoq;
using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Contacts.Commands;
using Dfe.Complete.Application.Contacts.Interfaces;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Tests.Common.Customizations.Models;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using System.Linq.Expressions;
using Entities = Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Contact;

public class DeleteExternalContactCommandTests
{
    private readonly Mock<IUnitOfWork> mockUnitOfWork;
    private readonly Mock<ICompleteRepository<Entities.Project>> mockProjectRepository;    
    private readonly Mock<IContactReadRepository> mockContactReadRepository;
    private readonly Mock<IContactWriteRepository> mockContactWriteRepository;
    private readonly Mock<ILogger<DeleteExternalContactCommandHandler>> mockLogger;
    private readonly DeleteExternalContactCommandHandler handler;
    private readonly IFixture fixture = new Fixture().Customize(new CompositeCustomization(new AutoMoqCustomization(), new ProjectIdCustomization(), new DateOnlyCustomization(), new IgnoreVirtualMembersCustomisation()));

    public DeleteExternalContactCommandTests()
    {
        mockUnitOfWork = new Mock<IUnitOfWork>();
        mockProjectRepository = new Mock<ICompleteRepository<Entities.Project>>();        
        mockContactReadRepository = new Mock<IContactReadRepository>();
        mockContactWriteRepository = new Mock<IContactWriteRepository>();
        mockLogger = new Mock<ILogger<DeleteExternalContactCommandHandler>>();
        handler = new DeleteExternalContactCommandHandler(
            mockUnitOfWork.Object,
            mockProjectRepository.Object,
            mockContactReadRepository.Object,
            mockContactWriteRepository.Object,
            mockLogger.Object);
    }

    [Theory]
    [InlineData("5f7f01d0-9a2f-46a6-a971-cdf153c0df88", null, null, null, "5f7f01d0-9a2f-46a6-a971-cdf153c0df88")]
    [InlineData(null, "5f7f01d0-9a2f-46a6-a971-cdf153c0df88", null, null, "5f7f01d0-9a2f-46a6-a971-cdf153c0df88")]
    [InlineData(null, null, "5f7f01d0-9a2f-46a6-a971-cdf153c0df88", null, "5f7f01d0-9a2f-46a6-a971-cdf153c0df88")]
    [InlineData(null, null, null, "5f7f01d0-9a2f-46a6-a971-cdf153c0df88", "5f7f01d0-9a2f-46a6-a971-cdf153c0df88")]    
    public async Task Handle_ShouldDeleteExternalContactSuccessfully(
        string EstablishmentMainContactId,
        string IncomingTrustMainContactId,
        string OutgoingTrustMainContactId,
        string LocalAuthorityMainContactId,
        string contactIdValue
        )
    {

        // Arrange             
        ProjectId projectId = fixture.Create<ProjectId>();
        ContactId contactId = new ContactId(Guid.Parse(contactIdValue));

        var command = new DeleteExternalContactCommand(contactId);            

        var contact = fixture.Build<Entities.Contact>()
                .With(q => q.Id, contactId)
                .With(q => q.ProjectId, projectId)
                .Create();

        var project = fixture.Build<Entities.Project>()
               .With(t => t.Id, projectId)
               .With(t => t.EstablishmentMainContactId, EstablishmentMainContactId == null ? fixture.Create<ContactId>() : new ContactId(Guid.Parse(EstablishmentMainContactId)))
               .With(t => t.IncomingTrustMainContactId, IncomingTrustMainContactId == null ? fixture.Create<ContactId>() : new ContactId(Guid.Parse(IncomingTrustMainContactId)))
               .With(t => t.OutgoingTrustMainContactId, OutgoingTrustMainContactId == null ? fixture.Create<ContactId>() : new ContactId(Guid.Parse(OutgoingTrustMainContactId)))
               .With(t => t.LocalAuthorityMainContactId, LocalAuthorityMainContactId == null ? fixture.Create<ContactId>() : new ContactId(Guid.Parse(LocalAuthorityMainContactId)))
               .Create();

        // Arrange
        var queryableContacts = new List<Entities.Contact> { contact }.AsQueryable().BuildMock();
        mockContactReadRepository.Setup(repo => repo.Contacts).Returns(queryableContacts);

        mockProjectRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Entities.Project, bool>>>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(project);

        mockContactWriteRepository.Setup(repo => repo.RemoveContactAsync(It.IsAny<Entities.Contact>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        mockProjectRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Entities.Project>(), It.IsAny<CancellationToken>()))
              .ReturnsAsync(project);

        mockUnitOfWork.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert        
        Assert.Multiple
        (
            () => Assert.NotNull(result),
            () => Assert.True(result.IsSuccess),
            () => mockContactWriteRepository.Verify(repo => repo.RemoveContactAsync(It.IsAny<Entities.Contact>(), It.IsAny<CancellationToken>()), Times.Once),
            () => mockProjectRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Entities.Project>(), It.IsAny<CancellationToken>()), Times.Once),
            () => mockUnitOfWork.Verify(uow => uow.BeginTransactionAsync(), Times.Once),
            () => mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once),
            () => mockUnitOfWork.Verify(uow => uow.RollBackAsync(), Times.Never)
        );
    }


    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenExceptionOccurs()
    {
        // Arrange
        ContactId contactId = fixture.Create<ContactId>();
        ProjectId projectId = fixture.Create<ProjectId>();        

        var command = new DeleteExternalContactCommand(contactId);

        var contact = fixture.Build<Entities.Contact>()
                .With(q => q.Id, contactId)
                .With(q => q.ProjectId, projectId)
                .Create();

        // Arrange
        var queryableContacts = new List<Entities.Contact> { contact }.AsQueryable().BuildMock();
        mockContactReadRepository.Setup(repo => repo.Contacts).Returns(queryableContacts);

        mockContactWriteRepository.Setup(repo => repo.RemoveContactAsync(It.IsAny<Entities.Contact>(), It.IsAny<CancellationToken>()))
           .ThrowsAsync(new Exception("throws error"));

        var expectedMessage = $"Could not delete external contact with Id {contactId.Value}.";

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Multiple
        (
            () => Assert.False(result.IsSuccess),
            () => Assert.Equal(expectedMessage, result.Error),
            () => mockUnitOfWork.Verify(uow => uow.BeginTransactionAsync(), Times.Once),
            () => mockUnitOfWork.Verify(uow => uow.RollBackAsync(), Times.Once),
            () => mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Never)            
        );
    }

    [Fact]    
    public async Task Handle_ShouldThrowException_WhenContactIsNotFound()
    {
        // Arrange
        ContactId contactId = fixture.Create<ContactId>();
        var command = new DeleteExternalContactCommand(contactId);
        var expectedMessage = $"External contact with Id {contactId.Value} not found.";

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Multiple
        (
            () => Assert.False(result.IsSuccess),
            () => Assert.Equal(expectedMessage, result.Error),
            () => mockUnitOfWork.Verify(uow => uow.BeginTransactionAsync(), Times.Once),
            () => mockUnitOfWork.Verify(uow => uow.RollBackAsync(), Times.Once),
            () => mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Never),            
            () => mockContactWriteRepository.Verify(repo => repo.RemoveContactAsync(It.IsAny<Entities.Contact>(), It.IsAny<CancellationToken>()), Times.Never)
        );
    }
}
