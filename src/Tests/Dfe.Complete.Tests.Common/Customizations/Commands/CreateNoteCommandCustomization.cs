using AutoFixture;
using Dfe.Complete.Application.Notes.Commands;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Tests.Common.Customizations.Commands;

public class CreateNoteCommandCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<CreateNoteCommand>(composer => composer.FromFactory(() =>
        {
            var projectId = fixture.Create<ProjectId>();
            var userId = fixture.Create<UserId>();
            var body = fixture.Create<string>();

            return new CreateNoteCommand(projectId, userId, body);
        }));
    }
}
