using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models;
using Dfe.Complete.Models.ProjectList;
using Dfe.Complete.Pages.Pagination;
using MediatR;

namespace Dfe.Complete.Pages.SignificantChange.ProjectList;

public class IndexModel(ISender sender) : BaseSignificantChangeProjectsPageModel()
{  
    public List<ListProjectsByFilterResultsModel> Projects { get; private set; } = [];

    public ProjectListFilters Filters { get; } = new();


    public async Task OnGet()
    {
        ViewData[TabNavigationModel.ViewDataKey] = SignificantChangeTabNavigationModel;

        Filters.PersistUsing(new Dictionary<string, object?>());
        Filters.AvailableStatuses = [.. Enum.GetNames<ProjectState>()];
        Filters.PopulateFrom(Request.Query);

        List<ProjectState> selectedProjectStatuses = [];

        foreach (var selectedStatus in Filters.SelectedStatuses.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (Enum.TryParse<ProjectState>(selectedStatus, true, out var parsedStatus))
            {
                selectedProjectStatuses.Add(parsedStatus);
            }
        }

        var request = new ListProjectsByFilterQuery(
            PageNumber,
            PageSize);
        var result = await sender.Send(request);

        var queryParts = new List<string>();

        foreach (var selectedStatus in Filters.SelectedStatuses)
        {
            queryParts.Add($"SelectedStatuses={Uri.EscapeDataString(selectedStatus)}");
        }

        var paginationUrl = RouteConstants.SignificantChange +
            (queryParts.Count > 0 ? $"?{string.Join("&", queryParts)}" : string.Empty);

        Pagination = new PaginationModel(paginationUrl, PageNumber, result.ItemCount, PageSize);

        if (result.IsSuccess) Projects = result.Value!;
    }
}
