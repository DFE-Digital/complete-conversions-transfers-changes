using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Application.Mappers
{
    public static class ProjectTeamPresentationMapper
    {
        public static string? Map(ProjectTeam? projectTeam)
        {
            if(projectTeam == null)
                return null;

            return projectTeam switch
            {
                ProjectTeam.London => "London",
                ProjectTeam.SouthEast => "South East",
                ProjectTeam.YorkshireAndTheHumber => "Yorkshire and the Humber",
                ProjectTeam.NorthWest => "North West",
                ProjectTeam.EastOfEngland => "East of England",
                ProjectTeam.WestMidlands => "West Midlands",
                ProjectTeam.NorthEast => "North East",
                ProjectTeam.SouthWest => "South West",
                ProjectTeam.EastMidlands => "East Midlands",
                ProjectTeam.RegionalCaseWorkerServices => "Regional casework services",
                ProjectTeam.ServiceSupport => "Service support",
                ProjectTeam.BusinessSupport => "Business support",
                ProjectTeam.DataConsumers => "Data consumers",
                _ => throw new ArgumentOutOfRangeException(nameof(projectTeam), projectTeam, null)
            };
        }
    }
}
