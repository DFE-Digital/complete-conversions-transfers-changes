using AutoFixture;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Tests.Common.Customizations.Queries;

public class CountAllProjectsQueryCustomization : ICustomization
{
    public ProjectState? ProjectStatus { get; set; }
    public ProjectType? Type{ get; set; }

    public void Customize(IFixture fixture)
    {
        fixture.Customize<ListAllProjectsQuery>(composer => composer
            .With(x => x.ProjectStatus, ProjectStatus)
            .With(x => x.Type, Type));
    }
}