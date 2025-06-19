using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using MediatR;

namespace Dfe.Complete.Pages.Projects.List.Statistics
{
    public class StatisticsModel(ISender sender) : AllProjectsModel(StatisticsNavigation)
    {
        public void OnGet()
        {

           // var statistics = sender.Send(new ListAllProjectsStatisticsQuery());

        }
    }
}
