using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notify.Client;
using Notify.Exceptions;
using Notify.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Dfe.Complete.Infrastructure.Notify
{
    /// <summary>
    /// GOV.UK Notify implementation of IEmailSender with manual retry logic.
    /// Sends emails through GOV.UK Notify API with automatic retries on transient failures.
    /// </summary>
    public class NotifyEmailSender : IEmailSender
    {
        private readonly IAsyncNotificationClient _notifyClient;
        private readonly ITemplateIdProvider _templateProvider;
        private readonly ILogger<NotifyEmailSender> _logger;
        private readonly NotifyOptions _options;

        public NotifyEmailSender(
            IAsyncNotificationClient notifyClient,
            ITemplateIdProvider templateProvider,
            ILogger<NotifyEmailSender> logger,
            IOptions<NotifyOptions> options)
        {
            _notifyClient = notifyClient ?? throw new ArgumentNullException(nameof(notifyClient));
            _templateProvider = templateProvider ?? throw new ArgumentNullException(nameof(templateProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Sends an email asynchronously with automatic retry on transient failures.
        /// </summary>
        public async Task<Result<EmailSendResult>> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            // Validate test mode first
            if (_options.TestMode && !IsAllowedTestDomain(message.To.Value))
            {
                _logger.LogWarning(
                    "Email blocked in test mode. Template: {TemplateKey}, Domain: {Domain}",
                    message.TemplateKey, ExtractDomain(message.To.Value));
                
                return Result<EmailSendResult>.Failure(
                    "Email blocked in test mode",
                    ErrorType.UnprocessableContent);
            }

            // Generate reference for idempotency if not provided
            var reference = message.Reference ?? GenerateReference(message);

            // Get template ID
            try
            {
                var templateId = _templateProvider.GetTemplateId(message.TemplateKey);

                // Call retry logic
                return await SendWithRetryAsync(
                    message.To.Value,
                    templateId,
                    message.Personalisation,
                    reference,
                    message.ReplyToId,
                    message.TemplateKey,
                    cancellationToken);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Template not found: {TemplateKey}", message.TemplateKey);
                return Result<EmailSendResult>.Failure(
                    $"Template not found: {message.TemplateKey}",
                    ErrorType.NotFound);
            }
        }

        /// <summary>
        /// Sends email with manual retry loop (no external library).
        /// Implements exponential backoff with configurable retry attempts.
        /// </summary>
        private async Task<Result<EmailSendResult>> SendWithRetryAsync(
            string emailAddress,
            string templateId,
            IReadOnlyDictionary<string, string> personalisation,
            string reference,
            string? replyToId,
            string templateKey,
            CancellationToken cancellationToken)
        {
            var maxRetries = _options.Retry.MaxRetryAttempts;
            Exception? lastException = null;

            // MANUAL RETRY LOOP - Simple for loop, no Polly
            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    // Convert personalisation to Dictionary<string, dynamic> for Notify API
                    var personalisationDict = personalisation?.ToDictionary(
                        kvp => kvp.Key,
                        kvp => (dynamic)kvp.Value) ?? new Dictionary<string, dynamic>();

                    // Send email via Notify API
                    var response = await _notifyClient.SendEmailAsync(
                        emailAddress,
                        templateId,
                        personalisationDict,
                        reference,
                        replyToId);

                    // SUCCESS - log and return immediately
                    _logger.LogInformation(
                        "Email sent successfully. Template: {TemplateKey}, Reference: {Reference}, MessageId: {MessageId}, Attempt: {Attempt}",
                        templateKey, reference, response.id, attempt + 1);

                    return Result<EmailSendResult>.Success(new EmailSendResult(
                        ProviderMessageId: response.id,
                        Reference: reference,
                        SentAt: DateTime.UtcNow));
                }
                catch (Exception ex) when (IsTransientFailure(ex) && attempt < maxRetries)
                {
                    // TRANSIENT FAILURE - wait and retry
                    lastException = ex;
                    var delay = CalculateDelay(attempt, _options.Retry.BaseDelaySeconds);

                    _logger.LogWarning(
                        "Email send failed (transient). Attempt: {Attempt}/{MaxAttempts}, Retrying in {Delay}ms. Template: {TemplateKey}, Reference: {Reference}, Error: {Error}",
                        attempt + 1, maxRetries + 1, delay.TotalMilliseconds, templateKey, reference, ex.Message);

                    // Wait before retrying (exponential backoff)
                    await Task.Delay(delay, cancellationToken);

                    // Loop continues to next attempt
                }
                catch (Exception ex)
                {
                    // PERMANENT FAILURE - don't retry, return error immediately
                    _logger.LogError(
                        ex,
                        "Email send failed (permanent). Attempt: {Attempt}, Template: {TemplateKey}, Reference: {Reference}",
                        attempt + 1, templateKey, reference);

                    return Result<EmailSendResult>.Failure(
                        $"Email send failed: {ex.Message}",
                        MapToErrorType(ex));
                }
            }

            // ALL RETRIES EXHAUSTED - return failure
            _logger.LogError(
                "Email send failed after {MaxAttempts} attempts. Template: {TemplateKey}, Reference: {Reference}, LastError: {Error}",
                maxRetries + 1, templateKey, reference, lastException?.Message);

            return Result<EmailSendResult>.Failure(
                $"Email send failed after {maxRetries + 1} attempts: {lastException?.Message}",
                ErrorType.Unknown);
        }

        /// <summary>
        /// Determines if an exception represents a transient failure that should be retried.
        /// </summary>
        private bool IsTransientFailure(Exception ex)
        {
            // Network errors - retry
            if (ex is HttpRequestException)
                return true;

            // Timeouts - retry
            if (ex is TaskCanceledException)
                return true;

            // Notify API 5xx errors - retry
            if (ex is NotifyClientException notifyEx)
            {
                var statusCode = GetStatusCodeFromNotifyException(notifyEx);
                if (statusCode >= 500 && statusCode < 600)
                    return true;
            }

            // All other exceptions - don't retry (permanent failures)
            return false;
        }

        /// <summary>
        /// Extracts HTTP status code from Notify exception message.
        /// </summary>
        private int GetStatusCodeFromNotifyException(NotifyClientException ex)
        {
            // Notify exceptions typically contain status code in the message
            // Example: "Status code: 500. Error response: ..."
            var message = ex.Message;
            var statusMatch = System.Text.RegularExpressions.Regex.Match(message, @"Status code:\s*(\d+)");
            if (statusMatch.Success && int.TryParse(statusMatch.Groups[1].Value, out var code))
            {
                return code;
            }
            return 0;
        }

        /// <summary>
        /// Calculates exponential backoff delay for retry attempts.
        /// </summary>
        private TimeSpan CalculateDelay(int attempt, int baseDelaySeconds)
        {
            // Exponential backoff: attempt 0 = 2s, attempt 1 = 4s, attempt 2 = 8s
            var delaySeconds = baseDelaySeconds * Math.Pow(2, attempt);
            return TimeSpan.FromSeconds(delaySeconds);
        }

        /// <summary>
        /// Generates a deterministic reference for idempotency.
        /// </summary>
        private string GenerateReference(EmailMessage message)
        {
            // Hash of template key + email + personalisation keys for idempotency
            var content = $"{message.TemplateKey}|{message.To.Value}|{string.Join(",", message.Personalisation.Keys.OrderBy(k => k))}";
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(content));
            return Convert.ToHexString(hash)[..32]; // 32 char hex string
        }

        /// <summary>
        /// Maps exceptions to ErrorType enum.
        /// </summary>
        private ErrorType MapToErrorType(Exception ex)
        {
            if (ex is NotifyClientException notifyEx)
            {
                var statusCode = GetStatusCodeFromNotifyException(notifyEx);
                if (statusCode == 404)
                    return ErrorType.NotFound;
                if (statusCode >= 400 && statusCode < 500)
                    return ErrorType.UnprocessableContent;
                if (statusCode >= 500)
                    return ErrorType.Unknown;
            }

            if (ex is HttpRequestException)
                return ErrorType.Unknown;

            if (ex is TaskCanceledException)
                return ErrorType.Unknown;

            return ErrorType.Unknown;
        }

        /// <summary>
        /// Checks if email domain is allowed in test mode.
        /// </summary>
        private bool IsAllowedTestDomain(string email)
        {
            var domain = ExtractDomain(email);
            return _options.TestModeAllowedDomains.Any(allowedDomain =>
                domain.EndsWith(allowedDomain.TrimStart('@'), StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Extracts domain from email address.
        /// </summary>
        private string ExtractDomain(string email)
        {
            var atIndex = email.IndexOf('@');
            return atIndex >= 0 ? email.Substring(atIndex) : string.Empty;
        }
    }
}

