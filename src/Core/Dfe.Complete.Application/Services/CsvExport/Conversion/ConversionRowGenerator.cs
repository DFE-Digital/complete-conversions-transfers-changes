using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Mappers;
using Dfe.Complete.Application.Services.CsvExport.Builders;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Application.Services.CsvExport.Conversion
{
    public class ConversionRowGenerator : IRowGenerator<ConversionCsvModel>, IHeaderGenerator<ConversionCsvModel>
    {
        private const string Unconfirmed = "unconfirmed";
        private const string NotApplicable = "not applicable";
        private const string Yes = "yes";
        private const string No = "no";
        private const string DateFormat = "yyyy-MM-dd";
        private readonly RowBuilder<ConversionCsvModel> _rowBuilder;

        public ConversionRowGenerator(IRowBuilderFactory<ConversionCsvModel> rowBuilder)
        {
            _rowBuilder = rowBuilder.DefineRow()
                    .Column("School name").BlankIfEmpty(x => x.CurrentSchool.Name)
                    .Column("School URN").BlankIfEmpty(x => x.Project.Urn.Value.ToString())
                    .Column("Project type").Builder(new ProjectTypeBuilder())
                    .Column("Academy name").DefaultIf(x => x.Project.AcademyUrn == null, x => x.Academy?.Name, Unconfirmed)
                    .Column("Academy URN").DefaultIf(x => x.Project.AcademyUrn == null, x => x.Academy?.Urn?.Value.ToString(), Unconfirmed)
                    .Column("Academy DfE number/LAESTAB").Builder(new DfeNumberLAESTABBuilder())
                    .Column("Incoming trust name").IncomingTrustData(x => x.Project, x => x.Name)
                    .Column("Local authority").BlankIfEmpty(x => x.LocalAuthority.Name)
                    .Column("Region").BlankIfEmpty(x => x.CurrentSchool.RegionName)
                    .Column("Diocese").BlankIfEmpty(x => x.CurrentSchool.DioceseName)
                    .Column("Provisional conversion date").Builder(new ProvisionalDateBuilder())
                    .Column("Confirmed conversion date").DefaultIf(x => x.Project.SignificantDateProvisional == true, x => x.Project.SignificantDate?.ToString(DateFormat) ?? Unconfirmed, Unconfirmed)
                    .Column("Academy order type").Builder(new AcademyOrderTypeBuilder<ConversionCsvModel>(x => x.Project))
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
                    .Column("Proposed capacity for pupils in reception to year 6").BlankIfEmpty(x => x.ConversionTasks.ProposedCapacityOfTheAcademyReceptionToSixYears)
                    .Column("Proposed capacity for pupils in years 7 to 11").BlankIfEmpty(x => x.ConversionTasks.ProposedCapacityOfTheAcademySevenToElevenYears)
                    .Column("Proposed capacity for students in year 12 or above").BlankIfEmpty(x => x.ConversionTasks.ProposedCapacityOfTheAcademyTwelveOrAboveYears)
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
                    .Column("Incoming trust address 3").IncomingTrustData(x => x.Project, t => t.Address.Additional)
                    .Column("Incoming trust address town").IncomingTrustData(x => x.Project, t => t.Address.Town)
                    .Column("Incoming trust address county").IncomingTrustData(x => x.Project, t => t.Address.County)
                    .Column("Incoming trust address postcode").IncomingTrustData(x => x.Project, t => t.Address.Postcode)
                    .Column("Incoming trust sharepoint link").BlankIfEmpty(x => x.Project.IncomingTrustSharepointLink)
                    .Column("Project created by name").Builder(new UserNameBuilder<ConversionCsvModel>(x => x.CreatedBy!))
                    .Column("Project created by email address").BlankIfEmpty(x => x.CreatedBy?.Email)
                    .Column("Assigned to name").Builder(new UserNameBuilder<ConversionCsvModel>(x => x.AssignedTo!))
                    .Column("Team managing the project").BlankIfEmpty(x => ProjectTeamPresentationMapper.Map(x.Project.Team))
                    .Column("Project main contact name").BlankIfEmpty(x => x.MainContact?.Name)
                    .Column("Headteacher name").BlankIfEmpty(x => x.Headteacher?.Name)
                    .Column("Headteacher role").BlankIfEmpty(x => x.Headteacher != null ? "Headteacher" : null)
                    .Column("Headteacher email").BlankIfEmpty(x => x.Headteacher?.Email)
                    .Column("Local authority contact name").BlankIfEmpty(x => x.LocalAuthorityContact?.Name)
                    .Column("Local authority contact email").BlankIfEmpty(x => x.LocalAuthorityContact?.Email)
                    .Column("Primary contact for incoming trust name").BlankIfEmpty(x => x.IncomingContact?.Name)
                    .Column("Primary contact for incoming trust email").BlankIfEmpty(x => x.IncomingContact?.Email)
                    .Column("Primary contact for outgoing trust name").BlankIfEmpty(x => x.OutgoingContact?.Name)
                    .Column("Primary contact for outgoing trust email").BlankIfEmpty(x => x.OutgoingContact?.Email)
                    .Column("Incoming trust CEO name").BlankIfEmpty(x => x.IncomingCEOContact?.Name)
                    .Column("Incoming trust CEO role").BlankIfEmpty(x => x.IncomingCEOContact != null ? "CEO" : null)
                    .Column("Incoming trust CEO email").BlankIfEmpty(x => x.IncomingCEOContact?.Email)
                    .Column("Solicitor contact name").BlankIfEmpty(x => x.SolicitorContact?.Name)
                    .Column("Solicitor contact email").BlankIfEmpty(x => x.SolicitorContact?.Email)
                    .Column("Diocese contact name").BlankIfEmpty(x => x.DioceseContact?.Name)
                    .Column("Diocese contact email").BlankIfEmpty(x => x.DioceseContact?.Email)
                    .Column("Director of child services name").BlankIfEmpty(x => x.DirectorOfServicesContact?.Name)
                    .Column("Director of child services email").BlankIfEmpty(x => x.DirectorOfServicesContact?.Email)
                    .Column("Director of child services role").BlankIfEmpty(x => x.DirectorOfServicesContact?.Title);
        }

        public string GenerateHeader()
        {
            return _rowBuilder.BuildHeaders();
        }

        public string GenerateRow(ConversionCsvModel model)
        {
            return _rowBuilder.Build(model);
        }
    }
}
