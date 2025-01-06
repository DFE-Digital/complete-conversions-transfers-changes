using AutoFixture;
using Dfe.Complete.Application.Projects.Commands.CreateProject;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Tests.Common.Customizations.Commands
{
    public class CreateConversionProjectCommandCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<CreateConversionProjectCommand>(composer => composer.FromFactory(() =>
            {
                var urn = new Urn(fixture.Create<int>());
                var significantDate = fixture.Create<DateOnly>();
                var isSignificantDateProvisional = fixture.Create<bool>();
                var incomingTrustUkprn = new Ukprn(fixture.Create<int>());
                var region = fixture.Create<Region>();
                var isDueTo2Ri = fixture.Create<bool>();
                var hasAcademyOrderBeenIssued = fixture.Create<bool>();
                var advisoryBoardDate = fixture.Create<DateOnly>();
                var advisoryBoardConditions = fixture.Create<string>();
                var establishmentSharepointLink = fixture.Create<Uri>().ToString();
                var incomingTrustSharepointLink = fixture.Create<Uri>().ToString();

                return new CreateConversionProjectCommand(
                    urn,
                    significantDate,
                    isSignificantDateProvisional,
                    incomingTrustUkprn,
                    region,
                    isDueTo2Ri,
                    hasAcademyOrderBeenIssued,
                    advisoryBoardDate,
                    advisoryBoardConditions,
                    establishmentSharepointLink,
                    incomingTrustSharepointLink, 
                );
            }));
        }
    }
}