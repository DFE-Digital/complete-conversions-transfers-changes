using Dfe.Complete.Pages.Projects.ProjectView;
using MediatR;

namespace Dfe.Complete.Pages.Projects
{
    public class ViewProjectNotesModel(ISender sender) : ProjectViewLayoutModel(sender);
}
