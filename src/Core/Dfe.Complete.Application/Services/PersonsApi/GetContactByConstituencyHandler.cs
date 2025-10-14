using AutoMapper;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Contacts.Models;
using Dfe.Complete.Application.Services.AcademiesApi;
using GovUK.Dfe.PersonsApi.Client.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Services.PersonsApi;

public record GetContactByConstituency(string ConstituencyName) : IRequest<Result<ConstituencyMemberContactDto>>;


internal class GetContactByConstituencyHandler(IConstituenciesClient constituenciesClient, IMapper mapper, ILogger<GetContactByConstituencyHandler> logger) : IRequestHandler<GetContactByConstituency, Result<ConstituencyMemberContactDto>>
{
    private readonly IConstituenciesClient _constituenciesClient = constituenciesClient ?? throw new ArgumentNullException(nameof(constituenciesClient));
    private readonly ILogger<GetContactByConstituencyHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<Result<ConstituencyMemberContactDto>> Handle(GetContactByConstituency request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ConstituencyName))
            throw new ArgumentException("Constituency name cannot be null or empty.");

        try
        {
            var result = await _constituenciesClient.GetMemberOfParliamentByConstituencyAsync(request.ConstituencyName, cancellationToken)                
                .ConfigureAwait(false);

            var constituencyMemberContactDto =  mapper.Map<ConstituencyMemberContactDto>(result);
            return Result<ConstituencyMemberContactDto>.Success(constituencyMemberContactDto);
        }
        catch (PersonsApiException ex)
        {
            var errorMessage = $"An error occurred with the Persons API client. Response: {ex.Message}";
            _logger.LogError(ex, errorMessage);
            return Result<ConstituencyMemberContactDto>.Failure(errorMessage);
        }
        catch (AggregateException ex)
        {
            var errorMessage = "An error occurred.";
            _logger.LogError(ex, errorMessage);
            return Result<ConstituencyMemberContactDto>.Failure(errorMessage);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, ex.Message, nameof(EstablishmentsClientHandler));
            return Result<ConstituencyMemberContactDto>.Failure(ex.Message);
        }
    }
}