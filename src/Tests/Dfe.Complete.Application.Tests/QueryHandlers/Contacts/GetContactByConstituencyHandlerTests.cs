using AutoFixture.Xunit2;
using AutoMapper;
using Dfe.Complete.Application.Contacts.Models;
using Dfe.Complete.Application.Services.PersonsApi;
using Dfe.Complete.Tests.Common.Customizations.DataAttributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.PersonsApi.Client.Contracts;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project
{
    public class GetContactByConstituencyHandlerTests
    {
        [Theory]
        [CustomAutoData]
        public async Task Handle_ReturnsSuccess_WhenValidConstituency(
            string constituencyName,
            MemberOfParliament mp,
            ConstituencyMemberContactDto dto,
            [Frozen] IConstituenciesClient client,
            [Frozen] IMapper mapper,
            [Frozen] ILogger<GetContactByConstituencyHandler> logger)
        {
            // Arrange
            var request = new GetContactByConstituency(constituencyName);
            var cancellationToken = CancellationToken.None;

            client.GetMemberOfParliamentByConstituencyAsync(constituencyName, cancellationToken)
                  .Returns(mp);

            mapper.Map<ConstituencyMemberContactDto>(mp)
                  .Returns(dto);

            var handler = new GetContactByConstituencyHandler(client, mapper, logger);

            // Act
            var result = await handler.Handle(request, cancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(dto, result.Value);
        }        

        [Theory]
        [CustomAutoData]
        public async Task Handle_ReturnsFailure_WhenPersonsApiExceptionThrown(
            string constituencyName,
            PersonsApiException exception,
            [Frozen] IConstituenciesClient client,
            [Frozen] IMapper mapper,
            [Frozen] ILogger<GetContactByConstituencyHandler> logger)
        {
            var request = new GetContactByConstituency(constituencyName);

            client.GetMemberOfParliamentByConstituencyAsync(constituencyName, Arg.Any<CancellationToken>())
                  .Throws(exception);

            var handler = new GetContactByConstituencyHandler(client, mapper, logger);

            var result = await handler.Handle(request, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Contains("Persons API client", result.Error);           
        }

        [Theory]
        [CustomAutoData]
        public async Task Handle_ReturnsFailure_WhenAggregateExceptionThrown(
            string constituencyName,
            AggregateException exception,
            [Frozen] IConstituenciesClient client,
            [Frozen] IMapper mapper,
            [Frozen] ILogger<GetContactByConstituencyHandler> logger)
        {
            var request = new GetContactByConstituency(constituencyName);

            client.GetMemberOfParliamentByConstituencyAsync(constituencyName, Arg.Any<CancellationToken>())
                  .Throws(exception);

            var handler = new GetContactByConstituencyHandler(client, mapper, logger);

            var result = await handler.Handle(request, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal("An error occurred.", result.Error);
            logger.Received().LogError(exception, "An error occurred.");
        }

        [Theory]
        [CustomAutoData]
        public async Task Handle_ReturnsFailure_WhenUnexpectedExceptionThrown(
            string constituencyName,
            InvalidOperationException exception,
            [Frozen] IConstituenciesClient client,
            [Frozen] IMapper mapper,
            [Frozen] ILogger<GetContactByConstituencyHandler> logger)
        {
            var request = new GetContactByConstituency(constituencyName);

            client.GetMemberOfParliamentByConstituencyAsync(constituencyName, Arg.Any<CancellationToken>())
                  .Throws(exception);

            var handler = new GetContactByConstituencyHandler(client, mapper, logger);

            var result = await handler.Handle(request, CancellationToken.None);

            var expectedMessage = $"An unexpected error occurred. Response: {exception.Message}";

            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Error);            
        }
    }

}
