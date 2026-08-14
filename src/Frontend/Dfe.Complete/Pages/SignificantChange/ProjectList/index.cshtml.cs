using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Constants;
using Dfe.Complete.Models;
using Dfe.Complete.Pages.Pagination;
using MediatR;

namespace Dfe.Complete.Pages.SignificantChange.ProjectList;

public class IndexModel(ISender sender) : BaseSignificantChangeProjectsPageModel()
{  
    public List<ListProjectsByFilterResultsModel> Projects { get; private set; } = [];


    public async Task OnGet()
    {
        ViewData[TabNavigationModel.ViewDataKey] = SignificantChangeTabNavigationModel;

        var request = new ListProjectsByFilterQuery(PageNumber, PageSize);
        var result = await sender.Send(request);
        
        Pagination = new PaginationModel(RouteConstants.SignificantChange, PageNumber, result.ItemCount, PageSize);

        if (result.IsSuccess) Projects = result.Value!;
    }
}
