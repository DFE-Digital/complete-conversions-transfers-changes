using AutoFixture;
using Dfe.Complete.Application.Projects.Commands.CreateProject;
using Ukprn = Dfe.Complete.Domain.ValueObjects.Ukprn;
using Urn = Dfe.Complete.Domain.ValueObjects.Urn;

namespace Dfe.Complete.Tests.Common.Customizations.Commands
{
    public class CreateTransferProjectCommandCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<CreateTransferProjectCommand>(composer => composer.FromFactory(() =>
            {
                var urn = new Urn(fixture.Create<int>());
                var outgoingTrustUkprn = new Ukprn(fixture.Create<int>());
                var incomingTrustUkprn = new Ukprn(fixture.Create<int>());
                var significantDate = fixture.Create<DateOnly>();
                var isSignificantDateProvisional = fixture.Create<bool>();
                var isDueTo2Ri = fixture.Create<bool>();
                var isDueToInedaquateOfstedRating = fixture.Create<bool>();
                var isDueToIssues = fixture.Create<bool>();
                var outGoingTrustWillClose = fixture.Create<bool>();
                var handingOverToRegionalCaseworkService = fixture.Create<bool>();
                var advisoryBoardDate = fixture.Create<DateOnly>();
                var advisoryBoardConditions = fixture.Create<string>();
                var establishmentSharepointLink = fixture.Create<Uri>().ToString();
                var incomingTrustSharepointLink = fixture.Create<Uri>().ToString();
                var outgoingTrustSharepointLink = fixture.Create<Uri>().ToString();
                var groupReferenceNumber = fixture.Create<string>();
                var handoverComments = fixture.Create<string>();
                var userAdId = fixture.Create<string>();
                
                return new CreateTransferProjectCommand(
                    urn,
                    outgoingTrustUkprn,
                    incomingTrustUkprn,
                    significantDate,
                    isSignificantDateProvisional,
                    isDueTo2Ri,
                    isDueToInedaquateOfstedRating,
                    isDueToIssues,
                    outGoingTrustWillClose,
                    handingOverToRegionalCaseworkService,
                    advisoryBoardDate,
                    advisoryBoardConditions,
                    establishmentSharepointLink,
                    incomingTrustSharepointLink,
                    outgoingTrustSharepointLink,
                    groupReferenceNumber,
                    handoverComments,
                    userAdId
                );
            }));
        }
    }
}