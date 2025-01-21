using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Services.CsvExport.Conversion;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
namespace Dfe.Complete.Application.Tests.Services.CsvExport.Conversion
{
    public class ConversionRowGeneratorTests
    {


        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(EstablishmentsCustomization), typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public void RowGeneratesAccountsForBlankData(Project project, GiasEstablishment establishment)
        { 
            project.Type = ProjectType.Conversion;
            var model = new ConversionCsvModel(project, establishment);
            
            //var model = new ConversionCsvModel()
            //{
            //    AllConditionsMet = true,
            //    AcademyDfENumber = null,
            //    AcademyName = null,
            //    AcademyOrderType = null,
            //    AcademyUrn = null,
            //    AdvisoryBoardConditions = null,
            //    AdvisoryBoardDate = new DateOnly(2024, 3, 2),
            //    CompletedGrantPaymentCertificateReceived = true,
            //    ConfirmedConversionDate = new DateOnly(2024, 4, 3),
            //    ConversionType = "ConversionType",
            //    Diocese = "Diocese",
            //    HeadteacherName = "HeadteacherName",
            //    IncomingTrustAddress1 = "IncomingTrustAddress1",
            //    IncomingTrustAddress2 = "IncomingTrustAddress2",
            //    IncomingTrustAddress3 = "IncomingTrustAddress3",
            //    IncomingTrustAddressCounty = "IncomingTrustAddressCounty",
            //    IncomingTrustAddressPostcode = "IncomingTrustAddressPostcode",
            //    IncomingTrustAddressTown = "IncomingTrustAddressTown",
            //    IncomingTrustCompaniesHouseNumber = "IncomingTrustCompaniesHouseNumber",
            //    IncomingTrustGroupIdentifier = "IncomingTrustGroupIdentifier",
            //    IncomingTrustName = "IncomingTrustName",
            //    IncomingTrustSharepointLink = "IncomingTrustSharepointLink",
            //    IncomingTrustUKPRN = 12345647,
            //    AssignedToName = "AssignedToName",
            //    DioceseContactEmail = "DioceseContactEmail",
            //    DioceseContactName = "DioceseContactName",
            //    DirectorOfChildServicesEmail = "DirectorOfChildServicesEmail",
            //    DirectorOfChildServicesName = "DirectorOfChildServicesName",
            //    DirectorOfChildServicesRole = "DirectorOfChildServicesRole",
            //    HeadteacherEmail = "HeadteacherEmail",
            //    HeadteacherRole = "HeadteacherRole",
            //    IncomingTrustCEOEmail = "IncomingTrustCEOEmail",
            //    IncomingTrustCEOName = "IncomingTrustCEOName",
            //    IncomingTrustCEORole = "IncomingTrustCEORole",
            //    LocalAuthority = "LocalAuthority",
            //    LocalAuthorityContactEmail = "LocalAuthorityContactEmail",
            //    LocalAuthorityContactName = "LocalAuthorityContactName",
            //    PrimaryContactForIncomingTrustEmail = "PrimaryContactForIncomingTrustEmail",
            //    PrimaryContactForIncomingTrustName = "PrimaryContactForIncomingTrustName",
            //    PrimaryContactForOutgoingTrustEmail = "PrimaryContactForOutgoingTrustEmail",
            //    PrimaryContactForOutgoingTrustName = "PrimaryContactForOutgoingTrustName",
            //    ProjectCreatedBy = "ProjectCreatedBy",
            //    ProjectCreatedByEmailAddress = "ProjectCreatedByEmailAddress",
            //    ProjectMainContactName = "ProjectMainContactName",
            //    FormAMat = false,
            //    ProposedCapacityForPupilsInReceptionToYear6 = 123,
            //    ProposedCapacityForPupilsInYears7To11 = 456,
            //    ProposedCapacityForStudentsInYear12OrAbove = 789,
            //    ProvisionalConversionDate = new DateOnly(2024, 5, 6),
            //    ReasonForCommercialInsurance = "ReasonForCommercialInsurance",
            //    Region = "Region",
            //    RiskProtectionArrangement = "RiskProtectionArrangement",
            //    SchoolAddress1 = "SchoolAddress1",
            //    SchoolAddress2 = "SchoolAddress2",
            //    SchoolAddress3 = "SchoolAddress3",
            //    SchoolAgeRange = "SchoolAgeRange",
            //    SchoolCounty = "SchoolCounty",
            //    SchoolName = "SchoolName",
            //    SchoolPhase = "SchoolPhase",
            //    SchoolPostcode = "SchoolPostcode",
            //    SchoolSharepointFolder = "SchoolSharepointFolder",
            //    SchoolTown = "SchoolTown",
            //    SchoolType = "SchoolType",
            //    SolicitorContactEmail = "SolicitorContactEmail",
            //    SchoolUrn = 12345,
            //    SolicitorContactName = "SolicitorContactName",
            //    TeamManagingTheProject = "TeamManagingTheProject",
            //    TwoRequiresImprovement = true
            //};

            var generator = new ConversionRowGenerator();

            generator.GenerateRow(model);

            var result = generator.GenerateRow(model).Split(",");

            Assert.Equal(establishment.Name, result[0]);
            Assert.Equal(project.Urn.ToString(), result[1]);
            Assert.Equal("join a MAT", result[2]);
            //Assert.Equal("AcademyName", result[3]);
            //Assert.Equal("54321", result[4]);
            //Assert.Equal("AcademyDfENumber", result[5]);
            //Assert.Equal("IncomingTrustName", result[6]);
            //Assert.Equal("LocalAuthority", result[7]);
            //Assert.Equal("Region", result[8]);
            //Assert.Equal("Diocese", result[9]);
            //Assert.Equal("06/05/2024", result[10]);
            //Assert.Equal("03/04/2024", result[11]);
            //Assert.Equal("AcademyOrderType", result[12]);
            //Assert.Equal("True", result[13]);
            //Assert.Equal("02/03/2024", result[14]);
            //Assert.Equal("AdvisoryBoardConditions", result[15]);
            //Assert.Equal("RiskProtectionArrangement", result[16]);
            //Assert.Equal("ReasonForCommercialInsurance", result[17]);
            //Assert.Equal("True", result[18]);
            //Assert.Equal("True", result[19]);
            //Assert.Equal("SchoolType", result[20]);
            //Assert.Equal("SchoolAgeRange", result[21]);
            //Assert.Equal("SchoolPhase", result[22]);
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

        //[Fact]
        //public void RowGeneratesBasedOnModel()
        //{
        //    //var model = new ConversionCsvModel()
        //    //{
        //    //    AllConditionsMet = true,
        //    //    AcademyDfENumber = "AcademyDfENumber",
        //    //    AcademyName = "AcademyName",
        //    //    AcademyOrderType = "AcademyOrderType",
        //    //    AcademyUrn = 54321,
        //    //    AdvisoryBoardConditions = "AdvisoryBoardConditions",
        //    //    AdvisoryBoardDate = new DateOnly(2024, 3, 2),
        //    //    CompletedGrantPaymentCertificateReceived = true,
        //    //    ConfirmedConversionDate = new DateOnly(2024, 4, 3),
        //    //    ConversionType = "ConversionType",
        //    //    Diocese = "Diocese",
        //    //    HeadteacherName = "HeadteacherName",
        //    //    IncomingTrustAddress1 = "IncomingTrustAddress1",
        //    //    IncomingTrustAddress2 = "IncomingTrustAddress2",
        //    //    IncomingTrustAddress3 = "IncomingTrustAddress3",
        //    //    IncomingTrustAddressCounty = "IncomingTrustAddressCounty",
        //    //    IncomingTrustAddressPostcode = "IncomingTrustAddressPostcode",
        //    //    IncomingTrustAddressTown = "IncomingTrustAddressTown",
        //    //    IncomingTrustCompaniesHouseNumber = "IncomingTrustCompaniesHouseNumber",
        //    //    IncomingTrustGroupIdentifier = "IncomingTrustGroupIdentifier",
        //    //    IncomingTrustName = "IncomingTrustName",
        //    //    IncomingTrustSharepointLink = "IncomingTrustSharepointLink",
        //    //    IncomingTrustUKPRN = 12345647,
        //    //    AssignedToName = "AssignedToName",
        //    //    DioceseContactEmail = "DioceseContactEmail",
        //    //    DioceseContactName = "DioceseContactName",
        //    //    DirectorOfChildServicesEmail = "DirectorOfChildServicesEmail",
        //    //    DirectorOfChildServicesName = "DirectorOfChildServicesName",
        //    //    DirectorOfChildServicesRole = "DirectorOfChildServicesRole",
        //    //    HeadteacherEmail = "HeadteacherEmail",
        //    //    HeadteacherRole = "HeadteacherRole",
        //    //    IncomingTrustCEOEmail = "IncomingTrustCEOEmail",
        //    //    IncomingTrustCEOName = "IncomingTrustCEOName",
        //    //    IncomingTrustCEORole = "IncomingTrustCEORole",
        //    //    LocalAuthority = "LocalAuthority",
        //    //    LocalAuthorityContactEmail = "LocalAuthorityContactEmail",
        //    //    LocalAuthorityContactName = "LocalAuthorityContactName",
        //    //    PrimaryContactForIncomingTrustEmail = "PrimaryContactForIncomingTrustEmail",
        //    //    PrimaryContactForIncomingTrustName = "PrimaryContactForIncomingTrustName",
        //    //    PrimaryContactForOutgoingTrustEmail = "PrimaryContactForOutgoingTrustEmail",
        //    //    PrimaryContactForOutgoingTrustName = "PrimaryContactForOutgoingTrustName",
        //    //    ProjectCreatedBy = "ProjectCreatedBy",
        //    //    ProjectCreatedByEmailAddress = "ProjectCreatedByEmailAddress",
        //    //    ProjectMainContactName = "ProjectMainContactName",
        //    //    FormAMat = true,
        //    //    ProposedCapacityForPupilsInReceptionToYear6 = 123,
        //    //    ProposedCapacityForPupilsInYears7To11 = 456,
        //    //    ProposedCapacityForStudentsInYear12OrAbove = 789,
        //    //    ProvisionalConversionDate = new DateOnly(2024, 5, 6),
        //    //    ReasonForCommercialInsurance = "ReasonForCommercialInsurance",
        //    //    Region = "Region",
        //    //    RiskProtectionArrangement = "RiskProtectionArrangement",
        //    //    SchoolAddress1 = "SchoolAddress1",
        //    //    SchoolAddress2 = "SchoolAddress2",
        //    //    SchoolAddress3 = "SchoolAddress3",
        //    //    SchoolAgeRange = "SchoolAgeRange",
        //    //    SchoolCounty = "SchoolCounty",
        //    //    SchoolName = "SchoolName",
        //    //    SchoolPhase = "SchoolPhase",
        //    //    SchoolPostcode = "SchoolPostcode",
        //    //    SchoolSharepointFolder = "SchoolSharepointFolder",
        //    //    SchoolTown = "SchoolTown",
        //    //    SchoolType = "SchoolType",
        //    //    SolicitorContactEmail = "SolicitorContactEmail",
        //    //    SchoolUrn = 12345,
        //    //    SolicitorContactName = "SolicitorContactName",
        //    //    TeamManagingTheProject = "TeamManagingTheProject",
        //    //    TwoRequiresImprovement = true
        //    //};


        //    var model = new ConversionCsvModel(null, null);

        //    var generator = new ConversionRowGenerator();

        //    generator.GenerateRow(model);
                            
        //    var result = generator.GenerateRow(model).Split(",");

        //    Assert.Equal("SchoolName", result[0]);
        //    Assert.Equal("12345", result[1]);
        //    Assert.Equal("ProjectType", result[2]);
        //    Assert.Equal("AcademyName", result[3]);
        //    Assert.Equal("54321", result[4]);
        //    Assert.Equal("AcademyDfENumber", result[5]);
        //    Assert.Equal("IncomingTrustName", result[6]);
        //    Assert.Equal("LocalAuthority", result[7]);
        //    Assert.Equal("Region", result[8]);
        //    Assert.Equal("Diocese", result[9]);
        //    Assert.Equal("06/05/2024", result[10]);
        //    Assert.Equal("03/04/2024", result[11]);
        //    Assert.Equal("AcademyOrderType", result[12]);
        //    Assert.Equal("True", result[13]);
        //    Assert.Equal("02/03/2024", result[14]);
        //    Assert.Equal("AdvisoryBoardConditions", result[15]);
        //    Assert.Equal("RiskProtectionArrangement", result[16]);
        //    Assert.Equal("ReasonForCommercialInsurance", result[17]);
        //    Assert.Equal("True", result[18]);
        //    Assert.Equal("True", result[19]);
        //    Assert.Equal("SchoolType", result[20]);
        //    Assert.Equal("SchoolAgeRange", result[21]);
        //    Assert.Equal("SchoolPhase", result[22]);
        //    Assert.Equal("123", result[23]);
        //    Assert.Equal("456", result[24]);
        //    Assert.Equal("789", result[25]);
        //    Assert.Equal("SchoolAddress1", result[26]);
        //    Assert.Equal("SchoolAddress2", result[27]);
        //    Assert.Equal("SchoolAddress3", result[28]);
        //    Assert.Equal("SchoolTown", result[29]);
        //    Assert.Equal("SchoolCounty", result[30]);
        //    Assert.Equal("SchoolPostcode", result[31]);
        //    Assert.Equal("SchoolSharepointFolder", result[32]);
        //    Assert.Equal("ConversionType", result[33]);
        //    Assert.Equal("12345647", result[34]);
        //    Assert.Equal("IncomingTrustGroupIdentifier", result[35]);
        //    Assert.Equal("IncomingTrustCompaniesHouseNumber", result[36]);
        //    Assert.Equal("IncomingTrustAddress1", result[37]);
        //    Assert.Equal("IncomingTrustAddress2", result[38]);
        //    Assert.Equal("IncomingTrustAddress3", result[39]);
        //    Assert.Equal("IncomingTrustAddressTown", result[40]);
        //    Assert.Equal("IncomingTrustAddressCounty", result[41]);
        //    Assert.Equal("IncomingTrustAddressPostcode", result[42]);
        //    Assert.Equal("IncomingTrustSharepointLink", result[43]);
        //    Assert.Equal("ProjectCreatedBy", result[44]);
        //    Assert.Equal("ProjectCreatedByEmailAddress", result[45]);
        //    Assert.Equal("AssignedToName", result[46]);
        //    Assert.Equal("TeamManagingTheProject", result[47]);
        //    Assert.Equal("ProjectMainContactName", result[48]);
        //    Assert.Equal("HeadteacherName", result[49]);
        //    Assert.Equal("HeadteacherRole", result[50]);
        //    Assert.Equal("HeadteacherEmail", result[51]);
        //    Assert.Equal("LocalAuthorityContactName", result[52]);
        //    Assert.Equal("LocalAuthorityContactEmail", result[53]);
        //    Assert.Equal("PrimaryContactForIncomingTrustName", result[54]);
        //    Assert.Equal("PrimaryContactForIncomingTrustEmail", result[55]);
        //    Assert.Equal("PrimaryContactForOutgoingTrustName", result[56]);
        //    Assert.Equal("PrimaryContactForOutgoingTrustEmail", result[57]);
        //    Assert.Equal("IncomingTrustCEOName", result[58]);
        //    Assert.Equal("IncomingTrustCEORole", result[59]);
        //    Assert.Equal("IncomingTrustCEOEmail", result[60]);
        //    Assert.Equal("SolicitorContactName", result[61]);
        //    Assert.Equal("SolicitorContactEmail", result[62]);
        //    Assert.Equal("DioceseContactName", result[63]);
        //    Assert.Equal("DioceseContactEmail", result[64]);
        //    Assert.Equal("DirectorOfChildServicesName", result[65]);
        //    Assert.Equal("DirectorOfChildServicesEmail", result[66]);
        //    Assert.Equal("DirectorOfChildServicesRole", result[67]);

            


            
        //}
    }
}
