using Dfe.Complete.Application.SignificantChange.Models;
using Dfe.Complete.Application.SignificantChange.Services;
using MediatR;

namespace Dfe.Complete.Application.SignificantChange.Queries;

public record GetSignificantChangeProjectFilterOptionsQuery(SignificantChangeProjectFilters? Filters = null)
    : IRequest<SignificantChangeProjectFilterOptions>;

internal class GetSignificantChangeProjectFilterOptionsQueryHandler(
    IListSignificantChangeProjectsByFilterQueryService listSignificantChangeProjectsByFilterQueryService
) : IRequestHandler<GetSignificantChangeProjectFilterOptionsQuery, SignificantChangeProjectFilterOptions>
{
    public Task<SignificantChangeProjectFilterOptions> Handle(GetSignificantChangeProjectFilterOptionsQuery request, CancellationToken cancellationToken)
    {
        return listSignificantChangeProjectsByFilterQueryService.GetFilterOptions(request.Filters ?? new(), cancellationToken);
    }
}
