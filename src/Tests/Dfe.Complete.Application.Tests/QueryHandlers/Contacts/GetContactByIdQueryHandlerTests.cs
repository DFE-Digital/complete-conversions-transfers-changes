using AutoFixture.Xunit2;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
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
            GetContactIdQueryHandler handler,
            Contact contact)
        {
            // Arrange
            mockContactRepository.GetAsync(Arg.Any<Expression<Func<Contact, bool>>>())
               .Returns(contact);

            var query = new GetContactByIdQuery(contact.Id); 

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(contact, result.Value);
        }
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldReturnFailure_WhenRepositoryThrowsException(
           [Frozen] ICompleteRepository<Contact> mockContactRepository,
           GetContactIdQueryHandler handler,
           Domain.Entities.Contact contact)
        {
            // Arrange
            var expectedMessage = "Repository error";
            mockContactRepository.GetAsync(Arg.Any<Expression<Func<Contact, bool>>>())
                .Throws(new Exception(expectedMessage));

            var query = new GetContactByIdQuery(contact.Id);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Error);
        }
    }
}
