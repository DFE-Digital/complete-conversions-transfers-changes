namespace Dfe.Complete.Domain.Enums;

public static class EnumHelper
{
    private static readonly ProjectTeam[] GeographicTeams = [
        ProjectTeam.London,
        ProjectTeam.SouthEast,
        ProjectTeam.YorkshireAndTheHumber,
        ProjectTeam.NorthWest,
        ProjectTeam.EastOfEngland,
        ProjectTeam.WestMidlands,
        ProjectTeam.NorthEast,
        ProjectTeam.SouthWest,
        ProjectTeam.EastMidlands
    ];

    public static bool TeamIsGeographic(ProjectTeam? projectTeam)
    {
        if (projectTeam == null) return false;
        return GeographicTeams.ToList().Contains((ProjectTeam)projectTeam);
    }
}