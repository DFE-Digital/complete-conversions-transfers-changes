using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using MediatR;

namespace Dfe.Complete.Pages.Projects.List.Statistics
{
    public class StatisticsModel(ISender sender) : AllProjectsModel(StatisticsNavigation)
    {
        public ListAllProjectsStatisticsModel Statistics { get; set; } = new();
        public async Task OnGetAsync()
        {
            var statistics = await sender.Send(new ListAllProjectsStatisticsQuery());
            Statistics = statistics.Value!;
        }
    }
}
