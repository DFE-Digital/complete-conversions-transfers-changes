using AutoFixture.Xunit2;
using AutoMapper;
using Dfe.Complete.Application.Contacts.Models;
using Dfe.Complete.Application.Contacts.Queries;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using NSubstitute;
using System.Linq.Expressions; 

namespace Dfe.Complete.Application.Tests.QueryHandlers.Contacts
{
    public class GetKeyContactsForProjectTests
    {
        [Theory]
        [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldReturnMatchingContacts_WhenLocalAuthorityIdIsValid(
        [Frozen] ICompleteRepository<KeyContact> mockKeyContactRepo,
        [Frozen] IMapper mockMapper,
        GetKeyContactsForProject handler,
        GetKeyContactsForProjectQuery query,
        KeyContact keyContact,
        KeyContactsDto keyContactsDto)
        {
            // Arrange
            mockKeyContactRepo
                .FindAsync(Arg.Any<Expression<Func<KeyContact, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(keyContact);
            mockMapper.Map<KeyContactsDto>(Arg.Any<KeyContact>()).Returns(keyContactsDto);

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.True(result.IsSuccess);
            Assert.Equal(keyContactsDto.Id, result.Value.Id);
            Assert.Equal(keyContactsDto.HeadteacherId, result.Value.HeadteacherId);
            Assert.Equal(keyContactsDto.IncomingTrustCeoId, result.Value.IncomingTrustCeoId);
            Assert.Equal(keyContactsDto.OutgoingTrustCeoId, result.Value.OutgoingTrustCeoId);
        }
    }
}
