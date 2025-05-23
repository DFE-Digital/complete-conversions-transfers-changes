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
    public class UpdateLocalAuthorityCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICompleteRepository<Domain.Entities.LocalAuthority>> _mockLocalAuthorityRepository;
        private readonly Mock<ICompleteRepository<Domain.Entities.Contact>> _mockContactRepository;
        private readonly Mock<ILogger<UpdateLocalAuthorityCommandHandler>> _mockLogger;
        private readonly UpdateLocalAuthorityCommandHandler _handler;

        public UpdateLocalAuthorityCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLocalAuthorityRepository = new Mock<ICompleteRepository<Domain.Entities.LocalAuthority>>();
            _mockContactRepository = new Mock<ICompleteRepository<Domain.Entities.Contact>>();
            _mockLogger = new Mock<ILogger<UpdateLocalAuthorityCommandHandler>>();
            _handler = new UpdateLocalAuthorityCommandHandler(
                _mockUnitOfWork.Object,
                _mockLocalAuthorityRepository.Object,
                _mockContactRepository.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldUpdateLocalAuthorityWithoutContactSuccessfully()
        {
            var command = new UpdateLocalAuthorityCommand(
                new LocalAuthorityId(Guid.NewGuid()), "Code", "Address1", "Address2", "Address3",
                "AddressTown", "AddressCounty", "AddressPostcode");

            var localAuthority = Domain.Entities.LocalAuthority.CreateLocalAuthority(
                command.Id, "Name", "Code", "Address1", "Address2", "Address3",
                "AddressTown", "AddressCounty", "AddressPostcode", DateTime.UtcNow); 

            _mockLocalAuthorityRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Domain.Entities.LocalAuthority, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(localAuthority);

            _mockLocalAuthorityRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.LocalAuthority>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(localAuthority);

            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            _mockUnitOfWork.Verify(uow => uow.BeginTransactionAsync(), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
            _mockLocalAuthorityRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.LocalAuthority>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockContactRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.Contact>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockContactRepository.Verify(repo => repo.AddAsync(It.IsAny<Domain.Entities.Contact>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockUnitOfWork.Verify(uow => uow.RollBackAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldUpdateLocalAuthorityWithContactSuccessfully()
        {
            var command = new UpdateLocalAuthorityCommand(
                new LocalAuthorityId(Guid.NewGuid()), "Code", "Address1", "Address2", "Address3",
                "AddressTown", "AddressCounty", "AddressPostcode", new ContactId(Guid.NewGuid()),
                "Title", "ContactName", "Email", "Phone");

            var localAuthority = Domain.Entities.LocalAuthority.CreateLocalAuthority(
                command.Id, "Name", "Code", "Address1", "Address2", "Address3",
                "AddressTown", "AddressCounty", "AddressPostcode", DateTime.UtcNow);
            var contact = Domain.Entities.Contact.CreateContact(
                command.ContactId!, command.Title!, command.ContactName!, command.Email, command.Phone,
                localAuthority.Id, ContactCategory.LocalAuthority, ContactType.DirectorOfChildServices.ToDescription(), DateTime.UtcNow);

            _mockLocalAuthorityRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Domain.Entities.LocalAuthority, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(localAuthority);

            _mockLocalAuthorityRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.LocalAuthority>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(localAuthority);

            _mockContactRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Domain.Entities.Contact, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(contact);

            _mockContactRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.Contact>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(contact);

            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            _mockUnitOfWork.Verify(uow => uow.BeginTransactionAsync(), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
            _mockLocalAuthorityRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.LocalAuthority>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockContactRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.Contact>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockContactRepository.Verify(repo => repo.AddAsync(It.IsAny<Domain.Entities.Contact>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockUnitOfWork.Verify(uow => uow.RollBackAsync(), Times.Never);
        }
        [Fact]
        public async Task Handle_ShouldUpdateLocalAuthorityWithNewContactSuccessfully()
        {
            var command = new UpdateLocalAuthorityCommand(
                new LocalAuthorityId(Guid.NewGuid()), "Code", "Address1", "Address2", "Address3",
                "AddressTown", "AddressCounty", "AddressPostcode", new ContactId(Guid.NewGuid()),
                "Title", "ContactName", "Email", "Phone");

            var localAuthority = Domain.Entities.LocalAuthority.CreateLocalAuthority(
                command.Id, "Name", "Code", "Address1", "Address2", "Address3",
                "AddressTown", "AddressCounty", "AddressPostcode", DateTime.UtcNow);
            var contact = Domain.Entities.Contact.CreateContact(
                command.ContactId!, command.Title!, command.ContactName!, command.Email, command.Phone,
                localAuthority.Id, ContactCategory.LocalAuthority, ContactType.DirectorOfChildServices.ToDescription(), DateTime.UtcNow);

            _mockLocalAuthorityRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Domain.Entities.LocalAuthority, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(localAuthority);

            _mockLocalAuthorityRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.LocalAuthority>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(localAuthority); 

            _mockContactRepository.Setup(repo => repo.AddAsync(It.IsAny<Domain.Entities.Contact>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(contact);

            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            _mockUnitOfWork.Verify(uow => uow.BeginTransactionAsync(), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
            _mockLocalAuthorityRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.LocalAuthority>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockContactRepository.Verify(repo => repo.AddAsync(It.IsAny<Domain.Entities.Contact>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockContactRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.Contact>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockUnitOfWork.Verify(uow => uow.RollBackAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenLocalAuthorityDoesNotExist()
        {
            var command = new UpdateLocalAuthorityCommand(
                new LocalAuthorityId(Guid.NewGuid()), "Code", "Address1", "Address2", "Address3",
                "AddressTown", "AddressCounty", "AddressPostcode", new ContactId(Guid.NewGuid()),
                "Title", "ContactName", "Email", "Phone");

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal("Cannot update Local authority as it is not existed.", result.Error);
            _mockUnitOfWork.Verify(uow => uow.BeginTransactionAsync(), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.RollBackAsync(), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenExceptionOccurs()
        {
            var command = new UpdateLocalAuthorityCommand(
                new LocalAuthorityId(Guid.NewGuid()), "Code", "Address1", "Address2", "Address3",
                "AddressTown", "AddressCounty", "AddressPostcode", new ContactId(Guid.NewGuid()),
                "Title", "ContactName", "Email", "Phone");

            _mockLocalAuthorityRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Domain.Entities.LocalAuthority, bool>>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal("Database error", result.Error);
            _mockUnitOfWork.Verify(uow => uow.BeginTransactionAsync(), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Never);
            _mockUnitOfWork.Verify(uow => uow.RollBackAsync(), Times.Once);
        }
    }

}
