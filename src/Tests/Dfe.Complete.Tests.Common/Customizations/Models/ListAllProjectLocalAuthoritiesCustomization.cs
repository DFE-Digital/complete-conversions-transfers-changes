using System.Linq.Expressions;
using AutoFixture;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using MockQueryable;
using NSubstitute;

namespace Dfe.Complete.Tests.Common.Customizations.Models
{
    public class ListAllProjectLocalAuthoritiesArrangementCustomization : ICustomization
    {
        private readonly int projectsToAssignCount;
        
        public ListAllProjectLocalAuthoritiesArrangementCustomization(int projectsToAssignCount)
        {
            this.projectsToAssignCount = projectsToAssignCount;
        }

        public void Customize(IFixture fixture)
        {
            var localAuthorities = fixture.CreateMany<LocalAuthority>(20).ToList();
            var expectedLocalAuthorityCodes = localAuthorities.Select(la => la.Code).ToList();

            var localAuthoritiesRepo = fixture.Freeze<ICompleteRepository<LocalAuthority>>();
            localAuthoritiesRepo
                .FetchAsync(Arg.Any<Expression<Func<LocalAuthority, bool>>>(), Arg.Any<CancellationToken>())
                .Returns(localAuthorities.ToList());

            var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(50).ToList();

            listAllProjectsQueryModels
                .Take(projectsToAssignCount)
                .ToList()
                .ForEach(p =>
                    p.Establishment.LocalAuthorityCode = expectedLocalAuthorityCodes.MinBy(_ => Guid.NewGuid()));

            var mockListAllProjectsQueryService = fixture.Freeze<IListAllProjectsQueryService>();
            var mockListAllProjects = listAllProjectsQueryModels.BuildMock();
            mockListAllProjectsQueryService
                .ListAllProjects(Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>())
                .Returns(mockListAllProjects);

            var expectedProjects = listAllProjectsQueryModels
                .Where(p => expectedLocalAuthorityCodes.Contains(p.Establishment.LocalAuthorityCode))
                .ToList();

            var expectedLocalAuthorities = localAuthorities
                .Where(la => expectedLocalAuthorityCodes.Contains(la.Code))
                .Select(la => new ListAllProjectLocalAuthoritiesResultModel(
                    la,
                    la.Code,
                    expectedProjects.Count(p => p.Establishment.LocalAuthorityCode == la.Code &&
                                                p.Project.Type == ProjectType.Conversion),
                    expectedProjects.Count(p => p.Establishment.LocalAuthorityCode == la.Code &&
                                                p.Project.Type == ProjectType.Transfer)))
                .ToList();

            fixture.Inject(localAuthorities);
            fixture.Inject(expectedLocalAuthorityCodes);
            fixture.Inject(listAllProjectsQueryModels);
            fixture.Inject(expectedProjects);
            fixture.Inject(expectedLocalAuthorities);
        }
    }
}
