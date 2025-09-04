using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Contacts.Commands;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using Moq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Reflection.Metadata;
using System.Threading;
using Entities = Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Contact;   

public class CreateExternalContactCommandTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldCreateExternalContactSuccessfully(
        [Frozen] ICompleteRepository<Entities.Contact> mockContactRepository,
        IFixture fixture,
        CreateExternalContactCommand command,
        CreateExternalContactCommandHandler sut        
        )
    {
        // Arrange       
        var contact = fixture.Build<Entities.Contact>()
            .With(q => q.Id, new ContactId(Guid.NewGuid()))
            .Create();

        mockContactRepository.AddAsync(Arg.Any<Entities.Contact>(), Arg.Any<CancellationToken>()).Returns(contact);

        // Act
        var result = await sut.Handle(command, cancellationToken);

        // Assert
        Assert.Multiple
        (
            () => Assert.NotNull(result),
            () => Assert.True(result.IsSuccess),
            () => Assert.IsType<ContactId>(result.Value),
            async () => await mockContactRepository.Received(1).AddAsync(Arg.Any<Entities.Contact>(), Arg.Any<CancellationToken>())
        );
    }


    [Theory]
    [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldReturnFailure_WhenExceptionOccurs(
      [Frozen] ICompleteRepository<Entities.Contact> mockContactRepository,
        CreateExternalContactCommand command,
        CreateExternalContactCommandHandler sut)
    {
        // Arrange       
        var expectedErrorMessage = string.Format(ErrorMessagesConstants.CouldNotCreateExternalContact, command.ProjectId?.Value);

        mockContactRepository.AddAsync(Arg.Any<Entities.Contact>(), Arg.Any<CancellationToken>())
           .ThrowsAsync(new Exception(expectedErrorMessage));

        // Act
        var result = await sut.Handle(command, cancellationToken);

        // Assert
        Assert.Multiple
        (
            () => Assert.False(result.IsSuccess),
            () => Assert.Equal(expectedErrorMessage, result.Error),
            async () => await mockContactRepository.DidNotReceive().AddAsync(Arg.Any<Entities.Contact>(), Arg.Any<CancellationToken>())
        );
    }

}
