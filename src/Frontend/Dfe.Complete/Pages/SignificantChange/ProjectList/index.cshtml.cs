using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Models;
using MediatR;

namespace Dfe.Complete.Pages.SignificantChange.ProjectList;

public class IndexModel(ISender sender) : BaseSignificantChangeProjectsPageModel
{  
    public List<ListProjectsByFilterResultsModel> Projects { get; private set; } = [];

    public async Task OnGet()
    {
        var request = new ListProjectsByFilterQuery();
        var result = await sender.Send(request);

        if (result.IsSuccess) Projects = result.Value!;
    }
}
