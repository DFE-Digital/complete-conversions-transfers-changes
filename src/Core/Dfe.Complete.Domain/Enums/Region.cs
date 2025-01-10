using System.ComponentModel;

namespace Dfe.Complete.Domain.Enums
{
    public enum Region
    {
        [Description("london")]
        London = 'H',
        [Description("south_east")]
        SouthEast = 'J',
        [Description("yorkshire_and_the_humber")]
        YorkshireAndTheHumber = 'D',
        [Description("north_west")]
        NorthWest = 'B',
        [Description("east_of_england")]
        EastOfEngland = 'G',
        [Description("west_midlands")]
        WestMidlands = 'F',
        [Description("north_east")]
        NorthEast = 'A',
        [Description("south_west")]
        SouthWest = 'K',
        [Description("east_midlands")]
        EastMidlands = 'E'
    }
}
