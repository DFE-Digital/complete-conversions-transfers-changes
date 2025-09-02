using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Contacts.Commands;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using Moq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Dfe.Complete.Application.Tests.Commands.ExternalContact;   

public class CreateExternalContactCommandTests
{
    //[Theory]
    //[CustomAutoData(typeof(DateOnlyCustomization),
    //    typeof(IgnoreVirtualMembersCustomisation))]
    //public async Task Handle_ShouldCreateExternalContactSuccessfully (
    //    [Frozen] ICompleteRepository<Contact> mockContactRepository,
    //    IFixture fixture,
    //    CreateExternalContactCommand command,
    //    CreateExternalContactCommandHandler sut
    //    )
    //{        
    //    // Arrange
    //    var now = DateTime.UtcNow;

    //    var contact = fixture.Build<Contact>()
    //        .With(q => q.Id, new ContactId(Guid.NewGuid()))
    //        .Create();

    //    mockContactRepository.AddAsync(Arg.Any<Contact>(), Arg.Any<CancellationToken>()).Returns(contact);

    //    // Act
    //    var contactId = await sut.Handle(command, CancellationToken.None);

    //    // Assert
    //    Assert.NotNull(contactId);
    //    Assert.IsType<ContactId>(contactId);
    //    await mockContactRepository.Received(1).AddAsync(Arg.Any<Contact>(), Arg.Any<CancellationToken>());
    //}


    //[Theory]
    //[CustomAutoData(typeof(IgnoreVirtualMembersCustomisation))]
    //public async Task Handle_ShouldReturnFailure_WhenExceptionOccurs(
    //  [Frozen] ICompleteRepository<Contact> mockContactRepository,
    //    CreateExternalContactCommand command,
    //    CreateExternalContactCommandHandler sut)
    //{
    //    // Arrange
    //    var now = DateTime.UtcNow;              

    //    mockContactRepository.AddAsync(Arg.Any<Contact>(), Arg.Any<CancellationToken>())
    //       .ThrowsAsync(new Exception("Exception on creating contact"));
        
    //    // Act and Assert
    //    var exception = await Assert.ThrowsAsync<Exception>(() => sut.Handle(command, CancellationToken.None));
    //    Assert.NotNull(exception);
    //    Assert.Equal("Exception on creating contact", exception.Message);
    //}

}
