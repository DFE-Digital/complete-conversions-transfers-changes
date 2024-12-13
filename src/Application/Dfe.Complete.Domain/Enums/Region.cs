using System.ComponentModel;

namespace Dfe.Complete.Domain.Enums
{
    public enum Region
    {
        [Description("London")]
        London = 1,
        [Description("South East")]
        SouthEast = 2,
        [Description("Yorkshire and the Humber")]
        YorkshireAndTheHumber = 3,
        [Description("North West")]
        NorthWest = 4,
        [Description("East of England")]
        EastOfEngland = 5,
        [Description("West Midlands")]
        WestMidlands = 6,
        [Description("North East")]
        NorthEast = 7,
        [Description("South West")]
        SouthWest = 8,
        [Description("East Midlands")]
        EastMidlands = 9
    }
}
