using Dfe.Complete.Application.SignificantChange.Models;
using Dfe.Complete.Application.SignificantChange.Queries;
using Dfe.Complete.Constants;
using Dfe.Complete.Models;
using Dfe.Complete.Models.ProjectList;
using Dfe.Complete.Pages.Pagination;
using MediatR;

namespace Dfe.Complete.Pages.SignificantChange.ProjectList;

public class IndexModel(ISender sender) : BaseSignificantChangeProjectsPageModel()
{  
    public List<ListSignificantChangeProjectsByFilterResultsModel> Projects { get; private set; } = [];

    public ProjectListFilters Filters { get; } = new();

    public async Task OnGet()
    {
        ViewData[TabNavigationModel.ViewDataKey] = SignificantChangeTabNavigationModel;

        Filters.PersistUsing(new Dictionary<string, object?>());
        Filters.PopulateFrom(Request.Query);

        var filters = Filters.ToFilters();

        var request = new ListSignificantChangeProjectsByFilterQuery(
            filters,
            PageNumber - 1,
            PageSize);

        var result = await sender.Send(request);

        var options = await sender.Send(new GetSignificantChangeProjectFilterOptionsQuery(filters));

        Filters.StatusOptions = options.Statuses;
        Filters.ProjectOwnerOptions = options.ProjectOwners;
        Filters.RegionOptions = options.Regions;
        Filters.LocalAuthorityOptions = options.LocalAuthorities;

        var queryString = Filters.ToQueryString();

        var paginationUrl = RouteConstants.SignificantChange +
            (string.IsNullOrEmpty(queryString) ? string.Empty : $"?{queryString}");

        Pagination = new PaginationModel(paginationUrl, PageNumber, result.ItemCount, PageSize);

        if (result.IsSuccess) Projects = result.Value!;
    }
}
