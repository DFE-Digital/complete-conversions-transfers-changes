using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Application.Users.Interfaces;
using Dfe.Complete.Application.Users.Models;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project
{
    public class ListAllProjectsStatisticsQueryHandlerTests
    {
        private readonly Mock<IProjectsQueryBuilder> _projectsQueryBuilderMock = new();
        private readonly Mock<IUsersQueryBuilder> _usersQueryBuilderMock = new();
        private readonly Mock<ILogger<ListAllProjectsStatisticsQueryHandler>> _loggerMock = new();

        private ListAllProjectsStatisticsQueryHandler CreateHandler(
            List<Domain.Entities.Project> projects = null!,
            List<string> usersPerTeam = null!)
        {
            if (projects != null)
            {
                var projectsQueryable = (projects).BuildMock();
                _projectsQueryBuilderMock.Setup(x => x.ApplyProjectFilters(It.IsAny<ProjectFilters>()))
                    .Returns(_projectsQueryBuilderMock.Object);
                _projectsQueryBuilderMock.Setup(x => x.GetProjects())
                    .Returns(projectsQueryable);
            }
            // Setup users
            _usersQueryBuilderMock.Setup(x => x.ApplyUsersFilters(It.IsAny<UsersFilters>()))
                .Returns(_usersQueryBuilderMock.Object);
            _usersQueryBuilderMock.Setup(x => x.Where(It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.User, bool>>>()))
                .Returns(_usersQueryBuilderMock.Object);
            _usersQueryBuilderMock.Setup(x => x.GetUsers())
                .Returns((usersPerTeam ?? [])
                    .Select(kvp => new Domain.Entities.User { Team = kvp, Id = new UserId(Guid.NewGuid()) })
                    .BuildMock());

            return new ListAllProjectsStatisticsQueryHandler(
                _projectsQueryBuilderMock.Object,
                _usersQueryBuilderMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturns_CorrectStatistics()
        {
            // Arrange
            var dateTime = new DateTime(2025, 06, 30);
            var projects = SetUpProjects(dateTime); 
            var regions = GetRegions(projects);
            var teams = projects.DistinctBy(x => x.Team)
                .Select(p => p.Team.ToDescription()).ToList();

            var handler = CreateHandler(projects, teams);

            // Act
            var result = await handler.Handle(new ListAllProjectsStatisticsQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(9, result.Value.OveraAllProjects.Conversions.TotalProjects);
            Assert.Equal(2, result.Value.OveraAllProjects.Conversions.InProgressProjects);
            Assert.Equal(2, result.Value.OveraAllProjects.Conversions.CompletedProjects);
            Assert.Equal(4, result.Value.OveraAllProjects.Conversions.DaoRevokedProjects);
            Assert.Equal(1, result.Value.OveraAllProjects.Conversions.UnassignedProjects);
            Assert.Equal(8, result.Value.OveraAllProjects.Transfers.TotalProjects);
            Assert.Equal(2, result.Value.OveraAllProjects.Transfers.InProgressProjects);
            Assert.Equal(2, result.Value.OveraAllProjects.Transfers.CompletedProjects);
            Assert.Equal(3, result.Value.OveraAllProjects.Transfers.DaoRevokedProjects);
            Assert.Equal(1, result.Value.OveraAllProjects.Transfers.UnassignedProjects);

            Assert.Equal(4, result.Value.RegionalCaseworkServicesProjects.Conversions.TotalProjects);
            Assert.Equal(1, result.Value.RegionalCaseworkServicesProjects.Conversions.InProgressProjects);
            Assert.Equal(1, result.Value.RegionalCaseworkServicesProjects.Conversions.CompletedProjects);
            Assert.Equal(0, result.Value.RegionalCaseworkServicesProjects.Conversions.UnassignedProjects);
            Assert.Null(result.Value.RegionalCaseworkServicesProjects.Conversions.DaoRevokedProjects);
            Assert.Equal(3, result.Value.RegionalCaseworkServicesProjects.Transfers.TotalProjects);
            Assert.Equal(1, result.Value.RegionalCaseworkServicesProjects.Transfers.InProgressProjects);
            Assert.Equal(1, result.Value.RegionalCaseworkServicesProjects.Transfers.CompletedProjects);
            Assert.Null(result.Value.RegionalCaseworkServicesProjects.Transfers.DaoRevokedProjects);
            Assert.Equal(0, result.Value.RegionalCaseworkServicesProjects.Conversions.UnassignedProjects);

            Assert.Null(result.Value.NotRegionalCaseworkServicesProjects.Conversions.DaoRevokedProjects);
            Assert.Equal(5, result.Value.NotRegionalCaseworkServicesProjects.Conversions.TotalProjects);
            Assert.Equal(1, result.Value.NotRegionalCaseworkServicesProjects.Conversions.InProgressProjects);
            Assert.Equal(1, result.Value.NotRegionalCaseworkServicesProjects.Conversions.CompletedProjects);
            Assert.Equal(1, result.Value.NotRegionalCaseworkServicesProjects.Conversions.UnassignedProjects);
            Assert.Equal(5, result.Value.NotRegionalCaseworkServicesProjects.Transfers.TotalProjects);
            Assert.Equal(1, result.Value.NotRegionalCaseworkServicesProjects.Transfers.InProgressProjects);
            Assert.Equal(1, result.Value.NotRegionalCaseworkServicesProjects.Transfers.CompletedProjects);
            Assert.Equal(1, result.Value.NotRegionalCaseworkServicesProjects.Transfers.UnassignedProjects);
            Assert.Null(result.Value.NotRegionalCaseworkServicesProjects.Transfers.DaoRevokedProjects);

            AssertProjectsPerRegion(regions, result.Value.ConversionsPerRegion, ProjectType.Conversion); 
            AssertProjectsPerRegion(regions, result.Value.TransfersPerRegion, ProjectType.Transfer);

            AssertSixMonthViewOfAllProjectOpeners(dateTime, result.Value.SixMonthViewOfAllProjectOpeners);

            AssertNewProjectsInAMonth(dateTime, result.Value.NewProjects);

            AssertUsersPerTeam(result.Value.UsersPerTeam, teams.Select(FormatDescription).ToList()); 
        }

        [Fact]
        public async Task Handle_ReturnsFailure_OnException()
        {
            // Arrange
            _projectsQueryBuilderMock.Setup(x => x.ApplyProjectFilters(It.IsAny<ProjectFilters>()))
                .Throws(new Exception("Test exception"));
            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(new ListAllProjectsStatisticsQuery(), CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Test exception", result.Error);
        }
        private static void AssertUsersPerTeam(Dictionary<string, int> usersPerTeam, List<string> teams)
        {
            foreach (var (key, value) in usersPerTeam)
            {
                Assert.Equal(teams.Contains(key) ? 1 : 0, value);
            }
        }

        private static void AssertNewProjectsInAMonth(DateTime dateTime, NewProjectsInThisMonth newProjects)
        { 
            var monthYear = $"{dateTime:MMMM} {dateTime.Year}";
            Assert.Equal(monthYear, newProjects.Date);
            Assert.Equal(17, newProjects.TotalProjects);
            Assert.Equal(9, newProjects.TotalConversions);
            Assert.Equal(8, newProjects.TotalTransfers);
        }

        private static void AssertSixMonthViewOfAllProjectOpeners(DateTime dateTime, List<AllOpenersProjectsModel> projects)
        {
            for (int i = 1; i <= projects.Count; i++)
            {
                var targetMonth = dateTime.AddMonths(i);
                var monthYear = $"{targetMonth:MMMM} {targetMonth.Year}";
                var project = projects[i - 1];
                Assert.Equal(monthYear, project.Date);
                Assert.Equal(0, project.Conversions);
                Assert.Equal(0, project.Tranfers);
            }
        }
        private static void AssertProjectsPerRegion(Dictionary<string, Dictionary<ProjectType, ProjectDetailsModel>> regions, List<RegionProjectDetailsModel> projectsPerRegion, ProjectType projectType)
        {
            foreach (var region in regions)
            {
                var project = projectsPerRegion.FirstOrDefault(x => x.RegionName == FormatDescription(region.Key));
                Assert.NotNull(project);
                var typeProject = region.Value.FirstOrDefault(x => x.Key == projectType); 
                 
                Assert.Equal(typeProject.Value.TotalProjects, project.Details.TotalProjects);
                Assert.Equal(typeProject.Value.InProgressProjects, project.Details.InProgressProjects);
                Assert.Equal(typeProject.Value.CompletedProjects, project.Details.CompletedProjects);
                Assert.Equal(typeProject.Value.UnassignedProjects, project.Details.UnassignedProjects);
                Assert.Null(project.Details.DaoRevokedProjects);
            }
        }
         
        private static string FormatDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return string.Empty;

            return string.Join(" ", description.Replace("_", " ")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(word => char.ToUpper(word[0]) + word[1..].ToLower()));
        }

        private record AssignmentCounts(int AssignedCount, int UnassignedCount);
        private record RegionStateTypeKey(string Region, ProjectState State, ProjectType? Type);

        private static List<Domain.Entities.Project> SetUpProjects(DateTime dateTime)
            =>
            [
                new() { Type = ProjectType.Conversion, State = ProjectState.Active, AssignedToId = new UserId(Guid.NewGuid()), Region = Region.London, Team = ProjectTeam.London, CreatedAt = dateTime },
                new() { Type = ProjectType.Conversion, State = ProjectState.Completed, AssignedToId = new UserId(Guid.NewGuid()), Region = Region.SouthEast, Team = ProjectTeam.SouthEast, CreatedAt = dateTime },
                new() { Type = ProjectType.Conversion, State = ProjectState.DaoRevoked, AssignedToId = new UserId(Guid.NewGuid()), Region = Region.EastOfEngland, Team = ProjectTeam.EastOfEngland, CreatedAt = dateTime },
                new() { Type = ProjectType.Conversion, State = ProjectState.DaoRevoked, AssignedToId = new UserId(Guid.NewGuid()), Region = Region.YorkshireAndTheHumber, Team = ProjectTeam.YorkshireAndTheHumber, CreatedAt = dateTime },
                new() { Type = ProjectType.Conversion, State = ProjectState.Active, Region = Region.London, Team = ProjectTeam.London, CreatedAt = dateTime },

                new() { Type = ProjectType.Conversion, State = ProjectState.Active, AssignedToId = new UserId(Guid.NewGuid()), Region = Region.London, Team = ProjectTeam.RegionalCaseWorkerServices, CreatedAt = dateTime },
                new() { Type = ProjectType.Conversion, State = ProjectState.Completed, AssignedToId = new UserId(Guid.NewGuid()), Region = Region.SouthEast, Team = ProjectTeam.RegionalCaseWorkerServices, CreatedAt = dateTime },
                new() { Type = ProjectType.Conversion, State = ProjectState.DaoRevoked, AssignedToId = new UserId(Guid.NewGuid()), Region = Region.EastOfEngland, Team = ProjectTeam.RegionalCaseWorkerServices, CreatedAt = dateTime },
                new() { Type = ProjectType.Conversion, State = ProjectState.DaoRevoked, AssignedToId = new UserId(Guid.NewGuid()), Region = Region.YorkshireAndTheHumber, Team = ProjectTeam.RegionalCaseWorkerServices, CreatedAt = dateTime },

                new() { Type = ProjectType.Transfer, State = ProjectState.Active, AssignedToId = new UserId(Guid.NewGuid()), Region = Region.London, Team = ProjectTeam.London, CreatedAt = dateTime },
                new() { Type = ProjectType.Transfer, State = ProjectState.Completed, AssignedToId = new UserId(Guid.NewGuid()), Region = Region.SouthEast, Team = ProjectTeam.SouthEast, CreatedAt = dateTime },
                new() { Type = ProjectType.Transfer, State = ProjectState.DaoRevoked, AssignedToId = new UserId(Guid.NewGuid()), Region = Region.EastOfEngland, Team = ProjectTeam.EastOfEngland, CreatedAt = dateTime },
                new() { Type = ProjectType.Transfer, State = ProjectState.DaoRevoked, AssignedToId = new UserId(Guid.NewGuid()), Region = Region.YorkshireAndTheHumber, Team = ProjectTeam.YorkshireAndTheHumber, CreatedAt = dateTime },
                new() { Type = ProjectType.Transfer, State = ProjectState.Active, Region = Region.London, Team = ProjectTeam.London, CreatedAt = dateTime },

                new() { Type = ProjectType.Transfer, State = ProjectState.Active, AssignedToId = new UserId(Guid.NewGuid()), Region = Region.London, Team = ProjectTeam.RegionalCaseWorkerServices, CreatedAt = dateTime },
                new() { Type = ProjectType.Transfer, State = ProjectState.Completed, AssignedToId = new UserId(Guid.NewGuid()), Region = Region.SouthEast, Team = ProjectTeam.RegionalCaseWorkerServices, CreatedAt = dateTime },
                new() { Type = ProjectType.Transfer, State = ProjectState.DaoRevoked, AssignedToId = new UserId(Guid.NewGuid()), Region = Region.EastOfEngland, Team = ProjectTeam.RegionalCaseWorkerServices, CreatedAt = dateTime },
            ];

        private static Dictionary<string, Dictionary<ProjectType, ProjectDetailsModel>> GetRegions(List<Domain.Entities.Project> projects) =>
            projects
            .GroupBy(p => p.Region?.ToDescription() ?? "Unassigned")
            .ToDictionary(
                regionGroup => regionGroup.Key,
                regionGroup => regionGroup
                    .GroupBy(p => p.Type ?? ProjectType.Conversion)
                    .ToDictionary(
                        typeGroup => typeGroup.Key,
                        typeGroup => new ProjectDetailsModel(
                            typeGroup.Count(p => p.State == ProjectState.Active && p.AssignedToId != null),
                            typeGroup.Count(p => p.State == ProjectState.Completed),
                            typeGroup.Count(p => p.State == ProjectState.Active && p.AssignedToId == null),
                            typeGroup.Count()
                        )
                    )
            );
    }

}
