using AutoFixture;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;

namespace Dfe.Complete.Tests.Common.Customizations.Models
{
    public class SignificantDateHistoryReasonCustomization : ICustomization
    {
        public SignificantDateHistoryReasonId? SignificantDateHistoryReasonId { get; set; }
        public string? ReasonType { get; set; }

        public SignificantDateHistoryId? SignificantDateHistoryId { get; set; }

        public void Customize(IFixture fixture)
        {
            fixture.Customize(new CompositeCustomization(
                    new DateOnlyCustomization()))
                .Customize<SignificantDateHistoryReason>(composer => composer
                    .With(x => x.Id, SignificantDateHistoryReasonId ?? fixture.Create<SignificantDateHistoryReasonId>())
                    .With(x => x.ReasonType, ReasonType ?? fixture.Create<string?>())
                    .With(x => x.SignificantDateHistoryId, fixture.Create<SignificantDateHistoryId>()));
        }
    }
}