using AutoFixture.Xunit2;
using AutoMapper;
using Dfe.Complete.Application.Contacts.Interfaces;
using Dfe.Complete.Application.Contacts.Models;
using Dfe.Complete.Application.Contacts.Queries;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project
{
    public class GetContactByIdQueryHandlerTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldReturnSuccess_WhenContactIsFound(
            [Frozen] IContactReadRepository mockContactRepository,
            [Frozen] IMapper mockMapper,
            GetContactIdQueryHandler handler,
            Contact contact,
            ContactDto mappedContact)
        {
            // Arrange
            var queryableContacts = new List<Contact> { contact }.AsQueryable().BuildMock();
            mockContactRepository.Contacts.Returns(queryableContacts);

            mockMapper.Map<ContactDto>(Arg.Any<Contact>()).Returns(mappedContact);

            var query = new GetContactByIdQuery(contact.Id); 

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Multiple
            (
                () => Assert.True(result.IsSuccess),
                () => Assert.NotNull(result.Value),
                () => Assert.Equal(mappedContact, result.Value)
            );
        }
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldReturnFailure_WhenRepositoryThrowsException(
           [Frozen] IContactReadRepository mockContactRepository,
           GetContactIdQueryHandler handler,
           Contact contact)
        {
            // Arrange
            var expectedMessage = $"Error occurred while getting external contact with Id {contact.Id}.";
            mockContactRepository.Contacts.Throws(new Exception(expectedMessage));

            var query = new GetContactByIdQuery(contact.Id);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Multiple
            (
                () => Assert.False(result.IsSuccess),
                () => Assert.Equal(expectedMessage, result.Error)
            );
        }
    }
}
