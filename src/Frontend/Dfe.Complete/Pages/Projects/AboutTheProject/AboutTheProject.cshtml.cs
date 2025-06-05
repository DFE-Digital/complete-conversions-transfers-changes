using Dfe.Complete.Pages.Projects.ProjectView;
using MediatR;

namespace Dfe.Complete.Pages.Projects.AboutTheProject
{
    public class AboutTheProjectModel(ISender sender) : ProjectLayoutModel(sender, AboutTheProjectNavigation);

    //public class AboutTheProjectModel1(ISender sender) : BaseProjectTabPageModel(sender, AboutProject);
}
