using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Contacts.Commands;
using Dfe.Complete.Application.Contacts.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using Moq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Entities = Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Contact;

public class CreateExternalContactCommandTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldCreateExternalContactSuccessfully(
        [Frozen] IContactWriteRepository mockContactWriteRepository,
        IFixture fixture,
        CreateExternalContactCommand command,
        CreateExternalContactCommandHandler sut
        )
    {
        // Arrange       
        var contact = fixture.Build<Entities.Contact>()
            .With(q => q.Id, new ContactId(Guid.NewGuid()))
            .Create();

        mockContactWriteRepository.CreateContactAsync(Arg.Any<Entities.Contact>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        var result = await sut.Handle(command, cancellationToken);

        // Assert
        Assert.Multiple
        (
            () => Assert.NotNull(result),
            () => Assert.True(result.IsSuccess),
            () => Assert.IsType<ContactId>(result.Value),
            async () => await mockContactWriteRepository.Received(1).CreateContactAsync(Arg.Any<Entities.Contact>(), Arg.Any<CancellationToken>())
        );
    }


    [Theory]
    [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldReturnFailure_WhenExceptionOccurs(
       [Frozen] IContactWriteRepository mockContactWriteRepository,
        CreateExternalContactCommand command,
        CreateExternalContactCommandHandler sut)
    {
        // Arrange       
        mockContactWriteRepository.CreateContactAsync(Arg.Any<Entities.Contact>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("Error"));

        // Act
        var result = await sut.Handle(command, cancellationToken);

        // Assert
        Assert.Multiple
        (
            () => Assert.False(result.IsSuccess),
            async () => await mockContactWriteRepository.DidNotReceive().CreateContactAsync(Arg.Any<Entities.Contact>(), Arg.Any<CancellationToken>())
        );
    }

}
