using System.ComponentModel;

namespace Dfe.Complete.Domain.Enums
{
    public enum Region
    {
        [Description("london")]
        London = 1,
        [Description("south_east")]
        SouthEast = 2,
        [Description("yorkshire_and_the_humber")]
        YorkshireAndTheHumber = 3,
        [Description("north_west")]
        NorthWest = 4,
        [Description("east_of_england")]
        EastOfEngland = 5,
        [Description("west_midlands")]
        WestMidlands = 6,
        [Description("north_east")]
        NorthEast = 7,
        [Description("south_west")]
        SouthWest = 8,
        [Description("east_midlands")]
        EastMidlands = 9
    }
}
