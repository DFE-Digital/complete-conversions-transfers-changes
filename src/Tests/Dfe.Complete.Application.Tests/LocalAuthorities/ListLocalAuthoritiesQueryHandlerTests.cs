using Dfe.Complete.Application.LocalAuthorities.Models;
using Dfe.Complete.Application.LocalAuthorities.Queries;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;

namespace Dfe.Complete.Application.Tests.LocalAuthorities
{
    public class ListLocalAuthoritiesQueryHandlerTests
    {
        private readonly Mock<ILocalAuthoritiesQueryService> _mockLocalAuthoritiesQueryService;
        private readonly Mock<ILogger<ListLocalAuthoritiesQueryHandler>> _mockLogger;
        private readonly ListLocalAuthoritiesQueryHandler _handler;

        public ListLocalAuthoritiesQueryHandlerTests()
        {
            _mockLocalAuthoritiesQueryService = new Mock<ILocalAuthoritiesQueryService>();
            _mockLogger = new Mock<ILogger<ListLocalAuthoritiesQueryHandler>>();
            _handler = new ListLocalAuthoritiesQueryHandler(
                _mockLocalAuthoritiesQueryService.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnPaginatedResultSuccessfully()
        {
            var query = new ListLocalAuthoritiesQuery { Page = 0, Count = 10 };
            var localAuthorities = new List<LocalAuthorityQueryModel>
        {
            new() { Id = new LocalAuthorityId(Guid.NewGuid()), Name = "Authority1" },
            new() { Id = new LocalAuthorityId(Guid.NewGuid()), Name = "Authority2" }
        };

            var mockQueryable = localAuthorities.AsQueryable().BuildMock();

            _mockLocalAuthoritiesQueryService
                .Setup(service => service.ListAllLocalAuthorities(null))
                .Returns(mockQueryable);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(localAuthorities.Count, result.ItemCount);
            Assert.Equal(localAuthorities, result.Value);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenExceptionOccurs()
        {
            var query = new ListLocalAuthoritiesQuery{ Page = 1, Count = 10 };

            _mockLocalAuthoritiesQueryService
                .Setup(service => service.ListAllLocalAuthorities(null))
                .Throws(new Exception("Database error"));

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal("Database error", result.Error); 
        }
    }

}
