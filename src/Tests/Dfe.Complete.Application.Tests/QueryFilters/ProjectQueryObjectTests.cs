using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.QueryFilters;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Dfe.Complete.Application.Tests.QueryFilters
{
    public class ProjectQueryObjectTests
    {
        private static IQueryable<Project> SampleProjects() => new[]
        {
            new Project { AssignedToId = new UserId(Guid.NewGuid()), RegionalDeliveryOfficerId = new UserId(Guid.NewGuid()),
                          IncomingTrustUkprn = new Ukprn(12345678), OutgoingTrustUkprn = new Ukprn(87654321),
                          NewTrustReferenceNumber = "NT1", NewTrustName="New Trust", State = ProjectState.Active,
                          CreatedAt = DateTime.Parse("2021-01-01"), CompletedAt = DateTime.Parse("2021-02-01"),
                          SignificantDate = DateOnly.Parse("2021-01-15"), Region = Region.EastMidlands, Team = ProjectTeam.DataConsumers,
                          Type = ProjectType.Conversion,
                          GiasEstablishment = new GiasEstablishment { Id = new GiasEstablishmentId(Guid.NewGuid()) , LocalAuthorityCode = "LA123", Name = "Acme School", EstablishmentNumber="0001"} },
            new Project { AssignedToId = null, RegionalDeliveryOfficerId = new UserId(Guid.NewGuid()),
                          IncomingTrustUkprn = null, OutgoingTrustUkprn = null,
                          NewTrustReferenceNumber = null, NewTrustName=null, State = ProjectState.Completed,
                          CreatedAt = DateTime.Parse("2020-01-01"), CompletedAt = DateTime.Parse("2020-02-01"),
                          SignificantDate = DateOnly.Parse("2020-01-15"), Region = Region.WestMidlands, Team = ProjectTeam.DataConsumers,
                          Type = ProjectType.Transfer,
                          GiasEstablishment = new GiasEstablishment { Id = new GiasEstablishmentId(Guid.NewGuid()) , LocalAuthorityCode = "LA999", Name = "Beta Academy", EstablishmentNumber="1234"}}
        }.AsQueryable();

        [Fact]
        public void AssignedToStateQuery_AssignedOnly()
        {
            var q = new AssignedToStateQuery(AssignedToState.AssignedOnly).Apply(SampleProjects());
            Assert.All(q, p => Assert.NotNull(p.AssignedToId));
        }

        [Fact]
        public void AssignedToStateQuery_UnassignedOnly()
        {
            var q = new AssignedToStateQuery(AssignedToState.UnassignedOnly).Apply(SampleProjects());
            Assert.All(q, p => Assert.Null(p.AssignedToId));
        }

        [Fact]
        public void AssignedToStateQuery_Null()
        {
            var all = SampleProjects().ToList();
            var q = new AssignedToStateQuery(null).Apply(SampleProjects());
            Assert.Equal(all.Count, q.Count());
        }

        [Fact]
        public void AssignedToUserQuery_WithUser()
        {
            var sample = SampleProjects().ToList();
            var user = sample.First().AssignedToId;

            var q = new AssignedToUserQuery(user).Apply(sample.AsQueryable());

            Assert.Single(q);
            Assert.Equal(user, q.Single().AssignedToId);
        }

        [Fact]
        public void AssignedToUserQuery_Null()
        {
            var count = SampleProjects().Count();
            var q = new AssignedToUserQuery(null).Apply(SampleProjects());
            Assert.Equal(count, q.Count());
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 1)]
        [InlineData(null, 2)]
        public void FormAMatQuery_Various(bool? isForm, int expectedCount)
        {
            var q = new FormAMatQuery(isForm).Apply(SampleProjects());
            Assert.Equal(expectedCount, q.Count());
        }

        [Theory]
        [InlineData("12345678", 1)]
        [InlineData("", 2)]
        public void IncomingUkprnQuery_Various(string ukprn, int expected)
        {
            var q = new IncomingUkprnQuery(ukprn).Apply(SampleProjects());
            Assert.Equal(expected, q.Count());
        }

        [Theory]
        [InlineData("LA123", 1)]
        [InlineData("LA999", 1)]
        [InlineData("", 2)]
        public void LocalAuthorityCodeQuery_Various(string code, int expected)
        {
            var q = new LocalAuthorityCodeQuery(code).Apply(SampleProjects());
            Assert.Equal(expected, q.Count());
        }

        [Theory]
        [InlineData("NT1", 1)]
        [InlineData("XYZ", 0)]
        [InlineData("", 2)]
        public void NewTrustReferenceQuery_Various(string r, int expected)
        {
            var q = new NewTrustReferenceQuery(r).Apply(SampleProjects());
            Assert.Equal(expected, q.Count());
        }

        [Fact]
        public void OrderProjectsQuery_SortsBySignificantDate_Explicit()
        {
            var list = new[]
            {
                new Project { SignificantDate = DateOnly.Parse("2021-02-01") },
                new Project { SignificantDate = DateOnly.Parse("2021-01-01") }
            }.AsQueryable();

            // Tell the query object to sort by SignificantDate
            var orderBy = new OrderProjectQueryBy(
                Field: OrderProjectByField.SignificantDate,
                Direction: OrderByDirection.Ascending
            );

            var q = new OrderProjectsQuery(orderBy).Apply(list).ToList();

            var actual = q.Select(p => p.SignificantDate!.Value.ToString("yyyy-MM-dd")).ToArray();
            var expected = new[] { "2021-01-01", "2021-02-01" };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void OrderProjectsQuery_SortsByCreatedAt_Descending()
        {
            var list = new[]
            {
                new Project { CreatedAt = DateTime.Parse("2021-01-01") },
                new Project { CreatedAt = DateTime.Parse("2021-02-01") }
            }.AsQueryable();
            var param = new OrderProjectQueryBy(OrderProjectByField.CreatedAt, OrderByDirection.Descending);
            var q = new OrderProjectsQuery(param).Apply(list).ToList();
            Assert.Equal(["2021-02-01", "2021-01-01"],
                q.Select(p => p.CreatedAt.ToString("yyyy-MM-dd")).ToArray());
        }

        [Fact]
        public void RegionQuery_Various()
        {
            var list = new[]
            {
                new Project { Region = Region.EastMidlands },
                new Project { Region = Region.WestMidlands }
            }.AsQueryable();

            var filtered = new RegionQuery(Region.EastMidlands).Apply(list);
            Assert.Single(filtered);

            var all = new RegionQuery(null).Apply(list);
            Assert.Equal(2, all.Count());
        }

        [Theory]
        [InlineData(ProjectType.Conversion, 1)]
        [InlineData(ProjectType.Transfer, 1)]
        [InlineData(null, 2)]
        public void TypeQuery_Various(ProjectType? t, int expected)
        {
            var list = new[]
            {
                new Project { Type = ProjectType.Conversion },
                new Project { Type = ProjectType.Transfer }
            }.AsQueryable();
            var q = new TypeQuery(t).Apply(list);
            Assert.Equal(expected, q.Count());
        }

        [Theory]
        [InlineData(ProjectTeam.DataConsumers, 1)]
        [InlineData(null, 2)]
        public void TeamQuery_Various(ProjectTeam? team, int expected)
        {
            var list = new[]
            {
                new Project { Team = ProjectTeam.DataConsumers },
                new Project { Team = ProjectTeam.YorkshireAndTheHumber }
            }.AsQueryable();
            var q = new TeamQuery(team).Apply(list);
            Assert.Equal(expected, q.Count());
        }

        [Fact]
        public void StateQuery_Various()
        {
            var list = new[]
            {
                new Project { State = ProjectState.Active },
                new Project { State = ProjectState.Completed }
            }.AsQueryable();
            Assert.Single(new StateQuery(ProjectState.Active).Apply(list));
            Assert.Equal(2, new StateQuery(null).Apply(list).Count());
        }

        [Fact]
        public void PagingQuery_SkipsAndTakes()
        {
            var list = Enumerable.Range(1, 10)
                .Select(i => new Project { /*dummy*/ }).AsQueryable();
            var q = new PagingQuery<Project>(page: 1, count: 3).Apply(list);
            Assert.Equal(3, q.Count());
        }

        [Theory]
        [InlineData("123456", 1, "URN match")]       // 6-digit URN
        [InlineData("12345678", 1, "UKPRN match")]      // 8-digit UKPRN
        [InlineData("0001", 1, "Estab# match")]     // 4-digit EstablishmentNumber
        [InlineData("West", 1, "Name contains")]    // fallback name
        public void SearchQuery_Various(string term, int expected, string _)
        {
            var options = new DbContextOptionsBuilder<CompleteContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();

            var services = new ServiceCollection();
            services.AddEntityFrameworkInMemoryDatabase();
            services.AddSingleton<IMediator>(Substitute.For<IMediator>());
            var serviceProvider = services.BuildServiceProvider();

            using var ctx = new CompleteContext(options, configuration, serviceProvider);

            var estab = new GiasEstablishment
            {
                Id = new GiasEstablishmentId(Guid.NewGuid()),
                Urn = new Urn(123456),
                EstablishmentNumber = "0001",
                Name = "West School"
            };
            ctx.GiasEstablishments.Add(estab);

            ctx.Projects.Add(new Project
            {
                Urn = new Urn(123456),
                IncomingTrustUkprn = new Ukprn(12345678),
                LocalAuthorityId = new LocalAuthorityId(Guid.NewGuid()),
                RegionalDeliveryOfficerId = new UserId(Guid.NewGuid()),
                GiasEstablishment = estab
            });

            ctx.SaveChanges();

            var q = new SearchQuery(term)
                .Apply(ctx.Projects
                    .Include(p => p.GiasEstablishment)
                    .AsNoTracking());

            Assert.Equal(expected, q.Count());
        }
    
}
}
