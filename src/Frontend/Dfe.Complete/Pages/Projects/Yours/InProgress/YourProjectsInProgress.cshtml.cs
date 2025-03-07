using Dfe.Complete.Models;
using MediatR;

namespace Dfe.Complete.Pages.Projects.Yours.InProgress;

public class YourProjectsInProgress(ISender sender) : YourProjectsModel(InProgressNavigation)
{
    public async Task OnGet()
    {
        ViewData[TabNavigationModel.ViewDataKey] = YourProjectsTabNavigationModel;
    }
    
    public async Task OnGetMovePage()
    {
        await OnGet();
    }
}