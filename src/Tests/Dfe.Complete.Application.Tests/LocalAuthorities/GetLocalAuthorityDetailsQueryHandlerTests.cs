using Dfe.Complete.Application.LocalAuthorities.Models;
using Dfe.Complete.Application.LocalAuthorities.Queries;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;

namespace Dfe.Complete.Application.Tests.LocalAuthorities
{
    public class GetLocalAuthorityDetailsQueryHandlerTests
    {
        private readonly Mock<ILocalAuthoritiesQueryService> _mockLocalAuthoritiesQueryService;
        private readonly Mock<ILogger<GetLocalAuthorityDetailsQueryHandler>> _mockLogger;
        private readonly GetLocalAuthorityDetailsQueryHandler _handler;

        public GetLocalAuthorityDetailsQueryHandlerTests()
        {
            _mockLocalAuthoritiesQueryService = new Mock<ILocalAuthoritiesQueryService>();
            _mockLogger = new Mock<ILogger<GetLocalAuthorityDetailsQueryHandler>>();
            _handler = new GetLocalAuthorityDetailsQueryHandler(
                _mockLocalAuthoritiesQueryService.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnLocalAuthorityDetailsSuccessfully()
        {
            var query = new GetLocalAuthorityDetailsQuery(new LocalAuthorityId(Guid.NewGuid()));
            var localAuthorityDetails = new LocalAuthorityDetailsModel { Contact = new ContactDetailsModel(), LocalAuthority = new LocalAuthorityDto() };

            _mockLocalAuthoritiesQueryService
                .Setup(service => service.GetLocalAuthorityDetailsAsync(query.LocalAuthorityId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(localAuthorityDetails);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.NotNull(result.Value.LocalAuthority);
            Assert.NotNull(localAuthorityDetails.Contact);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenLocalAuthorityDetailsNotFound()
        {
            var query = new GetLocalAuthorityDetailsQuery(new LocalAuthorityId(Guid.NewGuid()));

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal($"No local authority detail found for Id: {query.LocalAuthorityId.Value}.", result.Error);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenExceptionOccurs()
        {
            var query = new GetLocalAuthorityDetailsQuery(new LocalAuthorityId(Guid.NewGuid()));

            _mockLocalAuthoritiesQueryService
                .Setup(service => service.GetLocalAuthorityDetailsAsync(query.LocalAuthorityId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal("Database error", result.Error);
        }
    }

}
