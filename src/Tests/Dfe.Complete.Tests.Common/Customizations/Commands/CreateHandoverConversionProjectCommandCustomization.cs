using AutoFixture;
using CreateHandoverConversionProjectCommand = Dfe.Complete.Application.Projects.Commands.CreateHandoverProject.CreateHandoverConversionProjectCommand;

namespace Dfe.Complete.Tests.Common.Customizations.Commands
{
    public class CreateHandoverConversionProjectCommandCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<CreateHandoverConversionProjectCommand>(composer => composer.FromFactory(() =>
            {
                var urn = fixture.Create<int>();
                var incomingTrustUkprn = fixture.Create<int>();
                var advisoryBoardDate = fixture.Create<DateOnly>();
                var provisionalConversionDate = fixture.Create<DateOnly>();
                var createdByEmail = fixture.Create<string>();
                var createdByFirstName = fixture.Create<string>();
                var createdByLastName = fixture.Create<string>();
                var prepareId = fixture.Create<int>();
                var directiveAcademyOrder = fixture.Create<bool>();
                var advisoryBoardConditions = fixture.Create<string>();
                var groupReferenceNumber = fixture.Create<string>();
                
                return new CreateHandoverConversionProjectCommand(
                    urn,
                    incomingTrustUkprn,
                    advisoryBoardDate,
                    provisionalConversionDate,
                    createdByEmail,
                    createdByFirstName,
                    createdByLastName,
                    prepareId,
                    directiveAcademyOrder,
                    advisoryBoardConditions,
                    groupReferenceNumber
                );
            }));
        }
    }
}
