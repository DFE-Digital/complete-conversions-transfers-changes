using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Notify;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Notify.Exceptions;
using Notify.Interfaces;
using Notify.Models;
using Notify.Models.Responses;
using Xunit;

namespace Dfe.Complete.Application.Tests.Notify
{
    /// <summary>
    /// Comprehensive unit tests for NotifyEmailSender covering all functionality
    /// </summary>
    public class NotifyEmailSenderTests
    {
        private readonly Mock<ITemplateIdProvider> _templateProviderMock;
        private readonly Mock<ILogger<NotifyEmailSender>> _loggerMock;
        private readonly NotifyOptions _options;

        public NotifyEmailSenderTests()
        {
            _templateProviderMock = new Mock<ITemplateIdProvider>();
            _loggerMock = new Mock<ILogger<NotifyEmailSender>>();
            _options = new NotifyOptions
            {
                ApiKey = "test-key",
                TestMode = false,
                TestModeAllowedDomains = new List<string> { "@education.gov.uk" },
                Email = new NotifyOptions.EmailOptions
                {
                    Templates = new Dictionary<string, string>
                    {
                        { "TestTemplate", "template-id-123" }
                    }
                },
                Retry = new NotifyOptions.RetryOptions
                {
                    MaxRetryAttempts = 3,
                    BaseDelaySeconds = 2
                }
            };
        }

        /// <summary>
        /// Fake IAsyncNotificationClient implementation for testing
        /// Allows configurable behavior for different test scenarios
        /// </summary>
        private class FakeNotifyClient : IAsyncNotificationClient
        {
            public Func<string, string, Dictionary<string, dynamic>?, string?, string?, Task<EmailNotificationResponse>>? SendEmailAsyncHandler { get; set; }
            public int CallCount { get; private set; }
            public string? CapturedReference { get; private set; }

            public Task<EmailNotificationResponse> SendEmailAsync(
                string emailAddress,
                string templateId,
                Dictionary<string, dynamic>? personalisation = null,
                string? clientReference = null,
                string? emailReplyToId = null)
            {
                CallCount++;
                CapturedReference = clientReference;

                if (SendEmailAsyncHandler != null)
                {
                    return SendEmailAsyncHandler(emailAddress, templateId, personalisation, clientReference, emailReplyToId);
                }

                return Task.FromResult(new EmailNotificationResponse
                {
                    id = "default-msg-id",
                    reference = clientReference ?? "default-ref"
                });
            }

            // New overload with oneClickUnsubscribeURL parameter (from newer version of GOV.UK Notify)
            public Task<EmailNotificationResponse> SendEmailAsync(
                string emailAddress,
                string templateId,
                Dictionary<string, dynamic>? personalisation = null,
                string? clientReference = null,
                string? emailReplyToId = null,
                string? oneClickUnsubscribeURL = null)
            {
                // Delegate to the main implementation, ignoring oneClickUnsubscribeURL
                return SendEmailAsync(emailAddress, templateId, personalisation, clientReference, emailReplyToId);
            }

            // Not used in current tests - implement as stubs
            public Task<SmsNotificationResponse> SendSmsAsync(string mobileNumber, string templateId, Dictionary<string, dynamic>? personalisation = null, string? clientReference = null, string? smsSenderId = null)
                => throw new NotImplementedException();

            public Task<LetterNotificationResponse> SendLetterAsync(string templateId, Dictionary<string, dynamic> personalisation, string? clientReference = null)
                => throw new NotImplementedException();

            public Task<LetterNotificationResponse> SendPrecompiledLetterAsync(string clientReference, byte[] pdfContents, string? postage = null)
                => throw new NotImplementedException();

            public Task<Notification> GetNotificationByIdAsync(string notificationId)
                => throw new NotImplementedException();

            public Task<NotificationList> GetNotificationsAsync(string? templateType = null, string? status = null, string? reference = null, string? olderThanId = null, bool includeSpreadsheetUploads = false)
                => throw new NotImplementedException();

