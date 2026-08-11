using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Models;

namespace Dfe.Complete.Pages.SignificantChange.ProjectList;

public class IndexModel : BaseSignificantChangeProjectsPageModel
{  
    public List<ListProjectsByFilterResultsModel> Projects { get; private set; } = [];

    public void OnGet()
    {
    }
}
