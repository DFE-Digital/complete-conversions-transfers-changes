using Dfe.Complete.Pages.Projects.List.ProjectsByMonth;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Tests.Pages.Projects.List;

public class ProjectsByMonthModelTests_Additional
{
    private class TestProjectsByMonthModel : ProjectsByMonthModel
    {
        public TestProjectsByMonthModel() : base("conversions") {}
    }

    [Fact]
    public void GetMonths_ShouldReturnFiveYearsRangeStartingTwoYearsBack()
    {
        var months = ProjectsByMonthModel.GetMonths();

        var expectedStart = new DateOnly(DateTime.Now.Year - 2, 1, 1);
        var expectedEnd = new DateOnly(DateTime.Now.Year + 2, 12, 1);

        Assert.Equal(60, months.Count); // 5 years * 12 months
        Assert.Equal(expectedStart, months.First());
        Assert.Equal(expectedEnd, months.Last());
    }

    [Theory]
    [InlineData("2024-01-01", "2024-12-31", true)]
    [InlineData("invalid", "2024-12-31", false)]
    [InlineData("2024-01-01", "invalid", false)]
    public void TryParseInputDates_ShouldReturnCorrectParseOutcome(string from, string to, bool expectedSuccess)
    {
        var model = new TestProjectsByMonthModel
        {
            FromDate = from,
            ToDate = to
        };

        var success = model.TryParseInputDates(out var fromDate, out var toDate);

        Assert.Equal(expectedSuccess, success);

        if (expectedSuccess)
        {
            Assert.True(fromDate < toDate || fromDate == toDate);
        }
    }

    [Theory]
    [InlineData(1, 2025, true)]
    [InlineData(13, 2025, false)]
    public void ParseDate_ShouldReturnValidDateOnlyOrNull(int month, int year, bool shouldBeValid)
    {
        var result = ProjectsByMonthModel.ParseDate(month, year);

        if (shouldBeValid)
            Assert.NotNull(result);
        else
            Assert.Null(result);
    }

    [Fact]
    public void RedirectToDateRange_ShouldReturnCorrectRedirectResult()
    {
        var from = new DateTime(2024, 1, 1);
        var to = new DateTime(2024, 12, 1);
        string pathTemplate = "/projects/by-month/{0}/{1}/{2}/{3}";

        var result = ProjectsByMonthModel.RedirectToDateRange(pathTemplate, from, to) as RedirectResult;

        string expectedUrl = string.Format(pathTemplate, from.Month, from.Year, to.Month, to.Year);

        Assert.NotNull(result);
        Assert.Equal(expectedUrl, result.Url);
    }
}
