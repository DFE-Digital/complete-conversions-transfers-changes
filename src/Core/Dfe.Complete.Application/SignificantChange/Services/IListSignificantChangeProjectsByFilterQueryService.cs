using Dfe.Complete.Application.SignificantChange.Models;

namespace Dfe.Complete.Application.SignificantChange.Services;

public interface IListSignificantChangeProjectsByFilterQueryService
{
    IQueryable<ListSignificantChangeProjectsByFilterResultsModel> ListSignificantChangeProjectsByFilter(SignificantChangeProjectFilters filters);

    Task<SignificantChangeProjectFilterOptions> GetFilterOptions(SignificantChangeProjectFilters filters, CancellationToken cancellationToken);
}