using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Contacts.Commands;
using Dfe.Complete.Application.Contacts.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using Moq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Entities = Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Tests.CommandHandlers.Contact;

public class UpdateExternalContactCommandTests
{
    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization),
        typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldUpdateExternalContactSuccessfully(
        [Frozen] IContactReadRepository mockContactReadRepository,
        [Frozen] IContactWriteRepository mockContactWriteRepository,
        IFixture fixture,
        UpdateExternalContactCommand command,
        UpdateExternalContactCommandHandler sut
        )
    {
        // Arrange
        var contact = fixture.Build<Entities.Contact>()
            .With(q => q.Id, command.contactDto.Id)
            .Create();

        // Arrange
        var queryableContacts = new List<Entities.Contact> { contact }.AsQueryable().BuildMock();
        mockContactReadRepository.Contacts.Returns(queryableContacts);

        mockContactWriteRepository.UpdateContactAsync(Arg.Any<Entities.Contact>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert        
        Assert.Multiple
        (
            () => Assert.NotNull(result),
            () => Assert.True(result.IsSuccess),
            async () => await mockContactWriteRepository.Received(1).UpdateContactAsync(Arg.Any<Entities.Contact>(), Arg.Any<CancellationToken>())
        );
    }


    [Theory]
    [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldReturnFailure_WhenExceptionOccurs(
        [Frozen] IContactReadRepository mockContactReadRepository,
        [Frozen] IContactWriteRepository mockContactWriteRepository,
        IFixture fixture,
        UpdateExternalContactCommand command,
        UpdateExternalContactCommandHandler sut)
    {
        // Arrange
        var expectedMessage = $"Could not update external contact with Id {command.contactDto.Id.Value}.";

        var contact = fixture.Build<Entities.Contact>()
           .With(q => q.Id, command.contactDto.Id)
           .Create();       

        var queryableContacts = new List<Entities.Contact> { contact }.AsQueryable().BuildMock();
        mockContactReadRepository.Contacts.Returns(queryableContacts);

        mockContactWriteRepository.UpdateContactAsync(Arg.Any<Entities.Contact>(), Arg.Any<CancellationToken>())
           .ThrowsAsync(new Exception("throws error"));

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.Multiple
        (
            () => Assert.False(result.IsSuccess),
            () => Assert.Equal(expectedMessage, result.Error),
            async () => await mockContactWriteRepository.DidNotReceive().UpdateContactAsync(Arg.Any<Entities.Contact>(), Arg.Any<CancellationToken>())
        );
    }

    [Theory]
    [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldThrowException_WhenContactIsNotFound(
      [Frozen] IContactReadRepository mockContactReadRepository,
      [Frozen] IContactWriteRepository mockContactWriteRepository,
      UpdateExternalContactCommand command,
      UpdateExternalContactCommandHandler sut)
    {
        // Arrange

        // Arrange
        var queryableContacts = new List<Entities.Contact>().AsQueryable().BuildMock();
        mockContactReadRepository.Contacts.Returns(queryableContacts);

        var expectedMessage = $"External contact with Id {command.contactDto.Id.Value} not found.";        
        
        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.Multiple
        (
            () => Assert.False(result.IsSuccess),
            () => Assert.Equal(expectedMessage, result.Error),
            () => Assert.Equal(ErrorType.NotFound, result.ErrorType),
            async () => await mockContactWriteRepository.DidNotReceive().UpdateContactAsync(Arg.Any<Entities.Contact>(), Arg.Any<CancellationToken>())
        );
    }

}
