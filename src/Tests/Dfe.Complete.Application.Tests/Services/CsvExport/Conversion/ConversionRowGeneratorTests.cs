using AutoFixture;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Services.CsvExport.Builders;
using Dfe.Complete.Application.Services.CsvExport.Conversion;
using Dfe.Complete.Application.Services.TrustService;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Events;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Models;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using NSubstitute;
namespace Dfe.Complete.Application.Tests.Services.CsvExport.Conversion
{
    public class ConversionRowGeneratorTests
    {


        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(EstablishmentsCustomization), typeof(TrustDetailsDtoCustomization))]
        public void RowGeneratesAccountsForBlankData(Project project,
                                                     GiasEstablishment currentSchool,
                                                     TrustDetailsDto incomingTrust,
                                                     LocalAuthority localAuthority,
                                                     ConversionTasksData taskData)
        { 
            project.Type = ProjectType.Conversion;
            project.AcademyUrn = null;
            project.IncomingTrustUkprn = incomingTrust.Ukprn;
            project.SignificantDateProvisional = true;
            project.DirectiveAcademyOrder = true;
            project.TwoRequiresImprovement = true;
            project.AdvisoryBoardConditions = null;
            project.AllConditionsMet = false;
            currentSchool.PhaseName = "Not applicable";

            taskData.ReceiveGrantPaymentCertificateDateReceived = null;

            var TrustCache = Substitute.For<ITrustCache>();
            TrustCache.GetTrustAsync(incomingTrust.Ukprn).Returns(incomingTrust);

            var model = new ConversionCsvModel(project, currentSchool, null, localAuthority, null, taskData);
          
            var generator = new ConversionRowGenerator(new RowBuilderFactory<ConversionCsvModel>(TrustCache));

            generator.GenerateRow(model);

            var result = generator.GenerateRow(model).Split(",");

            Assert.Equal(currentSchool.Name, result[0]);
            Assert.Equal(project.Urn.ToString(), result[1]);
            Assert.Equal("Conversion", result[2]);
            Assert.Equal("unconfirmed", result[3]);
            Assert.Equal("unconfirmed", result[4]);
            Assert.Equal("", result[5]);
            Assert.Equal(incomingTrust.Name, result[6]);
            Assert.Equal(localAuthority.Name, result[7]);
            Assert.Equal(currentSchool.RegionName, result[8]);
            Assert.Equal(currentSchool.DioceseName, result[9]);
            Assert.Equal(project.SignificantDate.Value.ToString("dd/MM/yyyy"), result[10]);
            Assert.Equal("unconfirmed", result[11]); 
            Assert.Equal("directive academy order", result[12]);
            Assert.Equal("yes", result[13]);
            Assert.Equal(project.AdvisoryBoardDate.Value.ToString("dd/MM/yyyy"), result[14]);
            Assert.Equal("", result[15]);
            Assert.Equal("standard", result[16]);
            Assert.Equal("not applicable", result[17]);
            Assert.Equal("no", result[18]);
            Assert.Equal("unconfirmed", result[19]);
            Assert.Equal(currentSchool.TypeName, result[20]);
            Assert.Equal(currentSchool.AgeRangeLower + "-" + currentSchool.AgeRangeUpper, result[21]);
            Assert.Equal(currentSchool.TypeName, result[22]);
            //Assert.Equal("123", result[23]);
            //Assert.Equal("456", result[24]);
            //Assert.Equal("789", result[25]);
            //Assert.Equal("SchoolAddress1", result[26]);
            //Assert.Equal("SchoolAddress2", result[27]);
            //Assert.Equal("SchoolAddress3", result[28]);
            //Assert.Equal("SchoolTown", result[29]);
            //Assert.Equal("SchoolCounty", result[30]);
            //Assert.Equal("SchoolPostcode", result[31]);
            //Assert.Equal("SchoolSharepointFolder", result[32]);
            //Assert.Equal("ConversionType", result[33]);
            //Assert.Equal("12345647", result[34]);
            //Assert.Equal("IncomingTrustGroupIdentifier", result[35]);
            //Assert.Equal("IncomingTrustCompaniesHouseNumber", result[36]);
            //Assert.Equal("IncomingTrustAddress1", result[37]);
            //Assert.Equal("IncomingTrustAddress2", result[38]);
            //Assert.Equal("IncomingTrustAddress3", result[39]);
            //Assert.Equal("IncomingTrustAddressTown", result[40]);
            //Assert.Equal("IncomingTrustAddressCounty", result[41]);
            //Assert.Equal("IncomingTrustAddressPostcode", result[42]);
            //Assert.Equal("IncomingTrustSharepointLink", result[43]);
            //Assert.Equal("ProjectCreatedBy", result[44]);
            //Assert.Equal("ProjectCreatedByEmailAddress", result[45]);
            //Assert.Equal("AssignedToName", result[46]);
            //Assert.Equal("TeamManagingTheProject", result[47]);
            //Assert.Equal("ProjectMainContactName", result[48]);
            //Assert.Equal("HeadteacherName", result[49]);
            //Assert.Equal("HeadteacherRole", result[50]);
            //Assert.Equal("HeadteacherEmail", result[51]);
            //Assert.Equal("LocalAuthorityContactName", result[52]);
            //Assert.Equal("LocalAuthorityContactEmail", result[53]);
            //Assert.Equal("PrimaryContactForIncomingTrustName", result[54]);
            //Assert.Equal("PrimaryContactForIncomingTrustEmail", result[55]);
            //Assert.Equal("PrimaryContactForOutgoingTrustName", result[56]);
            //Assert.Equal("PrimaryContactForOutgoingTrustEmail", result[57]);
            //Assert.Equal("IncomingTrustCEOName", result[58]);
            //Assert.Equal("IncomingTrustCEORole", result[59]);
            //Assert.Equal("IncomingTrustCEOEmail", result[60]);
            //Assert.Equal("SolicitorContactName", result[61]);
            //Assert.Equal("SolicitorContactEmail", result[62]);
            //Assert.Equal("DioceseContactName", result[63]);
            //Assert.Equal("DioceseContactEmail", result[64]);
            //Assert.Equal("DirectorOfChildServicesName", result[65]);
            //Assert.Equal("DirectorOfChildServicesEmail", result[66]);
            //Assert.Equal("DirectorOfChildServicesRole", result[67]);
        }

        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(EstablishmentsCustomization), typeof(TrustDetailsDtoCustomization), typeof(LocalAuthorityCustomization), typeof(SignificantDateHistoryCustomization))]
        public void RowGeneratesBasedOnModel(Project project,
                                             GiasEstablishment currentSchool,
                                             GiasEstablishment academy,
                                             TrustDetailsDto incomingTrust,
                                             LocalAuthority localAuthority,
                                             SignificantDateHistory significantDateHistory,
                                             ConversionTasksData taskData)
        {
            project.Type = ProjectType.Conversion;
            project.IncomingTrustUkprn = incomingTrust.Ukprn;
            project.SignificantDateProvisional = false;
            project.TwoRequiresImprovement = false;
            project.DirectiveAcademyOrder = false;
            project.AllConditionsMet = true;

            taskData.RiskProtectionArrangementOption = RiskProtectionArrangementOption.Commercial;

            var model = new ConversionCsvModel(project, currentSchool, academy, localAuthority, significantDateHistory, taskData);

            var TrustCache = Substitute.For<ITrustCache>();
            TrustCache.GetTrustAsync(incomingTrust.Ukprn).Returns(incomingTrust);

            var generator = new ConversionRowGenerator(new RowBuilderFactory<ConversionCsvModel>(TrustCache));

            generator.GenerateRow(model);

            var result = generator.GenerateRow(model).Split(",");

            Assert.Equal(currentSchool.Name, result[0]);
            Assert.Equal(project.Urn.ToString(), result[1]);
            Assert.Equal("Conversion", result[2]);
            Assert.Equal(academy.Name, result[3]);
            Assert.Equal(academy.Urn.ToString(), result[4]);
            Assert.Equal(academy.LocalAuthorityCode + "/" + academy.EstablishmentNumber, result[5]);
            Assert.Equal(incomingTrust.Name, result[6]);
            Assert.Equal(localAuthority.Name, result[7]);
            Assert.Equal(currentSchool.RegionName, result[8]);
            Assert.Equal(currentSchool.DioceseName, result[9]);
            Assert.Equal(significantDateHistory.PreviousDate.Value.ToString("dd/MM/yyyy"), result[10]);
            Assert.Equal(project.SignificantDate.Value.ToString("dd/MM/yyyy"), result[11]);
            Assert.Equal("academy order", result[12]);
            Assert.Equal("no", result[13]);
            Assert.Equal(project.AdvisoryBoardDate.Value.ToString("dd/MM/yyyy"), result[14]);
            Assert.Equal(project.AdvisoryBoardConditions, result[15]);
            Assert.Equal("commercial", result[16]);
            Assert.Equal(taskData.RiskProtectionArrangementReason, result[17]);
            Assert.Equal("yes", result[18]);
            Assert.Equal(taskData.ReceiveGrantPaymentCertificateDateReceived?.ToString("dd/MM/yyyy"), result[19]);
            Assert.Equal(currentSchool.TypeName, result[20]);
            Assert.Equal(currentSchool.AgeRangeLower + "-" + currentSchool.AgeRangeUpper, result[21]);
            Assert.Equal(currentSchool.PhaseName, result[22]);
            //Assert.Equal("123", result[23]);
            //Assert.Equal("456", result[24]);
            //Assert.Equal("789", result[25]);
            //Assert.Equal("SchoolAddress1", result[26]);
            //Assert.Equal("SchoolAddress2", result[27]);
            //Assert.Equal("SchoolAddress3", result[28]);
            //Assert.Equal("SchoolTown", result[29]);
            //Assert.Equal("SchoolCounty", result[30]);
            //Assert.Equal("SchoolPostcode", result[31]);
            //Assert.Equal("SchoolSharepointFolder", result[32]);
            //Assert.Equal("ConversionType", result[33]);
            //Assert.Equal("12345647", result[34]);
            //Assert.Equal("IncomingTrustGroupIdentifier", result[35]);
            //Assert.Equal("IncomingTrustCompaniesHouseNumber", result[36]);
            //Assert.Equal("IncomingTrustAddress1", result[37]);
            //Assert.Equal("IncomingTrustAddress2", result[38]);
            //Assert.Equal("IncomingTrustAddress3", result[39]);
            //Assert.Equal("IncomingTrustAddressTown", result[40]);
            //Assert.Equal("IncomingTrustAddressCounty", result[41]);
            //Assert.Equal("IncomingTrustAddressPostcode", result[42]);
            //Assert.Equal("IncomingTrustSharepointLink", result[43]);
            //Assert.Equal("ProjectCreatedBy", result[44]);
            //Assert.Equal("ProjectCreatedByEmailAddress", result[45]);
            //Assert.Equal("AssignedToName", result[46]);
            //Assert.Equal("TeamManagingTheProject", result[47]);
            //Assert.Equal("ProjectMainContactName", result[48]);
            //Assert.Equal("HeadteacherName", result[49]);
            //Assert.Equal("HeadteacherRole", result[50]);
            //Assert.Equal("HeadteacherEmail", result[51]);
            //Assert.Equal("LocalAuthorityContactName", result[52]);
            //Assert.Equal("LocalAuthorityContactEmail", result[53]);
            //Assert.Equal("PrimaryContactForIncomingTrustName", result[54]);
            //Assert.Equal("PrimaryContactForIncomingTrustEmail", result[55]);
            //Assert.Equal("PrimaryContactForOutgoingTrustName", result[56]);
            //Assert.Equal("PrimaryContactForOutgoingTrustEmail", result[57]);
            //Assert.Equal("IncomingTrustCEOName", result[58]);
            //Assert.Equal("IncomingTrustCEORole", result[59]);
            //Assert.Equal("IncomingTrustCEOEmail", result[60]);
            //Assert.Equal("SolicitorContactName", result[61]);
            //Assert.Equal("SolicitorContactEmail", result[62]);
            //Assert.Equal("DioceseContactName", result[63]);
            //Assert.Equal("DioceseContactEmail", result[64]);
            //Assert.Equal("DirectorOfChildServicesName", result[65]);
            //Assert.Equal("DirectorOfChildServicesEmail", result[66]);
            //Assert.Equal("DirectorOfChildServicesRole", result[67]);
        }
    }
}
