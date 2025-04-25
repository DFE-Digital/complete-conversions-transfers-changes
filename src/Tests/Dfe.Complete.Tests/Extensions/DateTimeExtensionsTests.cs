using Dfe.Complete.Extensions;

namespace Dfe.Complete.Tests.Extensions;

public class DateTimeExtensionsTests
{
    [Theory]
    [InlineData(1,2,2025, "01/02/2025")]
    [InlineData(31,12,9999, "31/12/9999")]
    [InlineData(1,1,1, "01/01/0001")]
    [InlineData(10,9,1990, "10/09/1990")]
    public void ToUkDateString_ReturnsTheCorrectString(int date, int month, int year, string expected)
    {
        Assert.Equal(expected, new DateTime(year, month, date).ToUkDateString());
    }
    
    [Theory]
    [InlineData(1,2,2025, "1 February 2025")]
    [InlineData(31,12,9999, "31 December 9999")]
    [InlineData(1,1,1, "1 January 0001")]
    [InlineData(10,9,1990, "10 September 1990")]
    public void ToDateString_ReturnsTheCorrectString_WithoutDayOfTheWeek(int date, int month, int year, string expected)
    {
        Assert.Equal(expected, new DateTime(year, month, date).ToDateString());
    }
    
    [Theory]
    [InlineData(1,2,2025, "Saturday 1 February 2025")]
    [InlineData(31,12,9999, "Friday 31 December 9999")]
    [InlineData(1,1,1, "Monday 1 January 0001")]
    [InlineData(10,9,1990, "Monday 10 September 1990")]
    public void ToDateString_ReturnsTheCorrectString_WithDayOfTheWeek(int date, int month, int year, string expected)
    {
        Assert.Equal(expected, new DateTime(year, month, date).ToDateString(true));
    }
    
    [Fact]
    public void ToDateString_ReturnsEmptyString_WhenDateTimeIsNull()
    {
        DateTime? test = null;
        Assert.Equal(string.Empty, test.ToDateString(true));
    }
    
    [Theory]
    [InlineData(1,2,2025, 0, 2)]
    [InlineData(31,12,9998, 1, 1, 9999)]
    [InlineData(1,1,1, 10, 11)]
    [InlineData(10,9,1990, 200, 5, 2007)]
    [InlineData(10, 12, 2025, 0, 12)]
    public void FirstOfMonth_ReturnsTheCorrectDateTime(int testDate, int testMonth, int testYear, int monthsToAdd, int expectedMonth, int? expectedYear = null)
    {
        Assert.Equal(new DateTime(expectedYear ?? testYear, expectedMonth, 1), new DateTime(testYear, testMonth, testDate).FirstOfMonth(monthsToAdd));
    }
    
    [Theory]
    [InlineData(1,2,2025, "Feb 2025")]
    [InlineData(31,12,9999, "Dec 9999")]
    [InlineData(1,1,1, "Jan 0001")]
    [InlineData(10,9,1990, "Sept 1990")]
    public void ToDateMonthYearString_ReturnsTheCorrectString_WithDayOfTheWeek(int date, int month, int year, string expected)
    {
        DateTime? testDate = new DateTime(year, month, date);
        Assert.Equal(expected, testDate.ToDateMonthYearString());
    }
    
    [Theory]
    [InlineData(1,2,2025, "February 2025")]
    [InlineData(31,12,9999, "December 9999")]
    [InlineData(1,1,1, "January 0001")]
    [InlineData(10,9,1990, "September 1990")]
    public void ToFullDateMonthYearString_ReturnsTheCorrectString(int date, int month, int year, string expected)
    {
        DateTime? testDate = new DateTime(year, month, date);
        Assert.Equal(expected, testDate.ToFullDateMonthYearString());
    }
    
    [Fact]
    public void ToDateMonthYearString_ReturnsEmptyString_WhenDateTimeIsNull()
    {
        DateTime? test = null;
        Assert.Equal(string.Empty, test.ToDateMonthYearString());
    }
    
    [Fact]
    public void ToFullDateMonthYearString_ReturnsTheEmptyString()
    {
        DateTime? nullDate = null;
        Assert.Equal(string.Empty, nullDate.ToFullDateMonthYearString());
    }
}