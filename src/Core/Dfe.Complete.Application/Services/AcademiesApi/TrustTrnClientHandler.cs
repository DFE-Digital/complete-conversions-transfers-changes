using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Services.AcademiesApi
{
    public record GetTrustByTrnRequest(string Trn) : IRequest<Result<TrustDto>>;

    public class TrustTrnClientHandler(
        ITrustsV4Client trustsV4Client,
        ILogger<GetTrustByTrnRequest> logger)
        : IRequestHandler<GetTrustByTrnRequest, Result<TrustDto>>
    {
        private readonly ITrustsV4Client _trustsV4Client = trustsV4Client ?? throw new ArgumentNullException(nameof(trustsV4Client));
        private readonly ILogger<GetTrustByTrnRequest> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<Result<TrustDto>> Handle(GetTrustByTrnRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Trn))
                throw new ArgumentException("Ukprn cannot be null or empty.");
            
            try
            {
                var result = await _trustsV4Client
                    .GetTrustByUkprn2Async(request.Trn, cancellationToken)
                    .ConfigureAwait(false);

                return Result<TrustDto>.Success(result);
            }
            catch (AcademiesApiException ex)
            {
                var errorMessage = $"An error occurred with the Academies API client. Response: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return Result<TrustDto>.Failure(errorMessage);
            }
            catch (AggregateException ex)
            {
                var errorMessage = "An error occurred.";
                _logger.LogError(ex, errorMessage);
                return Result<TrustDto>.Failure(errorMessage);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, ex.Message, nameof(TrustTrnClientHandler));
                return Result<TrustDto>.Failure(ex.Message);
            }
        }
    }
}