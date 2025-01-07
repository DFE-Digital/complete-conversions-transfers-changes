namespace Dfe.Complete.Domain.Enums;

public static class EnumMapper
{
    public static Region? MapTeamToRegion(Team team)
    {
        return team switch
        {
            Team.RegionalCaseWorkerServices => default,
            Team.ServiceSupport => default,
            Team.London => Region.London,
            Team.SouthEast => Region.SouthEast,
            Team.YorkshireAndTheHumber => Region.YorkshireAndTheHumber,
            Team.NorthWest => Region.NorthWest,
            Team.EastOfEngland => Region.EastOfEngland,
            Team.WestMidlands => Region.WestMidlands,
            Team.NorthEast => Region.NorthEast,
            Team.SouthWest => Region.SouthWest,
            Team.EastMidlands => Region.EastMidlands,
            _ => throw new ArgumentOutOfRangeException(nameof(team), team, null)
        };
    }
}