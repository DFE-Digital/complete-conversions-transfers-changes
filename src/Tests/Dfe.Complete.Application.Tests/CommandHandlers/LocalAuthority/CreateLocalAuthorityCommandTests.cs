using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.LocalAuthorities.Commands;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace Dfe.Complete.Application.Tests.CommandHandlers.LocalAuthority
{
    public class CreateLocalAuthorityCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICompleteRepository<Domain.Entities.LocalAuthority>> _mockLocalAuthorityRepository;
        private readonly Mock<ICompleteRepository<Domain.Entities.Contact>> _mockContactRepository;
        private readonly Mock<ILogger<CreateLocalAuthorityCommandHandler>> _mockLogger;
        private readonly CreateLocalAuthorityCommandHandler _handler;
        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        public CreateLocalAuthorityCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLocalAuthorityRepository = new Mock<ICompleteRepository<Domain.Entities.LocalAuthority>>();
            _mockContactRepository = new Mock<ICompleteRepository<Domain.Entities.Contact>>();
            _mockLogger = new Mock<ILogger<CreateLocalAuthorityCommandHandler>>();
            _handler = new CreateLocalAuthorityCommandHandler(
                _mockUnitOfWork.Object,
                _mockLocalAuthorityRepository.Object,
                _mockContactRepository.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateLocalAuthoritySuccessfully()
        {
            var command = new CreateLocalAuthorityCommand(
                new LocalAuthorityId(Guid.NewGuid()), "Code", "Name", "Address1", "Address2", "Address3",
                "AddressTown", "AddressCounty", "AddressPostcode", new ContactId(Guid.NewGuid()),
                "Title", "ContactName", "Email", "Phone");

            _mockLocalAuthorityRepository.Setup(repo => repo.AddAsync(It.IsAny<Domain.Entities.LocalAuthority>(), _cancellationToken))
                .Returns(Task.FromResult(It.IsAny<Domain.Entities.LocalAuthority>()));

            _mockContactRepository.Setup(repo => repo.AddAsync(It.IsAny<Domain.Entities.Contact>(), _cancellationToken))
               .Returns(Task.FromResult(It.IsAny<Domain.Entities.Contact>()));

            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, _cancellationToken);

            Assert.True(result.IsSuccess);
            _mockUnitOfWork.Verify(uow => uow.BeginTransactionAsync(), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
            _mockLocalAuthorityRepository.Verify(repo => repo.AddAsync(It.IsAny<Domain.Entities.LocalAuthority>(), _cancellationToken), Times.Once);
            _mockContactRepository.Verify(repo => repo.AddAsync(It.IsAny<Domain.Entities.Contact>(), _cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenLocalAuthorityAlreadyExists()
        {
            var command = new CreateLocalAuthorityCommand(
                new LocalAuthorityId(Guid.NewGuid()), "Code", "Name", "Address1", "Address2", "Address3",
                "AddressTown", "AddressCounty", "AddressPostcode", new ContactId(Guid.NewGuid()),
                "Title", "ContactName", "Email", "Phone");

            var existingLocalAuthority = Domain.Entities.LocalAuthority.Create(
                command.Id, command.Name, command.Code, new AddressDetails(command.Address1, command.Address2, command.Address3,
                command.AddressTown, command.AddressCounty, command.AddressPostcode), DateTime.UtcNow);

            _mockLocalAuthorityRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Domain.Entities.LocalAuthority, bool>>>(), _cancellationToken))
              .ReturnsAsync(true); 

            var result = await _handler.Handle(command, _cancellationToken);

            Assert.False(result.IsSuccess);
            Assert.Equal($"Already existed local authority with code {command.Code}", result.Error);

            _mockUnitOfWork.Verify(uow => uow.BeginTransactionAsync(), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.RollBackAsync(), Times.Once);
            _mockLocalAuthorityRepository.Verify(repo => repo.FindAsync(It.IsAny<Expression<Func<Domain.Entities.LocalAuthority, bool>>>(), _cancellationToken), Times.Never());
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenExceptionOccurs()
        {
            var command = new CreateLocalAuthorityCommand(
                new LocalAuthorityId(Guid.NewGuid()), "Code", "Name", "Address1", "Address2", "Address3",
                "AddressTown", "AddressCounty", "AddressPostcode", new ContactId(Guid.NewGuid()),
                "Title", "ContactName", "Email", "Phone");

            _mockLocalAuthorityRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Domain.Entities.LocalAuthority, bool>>>(), _cancellationToken))
                .ThrowsAsync(new Exception("Database error"));

            var result = await _handler.Handle(command, _cancellationToken);

            Assert.False(result.IsSuccess);
            Assert.Equal("Database error", result.Error);

            _mockUnitOfWork.Verify(uow => uow.BeginTransactionAsync(), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.RollBackAsync(), Times.Once);
        }
    }

}
