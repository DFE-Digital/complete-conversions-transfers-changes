using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Contacts.Commands;
using Dfe.Complete.Domain.Constants;
using Entities = Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using Moq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Linq.Expressions;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Contact;

public class UpdateExternalContactCommandTests
{
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldUpdateExternalContactSuccessfully(
        [Frozen] ICompleteRepository<Entities.Contact> mockContactRepository,
        IFixture fixture,
        UpdateExternalContactCommand command,
        UpdateExternalContactCommandHandler sut
        )
    {
        // Arrange
        var contact = fixture.Build<Entities.Contact>()
            .With(q => q.Id, new ContactId(Guid.NewGuid()))
            .Create();

        mockContactRepository.GetAsync(Arg.Any<Expression<Func<Entities.Contact, bool>>>())
              .Returns(contact);

        mockContactRepository.UpdateAsync(Arg.Any<Entities.Contact>(), Arg.Any<CancellationToken>()).Returns(contact);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert        
        Assert.Multiple
        (
            () => Assert.NotNull(result),
            () => Assert.True(result.IsSuccess),
            async () => await mockContactRepository.Received(1).UpdateAsync(Arg.Any<Entities.Contact>(), Arg.Any<CancellationToken>())
        );
    }


    [Theory]
    [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldReturnFailure_WhenExceptionOccurs(
        [Frozen] ICompleteRepository<Entities.Contact> mockContactRepository,
        IFixture fixture,
        UpdateExternalContactCommand command,
        UpdateExternalContactCommandHandler sut)
    {
        // Arrange
        var expectedMessage = $"Could not update external contact with Id {command.contactDto.Id.Value}.";

        var contact = fixture.Build<Entities.Contact>()
           .With(q => q.Id, new ContactId(Guid.NewGuid()))
           .Create();

        mockContactRepository.GetAsync(Arg.Any<Expression<Func<Entities.Contact, bool>>>())
              .Returns(contact);

        mockContactRepository.UpdateAsync(Arg.Any<Entities.Contact>(), Arg.Any<CancellationToken>())
           .ThrowsAsync(new Exception("throws error"));

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.Multiple
        (
            () => Assert.False(result.IsSuccess),
            () => Assert.Equal(expectedMessage, result.Error),
            async () => await mockContactRepository.DidNotReceive().UpdateAsync(Arg.Any<Entities.Contact>(), Arg.Any<CancellationToken>())
        );
    }

    [Theory]
    [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldThrowException_WhenContactIsNotFound(
      [Frozen] ICompleteRepository<Entities.Contact> mockContactRepository,
      UpdateExternalContactCommand command,
      UpdateExternalContactCommandHandler sut)
    {
        // Arrange
        var expectedMessage = $"External contact with Id {command.ContactId.Value} not found.";        
        mockContactRepository.GetAsync(Arg.Any<Entities.Contact>(), Arg.Any<CancellationToken>()).Returns((Entities.Contact)null!);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.Multiple
        (
            () => Assert.False(result.IsSuccess),
            () => Assert.Equal(expectedMessage, result.Error),
            () => Assert.Equal(ErrorType.NotFound, result.ErrorType),
            async () => await mockContactRepository.DidNotReceive().UpdateAsync(Arg.Any<Entities.Contact>(), Arg.Any<CancellationToken>())
        );
    }

}
