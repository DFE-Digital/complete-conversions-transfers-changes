using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Pages.Projects.List.ProjectsByMonth;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Tests.Pages.Projects.List;

public class ProjectsByMonthModelTests_Additional
{
    private class TestProjectsByMonthModel : ProjectsByMonthModel
    {
        public TestProjectsByMonthModel() : base("conversions") { }
    }

    [Fact]
    public void GetMonths_ShouldReturnFiveYearsRangeStartingTwoYearsBack()
    {
        var months = ProjectsByMonthModel.GetMonths();

        var expectedStart = new DateOnly(DateTime.Now.Year - 2, 1, 1);
        var expectedEnd = new DateOnly(DateTime.Now.Year + 2, 12, 1);

        Assert.Equal(60, months.Count); // 5 years * 12 months
        Assert.Equal(expectedStart, months[0]);
        Assert.Equal(expectedEnd, months[^1]);
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

    [Fact]
    public void ParseDate_ShouldReturnValidDateOnlyOrNull()
    {
        var result = ProjectsByMonthModel.ParseDate(1, 2024);
        Assert.NotNull(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void ParseDate_ShouldThrowException_WhenOutOfRange(int month)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ProjectsByMonthModel.ParseDate(month, 2024));
    }

    [Fact]
    public void RedirectToDateRange_ShouldReturnCorrectRedirectResult()
    {
        var from = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var to = new DateTime(2024, 12, 1, 0, 0, 0, DateTimeKind.Utc    );
        string pathTemplate = "/projects/by-month/{0}/{1}/{2}/{3}";

        var result = ProjectsByMonthModel.RedirectToDateRange(pathTemplate, from, to) as RedirectResult;

        string expectedUrl = string.Format(pathTemplate, from.Month, from.Year, to.Month, to.Year);

        Assert.NotNull(result);
        Assert.Equal(expectedUrl, result.Url);
    }

    [Theory]
    [InlineData(ProjectType.Conversion)]
    [InlineData(ProjectType.Transfer)]

    public void GetProjectByMonthUrl_ShouldReturnCorrectUrl_DependantOnProjectType(ProjectType projectType)
    {
        DateTime date = DateTime.Now.AddMonths(1);
        string month = date.Month.ToString("0");
        string year = date.Year.ToString("0000");

        // Arrange
        string expectedUrl = string.Format(projectType == ProjectType.Conversion ? RouteConstants.ConversionProjectsByMonth : RouteConstants.TransfersProjectsByMonth, month, year);

        // Act
        var result = ProjectsByMonthModel.GetProjectByMonthUrl(projectType);

        // Assert
        Assert.Equal(result, expectedUrl);
    }

    [Theory]
    [InlineData(ProjectType.Conversion, ProjectTeam.DataConsumers, false, "/projects/all/by-month/conversions/4/2024")]
    [InlineData(ProjectType.Conversion, ProjectTeam.NorthWest, true, "/projects/all/by-month/conversions/4/2024")]
    [InlineData(ProjectType.Transfer, ProjectTeam.NorthWest, false, "/projects/all/by-month/transfers/4/2024")]
    public void GetProjectByMonthsUrl_ShouldReturnCorrectUrl(ProjectType projectType, ProjectTeam team, bool managesTeam, string expectedUrl)
    {
        // Arrange
        const int fromMonth = 3;
        const int fromYear = 2024;
        const int toMonth = 3;
        const int toYear = 2024;

        var user = new UserDto
        {
            ManageTeam = managesTeam,
            Team = team.ToString()
        };

        // Act
        var result = ProjectsByMonthModel.GetProjectByMonthsUrl(projectType, user, fromMonth, fromYear, toMonth, toYear);

        // Assert
        Assert.Equal(expectedUrl, result);
    }

    [Fact]
    public void GetProjectByMonthsUrl_ShouldReturnCorrectUrl_InDecember()
    {
        // Arrange
        const int fromMonth = 12;
        const int fromYear = 2024;

        var user = new UserDto
        {
            ManageTeam = false,
            Team = "north_west"
        };

        // Act
        var result = ProjectsByMonthModel.GetProjectByMonthsUrl(ProjectType.Conversion, user, fromMonth, fromYear, default, default);

        // Assert
        Assert.Equal("/projects/all/by-month/conversions/1/2025", result);
    }
}
