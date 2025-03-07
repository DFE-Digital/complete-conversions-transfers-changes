using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using MediatR;

namespace Dfe.Complete.Pages.Projects.Yours.InProgress;

public class YourProjectsInProgress(ISender sender) : YourProjectsModel(InProgressNavigation)
{
    public async Task OnGet()
    {
        ViewData[TabNavigationModel.ViewDataKey] = YourProjectsTabNavigationModel;

        var userAdId = User.GetUserAdId();

        var result = await sender.Send(new ListAllProjectForUserQuery(ProjectState.Active, userAdId));
    }
    
    public async Task OnGetMovePage()
    {
        await OnGet();
    }
}