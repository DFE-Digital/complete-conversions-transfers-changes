namespace Dfe.Complete.Domain.Enums;

public static class EnumMapper
{
    public static Region? MapTeamToRegion(ProjectTeam projectTeam)
    {
        return projectTeam switch
        {
            ProjectTeam.RegionalCaseWorkerServices => default,
            ProjectTeam.ServiceSupport => default,
            ProjectTeam.London => Region.London,
            ProjectTeam.SouthEast => Region.SouthEast,
            ProjectTeam.YorkshireAndTheHumber => Region.YorkshireAndTheHumber,
            ProjectTeam.NorthWest => Region.NorthWest,
            ProjectTeam.EastOfEngland => Region.EastOfEngland,
            ProjectTeam.WestMidlands => Region.WestMidlands,
            ProjectTeam.NorthEast => Region.NorthEast,
            ProjectTeam.SouthWest => Region.SouthWest,
            ProjectTeam.EastMidlands => Region.EastMidlands,
            _ => throw new ArgumentOutOfRangeException(nameof(projectTeam), projectTeam, null)
        };
    }
}