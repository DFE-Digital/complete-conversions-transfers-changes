namespace Dfe.Complete.Application.Projects.Models
{
    public class ListAllProjectsStatisticsModel
    {
        public ProjectsModel OveraAllProjects { get; set; } = null!;
        public ProjectsModel RegionalCaseworkServicesProjects { get; set; } = null!;
        public ProjectsModel NotRegionalCaseworkServicesProjects { get; set; } = null!;
        public List<RegionProjectDetailsModel> ConversionsPerRegion { get; set; } = [];
        public List<RegionProjectDetailsModel> TransfersPerRegion { get; set; } = [];
        public List<AllOpenersProjectsModel> SixMonthViewOfAllProjectOpeners { get; set; } = null!;
        public NewProjectsInThisMonth NewProjects { get; set; } = null!;
        public Dictionary<string, int> UsersPerTeam { get; set; } = [];

    }

    public record ProjectsModel(ProjectDetailsModel Conversions, ProjectDetailsModel Transfers)
    {
    }
    public record RegionProjectDetailsModel(string RegionName, ProjectDetailsModel Details);

    public record AllOpenersProjectsModel(string Date, int Conversions, int Tranfers)
    {
    }

    public record ProjectDetailsModel(int InProgressProjects, int CompletedProjects, int UnassignedProjects, int TotalProjects, int? DaoRevokedProjects = null)
    {
    }

    public record NewProjectsInThisMonth(string Date, int TotalProjects, int TotalConversions, int TotalTransfers)
    { 
    }
}