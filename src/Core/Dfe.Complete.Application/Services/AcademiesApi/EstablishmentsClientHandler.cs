using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Services.AcademiesApi
{
    public record GetEstablishmentByUrnRequest(string Urn) : IRequest<Result<EstablishmentDto>>;

    public class EstablishmentsClientHandler(
        IEstablishmentsV4Client establishmentsV4Client,
        ILogger<EstablishmentsClientHandler> logger)
        : IRequestHandler<GetEstablishmentByUrnRequest, Result<EstablishmentDto>>
    {
        private readonly IEstablishmentsV4Client _establishmentsV4Client = establishmentsV4Client ?? throw new ArgumentNullException(nameof(establishmentsV4Client));
        private readonly ILogger<EstablishmentsClientHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<Result<EstablishmentDto>> Handle(GetEstablishmentByUrnRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Urn))
                throw new ArgumentException("URN cannot be null or empty.");
            
            try
            {
                var result = await _establishmentsV4Client
                    .GetEstablishmentByUrnAsync(request.Urn, cancellationToken)
                    .ConfigureAwait(false);

                return Result<EstablishmentDto>.Success(result);
            }
            catch (AcademiesApiException ex)
            {
                var errorMessage = $"An error occurred with the Academies API client. Response: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return Result<EstablishmentDto>.Failure(errorMessage);
            }
            catch (AggregateException ex)
            {
                var errorMessage = "An error occurred.";
                _logger.LogError(ex, errorMessage);
                return Result<EstablishmentDto>.Failure(errorMessage);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, ex.Message, nameof(EstablishmentsClientHandler));
                return Result<EstablishmentDto>.Failure(ex.Message);
            }
        }
    }
}