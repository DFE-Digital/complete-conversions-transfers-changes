using AutoFixture;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using Guid = System.Guid;

namespace Dfe.Complete.Tests.Common.Customizations.Models
{
    public class NoteCustomization : ICustomization
    {
        public NoteId Id { get; set; } = default!;
        public ProjectId ProjectId { get; set; } = default!;
        public UserId UserId { get; set; } = default!;
        public string Body { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? TaskIdentifier { get; set; } = default!;
        public Guid? NotableId { get; set; } = default!;
        public string? NotableType { get; set; } = default!;

        public void Customize(IFixture fixture)
        {
            fixture.Customize(new CompositeCustomization(
                    new DateOnlyCustomization(),
                    new IgnoreVirtualMembersCustomisation()))
                .Customize<Note>(composer => composer
                    .With(x => x.Id, () => Id ?? new NoteId(fixture.Create<Guid>()))
                    .With(x => x.UserId, () => UserId ?? fixture.Create<UserId>())
                    .With(x => x.ProjectId, () => ProjectId ?? fixture.Create<ProjectId>())
                    .With(x => x.Body, () => Body ?? fixture.Create<string>())
                    .With(x => x.CreatedAt, () => CreatedAt == default ? fixture.Create<DateTime>() : CreatedAt)
                    .With(x => x.UpdatedAt, () => UpdatedAt == default ? fixture.Create<DateTime>() : UpdatedAt)
                    .With(x => x.NotableId, (Guid?)null)
                    .With(x => x.NotableType, (string?)null)
                    .With(x => x.TaskIdentifier, (string?)null)
                );
        }
    }
}