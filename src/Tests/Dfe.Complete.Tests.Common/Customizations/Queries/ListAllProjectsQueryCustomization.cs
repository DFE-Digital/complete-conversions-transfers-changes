using AutoFixture;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Tests.Common.Customizations.Queries;

public class ListAllProjectsQueryCustomization : ICustomization
{
    public ProjectState? ProjectStatus { get; set; }
    public ProjectType? Type { get; set; }
    public int Page { get; set; }
    public int Count { get; set; }

    public void Customize(IFixture fixture)
    {
        fixture.Customize<ListAllProjectsQuery>(composer => composer
            .With(x => x.ProjectStatus, ProjectStatus)
            .With(x => x.Type, Type)
            .With(x => x.Page, 0)
            .With(x => x.Count, 20));
    }
}