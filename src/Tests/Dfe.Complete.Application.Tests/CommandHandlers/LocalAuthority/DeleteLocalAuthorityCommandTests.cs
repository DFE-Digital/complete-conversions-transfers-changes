using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.LocalAuthorities.Commands;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace Dfe.Complete.Application.Tests.CommandHandlers.LocalAuthority
{
    public class DeleteLocalAuthorityCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICompleteRepository<Domain.Entities.Project>> _mockProjectRepository;
        private readonly Mock<ICompleteRepository<Domain.Entities.LocalAuthority>> _mockLocalAuthorityRepository;
        private readonly Mock<ICompleteRepository<Domain.Entities.Contact>> _mockContactRepository;
        private readonly Mock<ILogger<DeleteLocalAuthorityCommandHandler>> _mockLogger;
        private readonly DeleteLocalAuthorityCommandHandler _handler;

        public DeleteLocalAuthorityCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockProjectRepository = new Mock<ICompleteRepository<Domain.Entities.Project>>();
            _mockLocalAuthorityRepository = new Mock<ICompleteRepository<Domain.Entities.LocalAuthority>>();
            _mockContactRepository = new Mock<ICompleteRepository<Domain.Entities.Contact>>();
            _mockLogger = new Mock<ILogger<DeleteLocalAuthorityCommandHandler>>();
            _handler = new DeleteLocalAuthorityCommandHandler(
                _mockUnitOfWork.Object,
                _mockProjectRepository.Object,
                _mockLocalAuthorityRepository.Object,
                _mockContactRepository.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldDeleteLocalAuthorityAndContactSuccessfully()
        {
            var command = new DeleteLocalAuthorityCommand(new LocalAuthorityId(Guid.NewGuid())); 

            var localAuthority = Domain.Entities.LocalAuthority.CreateLocalAuthority(
                command.Id, "Name", "Code", "Address1", "Address2", "Address3",
                "AddressTown", "AddressCounty", "AddressPostcode", DateTime.UtcNow);
            var contact = Domain.Entities.Contact.CreateContact(
                new ContactId(Guid.NewGuid()),"Title", "Name", "Email", "Phone",
                command.Id, ContactCategory.LocalAuthority, ContactType.DirectorOfChildServices.ToDescription(), DateTime.UtcNow);

            _mockLocalAuthorityRepository.Setup(repo => repo.FindAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(localAuthority);

            _mockLocalAuthorityRepository.Setup(repo => repo.RemoveAsync(It.IsAny<Domain.Entities.LocalAuthority>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(localAuthority);
             

            _mockContactRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Domain.Entities.Contact, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(contact);

            _mockContactRepository.Setup(repo => repo.RemoveAsync(It.IsAny<Domain.Entities.Contact>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(contact);

            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            _mockUnitOfWork.Verify(uow => uow.BeginTransactionAsync(), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
            _mockLocalAuthorityRepository.Verify(repo => repo.RemoveAsync(It.IsAny<Domain.Entities.LocalAuthority>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockContactRepository.Verify(repo => repo.RemoveAsync(It.IsAny<Domain.Entities.Contact>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.RollBackAsync(), Times.Never);
        }
        [Fact]
        public async Task Handle_ShouldDeleteLocalAuthorityWithoutContactSuccessfully()
        {
            var command = new DeleteLocalAuthorityCommand(new LocalAuthorityId(Guid.NewGuid()));

            var localAuthority = Domain.Entities.LocalAuthority.CreateLocalAuthority(
                command.Id, "Name", "Code", "Address1", "Address2", "Address3",
                "AddressTown", "AddressCounty", "AddressPostcode", DateTime.UtcNow); 

            _mockLocalAuthorityRepository.Setup(repo => repo.FindAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(localAuthority);

            _mockLocalAuthorityRepository.Setup(repo => repo.RemoveAsync(It.IsAny<Domain.Entities.LocalAuthority>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(localAuthority);

            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            _mockUnitOfWork.Verify(uow => uow.BeginTransactionAsync(), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
            _mockLocalAuthorityRepository.Verify(repo => repo.RemoveAsync(It.IsAny<Domain.Entities.LocalAuthority>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockContactRepository.Verify(repo => repo.RemoveAsync(It.IsAny<Domain.Entities.Contact>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockUnitOfWork.Verify(uow => uow.RollBackAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenLocalAuthorityLinkedToProject()
        {
            var command = new DeleteLocalAuthorityCommand(new LocalAuthorityId(Guid.NewGuid()));
            var linkedProject = new Domain.Entities.Project(new ProjectId(Guid.NewGuid()), new Urn(1), DateTime.Now, DateTime.Now, TaskType.Transfer, ProjectType.Transfer, Guid.NewGuid(),
                DateOnly.MaxValue, false, null, null, null, false, null, DateOnly.MinValue, string.Empty, string.Empty, string.Empty, null, null, null, null, null, null, null, null, command.Id.Value);

            _mockProjectRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Domain.Entities.Project, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(linkedProject);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal("Cannot delete Local authority as it is linked to a project.", result.Error);
            _mockUnitOfWork.Verify(uow => uow.BeginTransactionAsync(), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.RollBackAsync(), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenLocalAuthorityNotFound()
        {
            var command = new DeleteLocalAuthorityCommand(new LocalAuthorityId(Guid.NewGuid()));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal($"Local authority with Id {command.Id} not found.", result.Error);
            _mockUnitOfWork.Verify(uow => uow.BeginTransactionAsync(), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.RollBackAsync(), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Never);
        } 
    }
}
