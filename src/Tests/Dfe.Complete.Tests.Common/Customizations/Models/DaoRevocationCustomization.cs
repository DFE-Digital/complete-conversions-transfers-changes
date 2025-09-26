using AutoFixture;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Tests.Common.Customizations.Models;

public class DaoRevocationCustomization : ICustomization
{
    public ProjectId? ProjectId { get; set; }
    public string? DecisionMakersName { get; set; }

    public DateOnly? DateOfDecision { get; set; }

    public void Customize(IFixture fixture)
    {
        fixture.Customize<DaoRevocationId>(c =>
            c.FromFactory<Guid>(guid => new DaoRevocationId(guid)));

        fixture
            .Customize<DaoRevocation>(composer => composer
                .With(x => x.ProjectId, ProjectId ?? fixture.Create<ProjectId>())
                .With(x => x.DecisionMakersName, DecisionMakersName ?? fixture.Create<string>())
                .With(x => x.DateOfDecision, DateOfDecision ?? fixture.Create<DateOnly>())
                 .With(x => x.CreatedAt, fixture.Create<DateTime>())
                 .With(x => x.UpdatedAt, fixture.Create<DateTime>())
                .Do(x => x.Id = fixture.Create<DaoRevocationId>()));
    }
}