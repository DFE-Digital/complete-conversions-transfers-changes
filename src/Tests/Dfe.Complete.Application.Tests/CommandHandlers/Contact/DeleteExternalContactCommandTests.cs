using AutoFixture;
using AutoFixture.AutoMoq;
using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Contacts.Commands;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using Entities = Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Contact;

public class DeleteExternalContactCommandTests
{
    private readonly Mock<IUnitOfWork> mockUnitOfWork;
    private readonly Mock<ICompleteRepository<Entities.Project>> mockProjectRepository;    
    private readonly Mock<ICompleteRepository<Entities.Contact>> mockContactRepository;
    private readonly Mock<ILogger<DeleteExternalContactCommandHandler>> mockLogger;
    private readonly DeleteExternalContactCommandHandler handler;
    private readonly IFixture fixture = new Fixture().Customize(new CompositeCustomization(new AutoMoqCustomization(), new ProjectIdCustomization(), new DateOnlyCustomization(), new IgnoreVirtualMembersCustomisation()));

    public DeleteExternalContactCommandTests()
    {
        mockUnitOfWork = new Mock<IUnitOfWork>();
        mockProjectRepository = new Mock<ICompleteRepository<Entities.Project>>();        
        mockContactRepository = new Mock<ICompleteRepository<Entities.Contact>>();
        mockLogger = new Mock<ILogger<DeleteExternalContactCommandHandler>>();
        handler = new DeleteExternalContactCommandHandler(
            mockUnitOfWork.Object,
            mockProjectRepository.Object,
            mockContactRepository.Object,            
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

        mockContactRepository.Setup(repo => repo.FindAsync(command.ContactId, It.IsAny<CancellationToken>()))
               .ReturnsAsync(contact);

        mockProjectRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Entities.Project, bool>>>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(project);

        mockContactRepository.Setup(repo => repo.RemoveAsync(It.IsAny<Entities.Contact>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(contact);

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
            () => mockContactRepository.Verify(repo => repo.RemoveAsync(It.IsAny<Entities.Contact>(), It.IsAny<CancellationToken>()), Times.Once),
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
       
        var expectedMessage = string.Format(ErrorMessagesConstants.CouldNotDeleteExternalContact, command.ContactId.Value);
        
        mockContactRepository.Setup(repo => repo.FindAsync(command.ContactId, It.IsAny<CancellationToken>()))
               .ReturnsAsync(contact);       

        mockContactRepository.Setup(repo => repo.RemoveAsync(It.IsAny<Entities.Contact>(), It.IsAny<CancellationToken>()))
           .ThrowsAsync(new Exception("throws error"));

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
        var expectedMessage = string.Format(ErrorMessagesConstants.NotFoundExternalContact, command.ContactId.Value);

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
            () => mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Never),
            () => mockContactRepository.Verify(repo => repo.RemoveAsync(It.IsAny<Entities.Contact>(), It.IsAny<CancellationToken>()), Times.Never)
        );
    }
}
