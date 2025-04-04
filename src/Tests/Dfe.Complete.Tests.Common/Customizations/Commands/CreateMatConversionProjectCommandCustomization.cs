using AutoFixture;
using Dfe.Complete.Application.Projects.Commands.CreateProject;
using Ukprn = Dfe.Complete.Domain.ValueObjects.Ukprn;
using Urn = Dfe.Complete.Domain.ValueObjects.Urn;

namespace Dfe.Complete.Tests.Common.Customizations.Commands
{
    public class CreateMatConversionProjectCommandCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<CreateMatConversionProjectCommand>(composer => composer.FromFactory(() =>
            {
                var urn = new Urn(fixture.Create<int>());
                var newTrustName = fixture.Create<string>();
                var newTrustReferenceNumber = fixture.Create<string>();
                var significantDate = fixture.Create<DateOnly>();
                var isSignificantDateProvisional = fixture.Create<bool>();
                var isDueTo2Ri = fixture.Create<bool>();
                var hasAcademyOrderBeenIssued = fixture.Create<bool>();
                var advisoryBoardDate = fixture.Create<DateOnly>();
                var advisoryBoardConditions = fixture.Create<string>();
                var establishmentSharepointLink = fixture.Create<Uri>().ToString();
                var incomingTrustSharepointLink = fixture.Create<Uri>().ToString();
                var handingOverToRegionalCaseworkService = fixture.Create<bool>();
                var handoverComments = fixture.Create<string>();
                var userAdId = fixture.Create<string>();
                
                return new CreateMatConversionProjectCommand(
                    urn,
                    newTrustName,
                    newTrustReferenceNumber,
                    significantDate,
                    isSignificantDateProvisional,
                    isDueTo2Ri,
                    hasAcademyOrderBeenIssued,
                    advisoryBoardDate,
                    advisoryBoardConditions,
                    establishmentSharepointLink,
                    incomingTrustSharepointLink,
                    handingOverToRegionalCaseworkService,
                    handoverComments,
                    userAdId
                );
            }));
        }
    }
}