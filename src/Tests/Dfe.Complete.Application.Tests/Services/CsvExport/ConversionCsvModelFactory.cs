using AutoFixture;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using Microsoft.Identity.Client;

namespace Dfe.Complete.Application.Tests.Services.CsvExport
{
    public static class ConversionCsvModelFactory
    {
        public static ConversionCsvModel Make(bool withAcademy = true,
                                              bool withSignificantDateHistory = true,
                                              bool withMainContact = true,
                                              bool withHeadteacher = true,
                                              bool withLAContact = true)
        {
            var fixture = new Fixture();   

            var establishment = fixture.Customize(
                new EstablishmentsCustomization()
                ).Create<GiasEstablishment>();

            var academy = fixture.Customize(
                new EstablishmentsCustomization()
                ).Create<GiasEstablishment>();

            var laContact = fixture.Customize(new OmitCircularReferenceCustomization()).Create<Contact>();

            var localAuthority = fixture.Customize(
                new LocalAuthorityCustomization()
                {
                    LocalAuthorityCode = establishment.LocalAuthorityCode
                }
                ).Create<LocalAuthority>();

            var conversionTasksData = fixture.Create<ConversionTasksData>();

            fixture.Customizations.Add(new IgnoreVirtualMembers());
            var createdBy = fixture.Create<User>();
            var assignedTo = fixture.Create<User>();

            var mainContact = fixture.Customize(new OmitCircularReferenceCustomization()).Create<Contact>();
            var headteacher = fixture.Customize(new OmitCircularReferenceCustomization()).Create<Contact>();

            var project = fixture.Customize(
                new ProjectCustomization()
                {
                    IncomingTrustUkprn = establishment.Ukprn,
                    AcademyUrn = withAcademy ? academy.Urn : null,
                    RegionalDeliveryOfficerId = createdBy.Id,
                    AssignedToId = assignedTo.Id,
                    MainContactId = withMainContact ? mainContact.Id : null,
                }
                ).Create<Project>();

            var significantDateHistory = fixture.Customize(
                new SignificantDateHistoryCustomization()
                { 
                    ProjectId = project.Id
                }
                ).Create<SignificantDateHistory>();


            return new ConversionCsvModel(project,
                                          establishment,
                                          withAcademy ? academy : null,
                                          localAuthority,
                                          withSignificantDateHistory ? significantDateHistory : null,
                                          conversionTasksData,
                                          createdBy,
                                          assignedTo,
                                          withMainContact ? mainContact : null,
                                          withHeadteacher ?  headteacher : null,
                                          withLAContact ? laContact : null);
        }
    }
}
