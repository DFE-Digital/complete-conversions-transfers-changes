using Dfe.Complete.Models;
using MediatR;

namespace Dfe.Complete.Pages.Projects.AboutTheProject
{
    public class AboutTheProjectModel(ISender sender) : BaseProjectTabPageModel(sender, AboutProject);
}