            public Task<TemplateResponse> GetTemplateByIdAsync(string templateId)
                => throw new NotImplementedException();

            public Task<TemplateResponse> GetTemplateByIdAndVersionAsync(string templateId, int version = 0)
                => throw new NotImplementedException();

            public Task<TemplateList> GetAllTemplatesAsync(string? templateType = null)
                => throw new NotImplementedException();

            public Task<TemplatePreviewResponse> GenerateTemplatePreviewAsync(string templateId, Dictionary<string, dynamic>? personalisation = null)
                => throw new NotImplementedException();

            public Task<ReceivedTextListResponse> GetReceivedTextsAsync(string? olderThanId = null)
                => throw new NotImplementedException();

            // IBaseClient members
            public Task<string> GET(string url)
                => throw new NotImplementedException();

            public Task<string> POST(string url, string json)
                => throw new NotImplementedException();

            public Task<string> MakeRequest(string path, HttpMethod method, HttpContent content)
                => throw new NotImplementedException();

            public Tuple<string, string> ExtractServiceIdAndApiKey(string fromApiKey)
                => throw new NotImplementedException();

            public Uri ValidateBaseUri(string baseUrl)
                => throw new NotImplementedException();

            public string GetUserAgent()
                => throw new NotImplementedException();
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithNullNotifyClient_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new NotifyEmailSender(null!, _templateProviderMock.Object, _loggerMock.Object, Options.Create(_options)));
        }

