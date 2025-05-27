using Dfe.Complete.Pages.Projects.ProjectView;
using MediatR;

namespace Dfe.Complete.Pages.Projects.Notes
{
    public class ViewProjectNotesModel(ISender sender) : ProjectLayoutModel(sender, NotesNavigation);
}
