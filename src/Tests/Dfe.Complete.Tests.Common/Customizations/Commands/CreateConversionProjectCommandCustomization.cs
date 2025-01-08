using AutoFixture;
using Dfe.Complete.Client.Contracts;
using Dfe.Complete.Domain.Enums;
using CreateConversionProjectCommand = Dfe.Complete.Application.Projects.Commands.CreateProject.CreateConversionProjectCommand;
using ProjectTeam = Dfe.Complete.Domain.Enums.ProjectTeam;
using Region = Dfe.Complete.Domain.Enums.Region;
using Ukprn = Dfe.Complete.Domain.ValueObjects.Ukprn;
using Urn = Dfe.Complete.Domain.ValueObjects.Urn;
using User = Dfe.Complete.Domain.Entities.User;

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
                var groupReferenceNumber = fixture.Create<string>();
                var handingOverToRegionalCaseworkService = fixture.Create<bool>();
                var handoverComments = fixture.Create<string>();
                var regionalDeliveryOfficer = fixture.Create<User>();
                var team = fixture.Create<ProjectTeam>();
                
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
                    groupReferenceNumber,
                    handingOverToRegionalCaseworkService,
                    handoverComments,
                    regionalDeliveryOfficer, 
                    team
                );
            }));
        }
    }
}