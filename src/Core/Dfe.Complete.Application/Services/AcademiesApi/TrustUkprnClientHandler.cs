using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Cache;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Extensions;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Services.AcademiesApi
{
    public record GetTrustByUkprnRequest(string Ukprn) : IRequest<Result<TrustDto?>>;

    public class TrustUkprnClientHandler(
        ITrustsV4Client trustsV4Client,
        ILogger<TrustUkprnClientHandler> logger,
        IDistributedCache cache)
        : IRequestHandler<GetTrustByUkprnRequest, Result<TrustDto?>>
    {
        private readonly ITrustsV4Client _trustsV4Client = trustsV4Client ?? throw new ArgumentNullException(nameof(trustsV4Client));
        private readonly ILogger<TrustUkprnClientHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IDistributedCache _cache = cache;

        public async Task<Result<TrustDto?>> Handle(GetTrustByUkprnRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Ukprn))
                throw new ArgumentException("Ukprn cannot be null or empty.");

            try
            {
                var cacheKey = $"-trust-{request.Ukprn}";

                var result = await _cache.GetOrSetAsync<TrustDto>(
                cacheKey,
                async () =>
                {
                    return await _trustsV4Client
                    .GetTrustByUkprn2Async(request.Ukprn, cancellationToken)
                    .ConfigureAwait(false);
                },
                    CacheOptions.DefaultCacheOptions
                );

                return Result<TrustDto?>.Success(result);

            }
            catch (AcademiesApiException ex)
            {
                var errorMessage = $"An error occurred with the Academies API client. Response: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return Result<TrustDto?>.Failure(errorMessage);
            }
            catch (AggregateException ex)
            {
                var errorMessage = "An error occurred.";
                _logger.LogError(ex, errorMessage);
                return Result<TrustDto?>.Failure(errorMessage);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, ex.Message, nameof(TrustUkprnClientHandler));
                return Result<TrustDto?>.Failure(ex.Message);
            }
        }
    }
}