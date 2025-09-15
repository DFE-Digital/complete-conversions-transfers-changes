using AutoFixture;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;

namespace Dfe.Complete.Tests.Common.Customizations.Models
{
    public class SignificantDateHistoryCustomization : ICustomization
    {
        public SignificantDateHistoryId? Id { get; set; }

        public ProjectId? ProjectId { get; set; }
        public User? User { get; set; }

        public void Customize(IFixture fixture)
        {
            fixture.Customize(new CompositeCustomization(
                    new DateOnlyCustomization()))
                .Customize<SignificantDateHistory>(composer => composer
                    .With(x => x.Id, Id ?? fixture.Create<SignificantDateHistoryId>())
                    .With(x => x.ProjectId, ProjectId ?? fixture.Create<ProjectId>())
                    .With(x => x.User, User ?? fixture.Create<User>())
                    .With(x => x.Reasons, fixture.Create<ICollection<SignificantDateHistoryReason>>()));
        }
    }
}