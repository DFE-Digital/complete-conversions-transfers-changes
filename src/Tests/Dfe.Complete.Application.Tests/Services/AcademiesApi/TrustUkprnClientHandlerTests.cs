using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Services.AcademiesApi;
using Microsoft.Extensions.Logging;
using Moq;

namespace Dfe.Complete.Application.Tests.Services.AcademiesApi;

public class TrustUkprnClientHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsSuccess_WhenApiCallSucceeds()
    {
        // Arrange
        const string ukprn = "12345678";
        var expectedTrust = new TrustDto();
        var mockClient = new Mock<ITrustsV4Client>();
        var mockLogger = new Mock<ILogger<TrustUkprnClientHandler>>();

        mockClient
            .Setup(client => client.GetTrustByUkprn2Async(ukprn, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTrust);

        var handler = new TrustUkprnClientHandler(mockClient.Object, mockLogger.Object);
        var request = new GetTrustByUkprnRequest(ukprn);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedTrust, result.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_ThrowsArgumentException_WhenUkprnIsNullOrWhiteSpace(string ukprn)
    {
        // Arrange
        var mockClient = new Mock<ITrustsV4Client>();
        var mockLogger = new Mock<ILogger<TrustUkprnClientHandler>>();
        var handler = new TrustUkprnClientHandler(mockClient.Object, mockLogger.Object);
        var request = new GetTrustByUkprnRequest(ukprn);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(request, CancellationToken.None));
        Assert.Equal("Ukprn cannot be null or empty.", ex.Message);
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenAcademiesApiExceptionThrown()
    {
        // Arrange
        var ukprn = "12345678";
        var exceptionMessage = "API error occurred";
        // Simulate AcademiesApiException (assuming it's defined in Dfe.AcademiesApi.Client.Contracts)
        var academiesApiException = new AcademiesApiException(exceptionMessage, 500, string.Empty, null, null);

        var mockClient = new Mock<ITrustsV4Client>();
        var mockLogger = new Mock<ILogger<TrustUkprnClientHandler>>();

        mockClient
            .Setup(client => client.GetTrustByUkprn2Async(ukprn, It.IsAny<CancellationToken>()))
            .ThrowsAsync(academiesApiException);

        var handler = new TrustUkprnClientHandler(mockClient.Object, mockLogger.Object);
        var request = new GetTrustByUkprnRequest(ukprn);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("An error occurred with the Academies API client", result.Error);
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenAggregateExceptionThrown()
    {
        // Arrange
        var urn = "123456";
        var aggregateException = new AggregateException();

        var mockClient = new Mock<ITrustsV4Client>();
        var mockLogger = new Mock<ILogger<TrustUkprnClientHandler>>();

        mockClient
            .Setup(client => client.GetTrustByUkprn2Async(urn, It.IsAny<CancellationToken>()))
            .ThrowsAsync(aggregateException);

        var handler = new TrustUkprnClientHandler(mockClient.Object, mockLogger.Object);
        var request = new GetTrustByUkprnRequest(urn);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        var expectedError = "An error occurred.";
        Assert.False(result.IsSuccess);
        Assert.Equal(expectedError, result.Error);
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenUnhandledExceptionThrown()
    {
        // Arrange
        var ukprn = "12345678";
        var exceptionMessage = "Unhandled error";
        var unhandledException = new Exception(exceptionMessage);

        var mockClient = new Mock<ITrustsV4Client>();
        var mockLogger = new Mock<ILogger<TrustUkprnClientHandler>>();

        mockClient
            .Setup(client => client.GetTrustByUkprn2Async(ukprn, It.IsAny<CancellationToken>()))
            .ThrowsAsync(unhandledException);

        var handler = new TrustUkprnClientHandler(mockClient.Object, mockLogger.Object);
        var request = new GetTrustByUkprnRequest(ukprn);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(exceptionMessage, result.Error);
    }
}