using AutoFixture.Xunit2;
using AutoMapper;
using Dfe.Complete.Application.Contacts.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Linq.Expressions;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project
{
    public class GetContactByIdQueryHandlerTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldReturnSuccess_WhenContactIsFound(
            [Frozen] ICompleteRepository<Contact> mockContactRepository,
            [Frozen] IMapper mockMapper,
            GetContactIdQueryHandler handler,
            Contact contact,
            ContactDto mappedContact)
        {
            // Arrange
            mockContactRepository.GetAsync(Arg.Any<Expression<Func<Contact, bool>>>())
               .Returns(contact);

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
           [Frozen] ICompleteRepository<Contact> mockContactRepository,
           GetContactIdQueryHandler handler,
           Contact contact)
        {
            // Arrange
            var expectedMessage = $"Error occurred while getting external contact with Id {contact.Id}.";
            mockContactRepository.GetAsync(Arg.Any<Expression<Func<Contact, bool>>>())
                .Throws(new Exception(expectedMessage));

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
