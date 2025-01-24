using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Services.CsvExport.Builders;
using Dfe.Complete.Application.Services.CsvExport.Conversion;
using Dfe.Complete.Application.Services.TrustService;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using NSubstitute;
namespace Dfe.Complete.Application.Tests.Services.CsvExport.Conversion
{
    public class ConversionRowGeneratorTests
    {


        [Theory]
        [CustomAutoData(typeof(TrustDetailsDtoCustomization))]
        public void RowGeneratesAccountsForBlankData(TrustDetailsDto incomingTrust)
        {
            var model = ConversionCsvModelFactory.Make(withAcademy: false, withSignificantDateHistory: false);

            model.Project.Type = ProjectType.Conversion;
            model.Project.AcademyUrn = null;
            model.Project.IncomingTrustUkprn = null;
            model.Project.SignificantDateProvisional = true;
            model.Project.DirectiveAcademyOrder = true;
            model.Project.TwoRequiresImprovement = true;
            model.Project.AdvisoryBoardConditions = null;
            model.Project.AllConditionsMet = false;
            model.Project.EstablishmentSharepointLink = null;

            model.CurrentSchool.PhaseName = "Not applicable";
            model.CurrentSchool.AddressStreet = null;
            model.CurrentSchool.AddressLocality = null;
            model.CurrentSchool.AddressAdditional = null;
            model.CurrentSchool.AddressTown = null;
            model.CurrentSchool.AddressCounty = null;
            model.CurrentSchool.AddressPostcode = null;

            model.ConversionTasks.ReceiveGrantPaymentCertificateDateReceived = null;
            model.ConversionTasks.ProposedCapacityOfTheAcademyReceptionToSixYears = null;
            model.ConversionTasks.ProposedCapacityOfTheAcademySevenToElevenYears = null;
            model.ConversionTasks.ProposedCapacityOfTheAcademyTwelveOrAboveYears = null;

            var TrustCache = Substitute.For<ITrustCache>();
            TrustCache.GetTrustByTrnAsync(model.Project.NewTrustReferenceNumber).Returns(incomingTrust);


            var generator = new ConversionRowGenerator(new RowBuilderFactory<ConversionCsvModel>(TrustCache));

            generator.GenerateRow(model);

            var result = generator.GenerateRow(model).Split(",");

            Assert.Equal(model.CurrentSchool.Name, result[0]);
            Assert.Equal(model.Project.Urn.ToString(), result[1]);
            Assert.Equal("Conversion", result[2]);
            Assert.Equal("unconfirmed", result[3]);
            Assert.Equal("unconfirmed", result[4]);
            Assert.Equal("", result[5]);
            Assert.Equal(incomingTrust.Name, result[6]);
            Assert.Equal(model.LocalAuthority.Name, result[7]);
            Assert.Equal(model.CurrentSchool.RegionName, result[8]);
            Assert.Equal(model.CurrentSchool.DioceseName, result[9]);
            Assert.Equal(model.Project.SignificantDate.Value.ToString("dd/MM/yyyy"), result[10]);
            Assert.Equal("unconfirmed", result[11]); 
            Assert.Equal("directive academy order", result[12]);
            Assert.Equal("yes", result[13]);
            Assert.Equal(model.Project.AdvisoryBoardDate.Value.ToString("dd/MM/yyyy"), result[14]);
            Assert.Equal("", result[15]);
            Assert.Equal("standard", result[16]);
            Assert.Equal("not applicable", result[17]);
            Assert.Equal("no", result[18]);
            Assert.Equal("unconfirmed", result[19]);
            Assert.Equal(model.CurrentSchool.TypeName, result[20]);
            Assert.Equal(model.CurrentSchool.AgeRangeLower + "-" + model.CurrentSchool.AgeRangeUpper, result[21]);
            Assert.Equal(model.CurrentSchool.TypeName, result[22]);
            Assert.Equal("not applicable", result[23]);
            Assert.Equal("not applicable", result[24]);
            Assert.Equal("not applicable", result[25]);
            Assert.Equal("", result[26]);
            Assert.Equal("", result[27]);
            Assert.Equal("", result[28]);
            Assert.Equal("", result[29]);
            Assert.Equal("", result[30]);
            Assert.Equal("", result[31]);
            Assert.Equal("", result[32]);
            Assert.Equal("form a MAT", result[33]);
            Assert.Equal(incomingTrust.Ukprn.ToString(), result[34]);
            Assert.Equal(incomingTrust.ReferenceNumber, result[35]);
            Assert.Equal(incomingTrust.CompaniesHouseNumber, result[36]);
            Assert.Equal(incomingTrust.Address.Street, result[37]);
            Assert.Equal(incomingTrust.Address.Locality, result[38]);
            Assert.Equal(incomingTrust.Address.AdditionalLine, result[39]);
            Assert.Equal(incomingTrust.Address.Town, result[40]);
            Assert.Equal(incomingTrust.Address.County, result[41]);
            Assert.Equal(incomingTrust.Address.Postcode, result[42]);
            Assert.Equal(model.Project.IncomingTrustSharepointLink, result[43]);
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
        [CustomAutoData(typeof(TrustDetailsDtoCustomization))]
        public void RowGeneratesBasedOnModel(TrustDetailsDto incomingTrust)
        {
            var model = ConversionCsvModelFactory.Make();

            model.Project.Type = ProjectType.Conversion;
            model.Project.IncomingTrustUkprn = incomingTrust.Ukprn;
            model.Project.SignificantDateProvisional = false;
            model.Project.TwoRequiresImprovement = false;
            model.Project.DirectiveAcademyOrder = false;
            model.Project.AllConditionsMet = true;

            model.ConversionTasks.RiskProtectionArrangementOption = RiskProtectionArrangementOption.Commercial;

            var TrustCache = Substitute.For<ITrustCache>();
            TrustCache.GetTrustAsync(incomingTrust.Ukprn).Returns(incomingTrust);

            var generator = new ConversionRowGenerator(new RowBuilderFactory<ConversionCsvModel>(TrustCache));

            generator.GenerateRow(model);

            var result = generator.GenerateRow(model).Split(",");

            Assert.Equal(model.CurrentSchool.Name, result[0]);
            Assert.Equal(model.Project.Urn.ToString(), result[1]);
            Assert.Equal("Conversion", result[2]);
            Assert.Equal(model.Academy.Name, result[3]);
            Assert.Equal(model.Academy.Urn.ToString(), result[4]);
            Assert.Equal(model.Academy.LocalAuthorityCode + "/" + model.Academy.EstablishmentNumber, result[5]);
            Assert.Equal(incomingTrust.Name, result[6]);
            Assert.Equal(model.LocalAuthority.Name, result[7]);
            Assert.Equal(model.CurrentSchool.RegionName, result[8]);
            Assert.Equal(model.CurrentSchool.DioceseName, result[9]);
            Assert.Equal(model.SignificantDateHistory.PreviousDate.Value.ToString("dd/MM/yyyy"), result[10]);
            Assert.Equal(model.Project.SignificantDate.Value.ToString("dd/MM/yyyy"), result[11]);
            Assert.Equal("academy order", result[12]);
            Assert.Equal("no", result[13]);
            Assert.Equal(model.Project.AdvisoryBoardDate.Value.ToString("dd/MM/yyyy"), result[14]);
            Assert.Equal(model.Project.AdvisoryBoardConditions, result[15]);
            Assert.Equal("commercial", result[16]);
            Assert.Equal(model.ConversionTasks.RiskProtectionArrangementReason, result[17]);
            Assert.Equal("yes", result[18]);
            Assert.Equal(model.ConversionTasks.ReceiveGrantPaymentCertificateDateReceived?.ToString("dd/MM/yyyy"), result[19]);
            Assert.Equal(model.CurrentSchool.TypeName, result[20]);
            Assert.Equal(model.CurrentSchool.AgeRangeLower + "-" + model.CurrentSchool.AgeRangeUpper, result[21]);
            Assert.Equal(model.CurrentSchool.PhaseName, result[22]);
            Assert.Equal(model.ConversionTasks.ProposedCapacityOfTheAcademyReceptionToSixYears, result[23]);
            Assert.Equal(model.ConversionTasks.ProposedCapacityOfTheAcademySevenToElevenYears, result[24]);
            Assert.Equal(model.ConversionTasks.ProposedCapacityOfTheAcademyTwelveOrAboveYears, result[25]);
            Assert.Equal(model.CurrentSchool.AddressStreet, result[26]);
            Assert.Equal(model.CurrentSchool.AddressLocality, result[27]);
            Assert.Equal(model.CurrentSchool.AddressAdditional, result[28]);
            Assert.Equal(model.CurrentSchool.AddressTown, result[29]);
            Assert.Equal(model.CurrentSchool.AddressCounty, result[30]);
            Assert.Equal(model.CurrentSchool.AddressPostcode, result[31]);
            Assert.Equal(model.Project.EstablishmentSharepointLink, result[32]);
            Assert.Equal("join a MAT", result[33]);
            Assert.Equal(incomingTrust.Ukprn.ToString(), result[34]);
            Assert.Equal(incomingTrust.ReferenceNumber.ToString(), result[35]);
            Assert.Equal(incomingTrust.CompaniesHouseNumber, result[36]);
            Assert.Equal(incomingTrust.Address.Street, result[37]);
            Assert.Equal(incomingTrust.Address.Locality, result[38]);
            Assert.Equal(incomingTrust.Address.AdditionalLine, result[39]);
            Assert.Equal(incomingTrust.Address.Town, result[40]);
            Assert.Equal(incomingTrust.Address.County, result[41]);
            Assert.Equal(incomingTrust.Address.Postcode, result[42]);
            Assert.Equal(model.Project.IncomingTrustSharepointLink, result[43]);
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
