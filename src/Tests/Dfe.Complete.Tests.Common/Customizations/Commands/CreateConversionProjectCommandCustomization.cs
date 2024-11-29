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
                var urn = fixture.Create<int>();
                var createdAt = fixture.Create<DateTime>().ToUniversalTime(); 
                var updatedAt = createdAt.AddMinutes(fixture.Create<int>() % 100);
                var taskType = fixture.Create<TaskType>();
                var projectType = fixture.Create<ProjectType>();
                var tasksDataId = fixture.Create<Guid>();
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
                    new Urn(urn),
                    createdAt,
                    updatedAt,
                    taskType,
                    projectType,
                    tasksDataId,
                    significantDate,
                    isSignificantDateProvisional,
                    incomingTrustUkprn,
                    region,
                    isDueTo2Ri,
                    hasAcademyOrderBeenIssued,
                    advisoryBoardDate,
                    advisoryBoardConditions,
                    establishmentSharepointLink,
                    incomingTrustSharepointLink
                );
            }));
        }
    }
}
