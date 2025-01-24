using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Services.CsvExport.Builders;
using Dfe.Complete.Domain.Enums;
using System.Net;

namespace Dfe.Complete.Application.Services.CsvExport.Conversion
{
    public class ConversionRowGenerator(IRowBuilderFactory<ConversionCsvModel> rowBuilder) : IRowGenerator<ConversionCsvModel>
    {
        private const string Unconfirmed = "unconfirmed";
        private const string NotApplicable = "not applicable";
        private const string Yes = "yes";
        private const string No = "no";
        private const string DateFormat = "dd/MM/yyyy";

        public string GenerateRow(ConversionCsvModel model)
        {
            return rowBuilder.DefineRow()
                    .Column("School name").BlankIfEmpty(x => x.CurrentSchool.Name)
                    .Column("School URN").BlankIfEmpty(x => x.Project.Urn)
                    .Column("Project type").Builder(new ProjectTypeBuilder())
                    .Column("Academy name").DefaultIf(x => x.Project.AcademyUrn == null, x => x.Academy?.Name, Unconfirmed)
                    .Column("Academy URN").DefaultIf(x => x.Project.AcademyUrn == null, x => x.Academy?.Urn?.ToString(), Unconfirmed)
                    .Column("Academy DfE number/LAESTAB").Builder(new DfeNumberLAESTABBuilder())
                    .Column("Incoming trust name").IncomingTrustData(x => x.Project, x => x.Name)
                    .Column("Local authority").BlankIfEmpty(x => x.LocalAuthority.Name)
                    .Column("Region").BlankIfEmpty(x => x.CurrentSchool.RegionName)
                    .Column("Diocese").BlankIfEmpty(x => x.CurrentSchool.DioceseName)
                    .Column("Provisional conversion date").Builder(new ProvisionalDateBuilder())
                    .Column("Confirmed conversion date").DefaultIf(x => x.Project.SignificantDateProvisional == true, x => x.Project.SignificantDate?.ToString(DateFormat) ?? Unconfirmed, Unconfirmed)
                    .Column("Academy order type").Builder(new AcademyOrderTypeBuilder<ConversionCsvModel>(x=> x.Project))
                    .Column("2RI (Two Requires Improvement)").Bool(x => x.Project.TwoRequiresImprovement, Yes, No)
                    .Column("Advisory board date").BlankIfEmpty(x => x.Project.AdvisoryBoardDate?.ToString(DateFormat))
                    .Column("Advisory board conditions").BlankIfEmpty(x => x.Project.AdvisoryBoardConditions)
                    .Column("Risk protection arrangement").Builder(new RPAOptionBuilder<ConversionCsvModel>(x => x.ConversionTasks.RiskProtectionArrangementOption))
                    .Column("Reason for commercial insurance").DefaultIf(x => x.ConversionTasks.RiskProtectionArrangementOption != RiskProtectionArrangementOption.Commercial, x => x.ConversionTasks.RiskProtectionArrangementReason, NotApplicable)
                    .Column("All conditions met").Bool(x => x.Project.AllConditionsMet, Yes, No)
                    .Column("Completed grant payment certificate received").DefaultIfEmpty(x => x.ConversionTasks.ReceiveGrantPaymentCertificateDateReceived?.ToString(DateFormat), Unconfirmed)
                    .Column("School type").BlankIfEmpty(x => x.CurrentSchool.TypeName)
                    .Column("School age range").Builder(new AgeRangeBuilder<ConversionCsvModel>(x => x.CurrentSchool))
                    .Column("School phase").Builder(new SchoolPhaseBuilder<ConversionCsvModel>(x => x.CurrentSchool))
                    .Column("Proposed capacity for pupils in reception to year 6").DefaultIfEmpty(x => x.ConversionTasks.ProposedCapacityOfTheAcademyReceptionToSixYears, NotApplicable)
                    .Column("Proposed capacity for pupils in years 7 to 11").DefaultIfEmpty(x => x.ConversionTasks.ProposedCapacityOfTheAcademySevenToElevenYears, NotApplicable)
                    .Column("Proposed capacity for students in year 12 or above").DefaultIfEmpty(x => x.ConversionTasks.ProposedCapacityOfTheAcademyTwelveOrAboveYears, NotApplicable)
                    .Column("School address 1").BlankIfEmpty(x => x.CurrentSchool.AddressStreet)
                    .Column("School address 2").BlankIfEmpty(x => x.CurrentSchool.AddressLocality)
                    .Column("School address 3").BlankIfEmpty(x => x.CurrentSchool.AddressAdditional)
                    .Column("School town").BlankIfEmpty(x => x.CurrentSchool.AddressTown)
                    .Column("School county").BlankIfEmpty(x => x.CurrentSchool.AddressCounty)
                    .Column("School postcode").BlankIfEmpty(x => x.CurrentSchool.AddressPostcode)
                    .Column("School sharepoint folder").BlankIfEmpty(x => x.Project.EstablishmentSharepointLink)
                    .Column("Conversion type").Builder(new FormAMat<ConversionCsvModel>(x => x.Project))
                    .Column("Incoming trust UKPRN").IncomingTrustData(x => x.Project, t => t.Ukprn.ToString())
                    .Column("Incoming trust group identifier").IncomingTrustData(x => x.Project, t => t.ReferenceNumber)
                    .Column("Incoming trust companies house number").IncomingTrustData(x => x.Project, t => t.CompaniesHouseNumber)
                    .Column("Incoming trust address 1").IncomingTrustData(x => x.Project, t => t.Address.Street)
                    .Column("Incoming trust address 2").IncomingTrustData(x => x.Project, t => t.Address.Locality)
                    .Column("Incoming trust address 3").IncomingTrustData(x => x.Project, t => t.Address.AdditionalLine)
                    .Column("Incoming trust address town").IncomingTrustData(x => x.Project, t => t.Address.Town)
                    .Column("Incoming trust address county").IncomingTrustData(x => x.Project, t => t.Address.County)
                    .Column("Incoming trust address postcode").IncomingTrustData(x => x.Project, t => t.Address.Postcode)
                    .Column("Incoming trust sharepoint link").BlankIfEmpty(x => x.Project.IncomingTrustSharepointLink)
                    .Build(model);
        }
    }
}
