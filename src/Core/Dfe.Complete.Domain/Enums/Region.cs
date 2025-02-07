using System.ComponentModel;
using Dfe.Complete.Utils.Attributes;

namespace Dfe.Complete.Domain.Enums
{
    public enum Region
    {
        [Description("london")]
        [SecondaryDescription("London")]
        London = 'H',
        [Description("south_east")]
        [SecondaryDescription("South East")]
        SouthEast = 'J',
        [Description("yorkshire_and_the_humber")]
        [SecondaryDescription("Yorkshire and the Humber")]
        YorkshireAndTheHumber = 'D',
        [Description("north_west")]
        [SecondaryDescription("North West")]
        NorthWest = 'B',
        [Description("east_of_england")]
        [SecondaryDescription("East of England")]
        EastOfEngland = 'G',
        [Description("west_midlands")]
        [SecondaryDescription("West Midlands")]
        WestMidlands = 'F',
        [Description("north_east")]
        [SecondaryDescription("North East")]
        NorthEast = 'A',
        [Description("south_west")]
        [SecondaryDescription("South West")]
        SouthWest = 'K',
        [Description("east_midlands")]
        [SecondaryDescription("East Midlands")]
        EastMidlands = 'E'
    }
}
