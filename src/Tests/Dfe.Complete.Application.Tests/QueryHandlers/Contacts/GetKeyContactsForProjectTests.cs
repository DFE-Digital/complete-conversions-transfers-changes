using AutoFixture.Xunit2;
using AutoMapper;
using Dfe.Complete.Application.Contacts.Models;
using Dfe.Complete.Application.Contacts.Queries;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using MockQueryable;
using NSubstitute;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Contacts
{
    public class GetKeyContactsForProjectTests
    {
        [Theory]
        [CustomAutoData(typeof(IgnoreVirtualMembersCustomisation))]
        public async Task Handle_ShouldReturnMatchingContacts_WhenLocalAuthorityIdIsValid(
        [Frozen] IKeyContactReadRepository mockKeyContactRepo,
        [Frozen] IMapper mockMapper,
        GetKeyContactsForProjectQueryHandler handler,
        GetKeyContactsForProjectQuery query,
        KeyContact keyContact)
        {
            // Arrange
            keyContact.ProjectId = query.ProjectId;
            var mockQueryable = new[] { keyContact }.AsQueryable().BuildMock();
            mockKeyContactRepo
                .KeyContacts.Returns(mockQueryable);
            var keycontactDto = new KeyContactDto
            {
                Id = keyContact.Id,
                HeadteacherId = keyContact.HeadteacherId,
                IncomingTrustCeoId = keyContact.IncomingTrustCeoId,
                OutgoingTrustCeoId = keyContact.OutgoingTrustCeoId
            };
            mockMapper.Map<KeyContactDto>(Arg.Any<KeyContact>()).Returns(keycontactDto);

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.True(result.IsSuccess);
            Assert.Equal(keycontactDto.Id, result.Value.Id);
            Assert.Equal(keycontactDto.HeadteacherId, result.Value.HeadteacherId);
            Assert.Equal(keycontactDto.IncomingTrustCeoId, result.Value.IncomingTrustCeoId);
            Assert.Equal(keycontactDto.OutgoingTrustCeoId, result.Value.OutgoingTrustCeoId);
        }
    }
}