        [Fact]
        public void Constructor_WithNullTemplateProvider_ThrowsArgumentNullException()
        {
            // Arrange
            var fakeClient = new FakeNotifyClient();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new NotifyEmailSender(fakeClient, null!, _loggerMock.Object, Options.Create(_options)));
        }

        [Fact]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            var fakeClient = new FakeNotifyClient();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new NotifyEmailSender(fakeClient, _templateProviderMock.Object, null!, Options.Create(_options)));
        }

        [Fact]
        public void Constructor_WithNullOptions_ThrowsArgumentNullException()
        {
            // Arrange
            var fakeClient = new FakeNotifyClient();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new NotifyEmailSender(fakeClient, _templateProviderMock.Object, _loggerMock.Object, null!));
        }

        #endregion

        #region SendAsync Tests

        [Fact]
        public async Task SendAsync_WithNullMessage_ThrowsArgumentNullException()
        {
            // Arrange
            var fakeClient = new FakeNotifyClient();
            var sender = new NotifyEmailSender(
                fakeClient,
                _templateProviderMock.Object,
                _loggerMock.Object,
                Options.Create(_options));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                sender.SendAsync(null!, CancellationToken.None));
        }

        [Fact]
        public async Task SendAsync_WithValidMessage_SendsEmailSuccessfully()
        {
            // Arrange
            var message = new EmailMessage(
                To: EmailAddress.Create("test@education.gov.uk"),
                TemplateKey: "TestTemplate",
                Personalisation: new Dictionary<string, string> { { "name", "Test" } },
                Reference: "test-ref"
            );

            _templateProviderMock
                .Setup(x => x.GetTemplateId("TestTemplate"))
                .Returns("template-id-123");

            var fakeClient = new FakeNotifyClient
            {
                SendEmailAsyncHandler = (email, template, personalisation, reference, replyTo) =>
                    Task.FromResult(new EmailNotificationResponse
                    {
                        id = "notify-msg-id",
                        reference = "test-ref"
                    })
            };

            var sender = new NotifyEmailSender(
                fakeClient,
                _templateProviderMock.Object,
                _loggerMock.Object,
                Options.Create(_options));

            // Act
            var result = await sender.SendAsync(message, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("notify-msg-id", result.Value.ProviderMessageId);
            Assert.Equal("test-ref", result.Value.Reference);
        }

        [Fact]
        public async Task SendAsync_WithTemplateNotFound_ReturnsFailure()
        {
            // Arrange
            var message = new EmailMessage(
                To: EmailAddress.Create("test@education.gov.uk"),
                TemplateKey: "NonExistentTemplate",
                Personalisation: new Dictionary<string, string>(),
                Reference: null
            );

            _templateProviderMock
                .Setup(x => x.GetTemplateId("NonExistentTemplate"))
                .Throws(new KeyNotFoundException("Template not found"));

            var fakeClient = new FakeNotifyClient();
            var sender = new NotifyEmailSender(
                fakeClient,
                _templateProviderMock.Object,
                _loggerMock.Object,
                Options.Create(_options));

            // Act
            var result = await sender.SendAsync(message, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.NotFound, result.ErrorType);
            Assert.Equal("Email template not found", result.Error);
        }

        [Fact]
        public async Task SendAsync_InTestModeWithDisallowedDomain_BlocksEmail()
        {
            // Arrange
            var testOptions = new NotifyOptions
            {
                ApiKey = "test-key",
                TestMode = true,
                TestModeAllowedDomains = new List<string> { "@education.gov.uk" },
                Email = new NotifyOptions.EmailOptions { Templates = new Dictionary<string, string>() },
                Retry = new NotifyOptions.RetryOptions { MaxRetryAttempts = 0 }
            };

            var message = new EmailMessage(
                To: EmailAddress.Create("test@external.com"), // Not allowed in test mode
                TemplateKey: "TestTemplate",
                Personalisation: new Dictionary<string, string>(),
                Reference: null
            );

            var fakeClient = new FakeNotifyClient();
            var sender = new NotifyEmailSender(
                fakeClient,
                _templateProviderMock.Object,
                _loggerMock.Object,
                Options.Create(testOptions));

            // Act
            var result = await sender.SendAsync(message, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.UnprocessableContent, result.ErrorType);
            Assert.Equal("Email blocked in test mode", result.Error);
        }

        [Fact]
        public async Task SendAsync_InTestModeWithAllowedDomain_SendsEmail()
        {
            // Arrange
            var testOptions = new NotifyOptions
            {
                ApiKey = "test-key",
                TestMode = true,
                TestModeAllowedDomains = new List<string> { "@education.gov.uk" },
                Email = new NotifyOptions.EmailOptions { Templates = new Dictionary<string, string>() },
                Retry = new NotifyOptions.RetryOptions { MaxRetryAttempts = 0 }
            };

            var message = new EmailMessage(
                To: EmailAddress.Create("test@education.gov.uk"), // Allowed domain
                TemplateKey: "TestTemplate",
                Personalisation: new Dictionary<string, string>(),
                Reference: "test-ref"
            );

            _templateProviderMock
                .Setup(x => x.GetTemplateId("TestTemplate"))
                .Returns("template-id-123");

            var fakeClient = new FakeNotifyClient
            {
                SendEmailAsyncHandler = (email, template, personalisation, reference, replyTo) =>
                    Task.FromResult(new EmailNotificationResponse
                    {
                        id = "notify-msg-id",
                        reference = "test-ref"
                    })
            };

            var sender = new NotifyEmailSender(
                fakeClient,
                _templateProviderMock.Object,
                _loggerMock.Object,
                Options.Create(testOptions));

            // Act
            var result = await sender.SendAsync(message, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task SendAsync_WithNoReference_GeneratesReference()
        {
            // Arrange
            var message = new EmailMessage(
                To: EmailAddress.Create("test@education.gov.uk"),
                TemplateKey: "TestTemplate",
                Personalisation: new Dictionary<string, string> { { "key", "value" } },
                Reference: null // No reference provided
            );

            _templateProviderMock
                .Setup(x => x.GetTemplateId("TestTemplate"))
                .Returns("template-id-123");

            var fakeClient = new FakeNotifyClient
            {
                SendEmailAsyncHandler = (email, template, personalisation, reference, replyTo) =>
                    Task.FromResult(new EmailNotificationResponse
                    {
                        id = "notify-msg-id",
                        reference = reference!
                    })
            };

            var sender = new NotifyEmailSender(
                fakeClient,
                _templateProviderMock.Object,
                _loggerMock.Object,
                Options.Create(_options));

            // Act
            var result = await sender.SendAsync(message, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(fakeClient.CapturedReference);
            Assert.NotEmpty(fakeClient.CapturedReference);
            Assert.Equal(32, fakeClient.CapturedReference.Length); // SHA256 hash truncated to 32 chars
        }

        [Fact]
        public async Task SendAsync_WithTransientFailure_RetriesAndSucceeds()
        {
            // Arrange
            var message = new EmailMessage(
                To: EmailAddress.Create("test@education.gov.uk"),
                TemplateKey: "TestTemplate",
                Personalisation: new Dictionary<string, string>(),
                Reference: "test-ref"
            );

            _templateProviderMock
                .Setup(x => x.GetTemplateId("TestTemplate"))
                .Returns("template-id-123");

            var fakeClient = new FakeNotifyClient();
            fakeClient.SendEmailAsyncHandler = (email, template, personalisation, reference, replyTo) =>
            {
                if (fakeClient.CallCount < 2)
                {
                    throw new HttpRequestException("Transient network error");
                }
                return Task.FromResult(new EmailNotificationResponse
                {
                    id = "notify-msg-id",
                    reference = "test-ref"
                });
            };

            var sender = new NotifyEmailSender(
                fakeClient,
                _templateProviderMock.Object,
                _loggerMock.Object,
                Options.Create(_options));

            // Act
            var result = await sender.SendAsync(message, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, fakeClient.CallCount); // First failed, second succeeded
        }

        [Fact]
        public async Task SendAsync_WithPermanentFailure_ReturnsFailureImmediately()
        {
            // Arrange
            var message = new EmailMessage(
                To: EmailAddress.Create("test@education.gov.uk"),
                TemplateKey: "TestTemplate",
                Personalisation: new Dictionary<string, string>(),
                Reference: "test-ref"
            );

            _templateProviderMock
                .Setup(x => x.GetTemplateId("TestTemplate"))
                .Returns("template-id-123");

            var fakeClient = new FakeNotifyClient
            {
                SendEmailAsyncHandler = (email, template, personalisation, reference, replyTo) =>
                    throw new NotifyClientException("Status code: 400. Bad request")
            };

            var sender = new NotifyEmailSender(
                fakeClient,
                _templateProviderMock.Object,
                _loggerMock.Object,
                Options.Create(_options));

            // Act
            var result = await sender.SendAsync(message, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(1, fakeClient.CallCount); // No retries for permanent failure
            Assert.Equal(ErrorType.UnprocessableContent, result.ErrorType);
        }

        [Fact]
        public async Task SendAsync_WithRetriesExhausted_ReturnsFailure()
        {
            // Arrange
            var retryOptions = new NotifyOptions
            {
                ApiKey = "test-key",
                TestMode = false,
                Email = new NotifyOptions.EmailOptions { Templates = new Dictionary<string, string>() },
                Retry = new NotifyOptions.RetryOptions
                {
                    MaxRetryAttempts = 2,
                    BaseDelaySeconds = 1
                }
            };

            var message = new EmailMessage(
                To: EmailAddress.Create("test@education.gov.uk"),
                TemplateKey: "TestTemplate",
                Personalisation: new Dictionary<string, string>(),
                Reference: "test-ref"
            );

            _templateProviderMock
                .Setup(x => x.GetTemplateId("TestTemplate"))
                .Returns("template-id-123");

            var fakeClient = new FakeNotifyClient
            {
                SendEmailAsyncHandler = (email, template, personalisation, reference, replyTo) =>
                    throw new HttpRequestException("Network error") // Always fails
            };

            var sender = new NotifyEmailSender(
                fakeClient,
                _templateProviderMock.Object,
                _loggerMock.Object,
                Options.Create(retryOptions));

            // Act
            var result = await sender.SendAsync(message, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(3, fakeClient.CallCount); // Initial + 2 retries = 3 attempts
            Assert.Contains("Email send failed due to a permanent error", result.Error);
        }

        [Fact]
        public async Task SendAsync_WithNotifyServerError_RetriesAndSucceeds()
        {
            // Arrange
            var message = new EmailMessage(
                To: EmailAddress.Create("test@education.gov.uk"),
                TemplateKey: "TestTemplate",
                Personalisation: new Dictionary<string, string>(),
                Reference: "test-ref"
            );

            _templateProviderMock
                .Setup(x => x.GetTemplateId("TestTemplate"))
                .Returns("template-id-123");

            var fakeClient = new FakeNotifyClient();
            fakeClient.SendEmailAsyncHandler = (email, template, personalisation, reference, replyTo) =>
            {
                if (fakeClient.CallCount < 2)
                {
                    throw new NotifyClientException("Status code: 503. Service unavailable");
                }
                return Task.FromResult(new EmailNotificationResponse
                {
                    id = "notify-msg-id",
                    reference = "test-ref"
                });
            };

            var sender = new NotifyEmailSender(
                fakeClient,
                _templateProviderMock.Object,
                _loggerMock.Object,
                Options.Create(_options));

            // Act
            var result = await sender.SendAsync(message, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, fakeClient.CallCount); // First failed with 503, second succeeded
        }

        [Fact]
        public async Task SendAsync_WithNotify404Error_ReturnsNotFoundWithoutRetry()
        {
            // Arrange
            var message = new EmailMessage(
                To: EmailAddress.Create("test@education.gov.uk"),
                TemplateKey: "TestTemplate",
                Personalisation: new Dictionary<string, string>(),
                Reference: "test-ref"
            );

            _templateProviderMock
                .Setup(x => x.GetTemplateId("TestTemplate"))
                .Returns("template-id-123");

            var fakeClient = new FakeNotifyClient
            {
                SendEmailAsyncHandler = (email, template, personalisation, reference, replyTo) =>
                    throw new NotifyClientException("Status code: 404. Template not found")
            };

            var sender = new NotifyEmailSender(
                fakeClient,
                _templateProviderMock.Object,
                _loggerMock.Object,
                Options.Create(_options));

            // Act
            var result = await sender.SendAsync(message, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(1, fakeClient.CallCount); // No retries for 404
            Assert.Equal(ErrorType.NotFound, result.ErrorType);
        }

        [Fact]
        public async Task SendAsync_WithSameMessageTwice_GeneratesSameReference()
        {
            // Arrange
            var message1 = new EmailMessage(
                To: EmailAddress.Create("test@education.gov.uk"),
                TemplateKey: "TestTemplate",
                Personalisation: new Dictionary<string, string> { { "key", "value" } },
                Reference: null
            );

            var message2 = new EmailMessage(
                To: EmailAddress.Create("test@education.gov.uk"),
                TemplateKey: "TestTemplate",
                Personalisation: new Dictionary<string, string> { { "key", "value" } },
                Reference: null
            );

            _templateProviderMock
                .Setup(x => x.GetTemplateId("TestTemplate"))
                .Returns("template-id-123");

            var references = new List<string>();
            var fakeClient = new FakeNotifyClient
            {
                SendEmailAsyncHandler = (email, template, personalisation, reference, replyTo) =>
                {
                    references.Add(reference!);
                    return Task.FromResult(new EmailNotificationResponse
                    {
                        id = "notify-msg-id",
                        reference = reference!
                    });
                }
            };

            var sender = new NotifyEmailSender(
                fakeClient,
                _templateProviderMock.Object,
                _loggerMock.Object,
                Options.Create(_options));

            // Act
            await sender.SendAsync(message1, CancellationToken.None);
            await sender.SendAsync(message2, CancellationToken.None);

            // Assert
            Assert.Equal(2, references.Count);
            Assert.Equal(references[0], references[1]); // Same reference for idempotency
        }

        #endregion
    }
}

