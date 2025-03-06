using Dfe.Complete.Models;

namespace Dfe.Complete.Pages.Projects.Yours.InProgress;

public class YourProjectsInProgress() : YourProjectsModel(InProgressNavigation)
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