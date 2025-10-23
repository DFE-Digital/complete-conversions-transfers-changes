using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Cache;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Extensions;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Services.AcademiesApi
{
    public record GetEstablishmentByUrnRequest(string Urn) : IRequest<Result<EstablishmentDto?>>;

    public class EstablishmentsClientHandler(
        IEstablishmentsV4Client establishmentsV4Client,
        ILogger<EstablishmentsClientHandler> logger,
        IDistributedCache cache)
        : IRequestHandler<GetEstablishmentByUrnRequest, Result<EstablishmentDto?>>
    {
        private readonly IEstablishmentsV4Client _establishmentsV4Client = establishmentsV4Client ?? throw new ArgumentNullException(nameof(establishmentsV4Client));
        private readonly ILogger<EstablishmentsClientHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IDistributedCache _cache = cache;

        public async Task<Result<EstablishmentDto?>> Handle(GetEstablishmentByUrnRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Urn))
                throw new ArgumentException("URN cannot be null or empty.");
            
            try
            {
                var cacheKey = $"-establishment-{request.Urn}";

                var result = await _cache.GetOrSetAsync<EstablishmentDto>(
                cacheKey,
                async () =>
                {
                    return await _establishmentsV4Client
                    .GetEstablishmentByUrnAsync(request.Urn, cancellationToken)
                    .ConfigureAwait(false);
                },
                    CacheOptions.DefaultCacheOptions
                );                       
                
                return Result<EstablishmentDto?>.Success(result);
            }
            catch (AcademiesApiException ex)
            {
                var errorMessage = $"An error occurred with the Academies API client. Response: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return Result<EstablishmentDto?>.Failure(errorMessage);
            }
            catch (AggregateException ex)
            {
                var errorMessage = "An error occurred.";
                _logger.LogError(ex, errorMessage);
                return Result<EstablishmentDto?>.Failure(errorMessage);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, ex.Message, nameof(EstablishmentsClientHandler));
                return Result<EstablishmentDto?>.Failure(ex.Message);
            }
        }
    }
}