using AutoFixture;
using Dfe.Complete.Application.Projects.Interfaces;
using NSubstitute;

namespace Dfe.Complete.Tests.Common.Customizations.Models;

public class ProjectsQueryBuilderCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<IProjectsQueryBuilder>(composer =>
            composer.FromFactory(() => Substitute.For<IProjectsQueryBuilder>()));
    }
}