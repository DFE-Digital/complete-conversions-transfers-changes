using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Services.CsvExport.Builders;
using System.Text;

namespace Dfe.Complete.Application.Services.CsvExport.Conversion
{
    public class ConversionRowGenerator : IRowGenerator<ConversionCsvModel>
    {
        private const string Unconfirmed = "unconfirmed";

        private IColumnBuilder<ConversionCsvModel>[] _columnBuilders =
            [
                new BlankIfEmpty<ConversionCsvModel>(x => x.CurrentSchool.Name),
                new BlankIfEmpty<ConversionCsvModel>(x => x.Project.Urn),
                new ProjectTypeBuilder(),
                new DefaultIf<ConversionCsvModel>(x => x.Project.AcademyUrn == null, x => x.Academy.Name, Unconfirmed),
                new DefaultIf<ConversionCsvModel>(x => x.Project.AcademyUrn == null, x => x.Academy.Urn, Unconfirmed),
                new DfeNumberLAESTABBuilder(),
            ];



        public string GenerateRow(ConversionCsvModel model)
        {
            var row = new StringBuilder();

            _columnBuilders.Select(x => x.Build(model)).Aggregate(row, (acc, x) => acc.Append(x).Append(","));

            //row.Append(model.SchoolName);
            //row.Append(",");
            //row.Append(model.SchoolUrn);
            //row.Append(",");
            //row.Append(model.FormAMat ? "form a MAT": "join a MAT");
            //row.Append(",");
            //row.Append(BlankIfNull(model.AcademyName));
            //row.Append(",");
            //row.Append(BlankIfNull(model.AcademyUrn));
            //row.Append(",");
            //row.Append(model.AcademyDfENumber);
            //row.Append(",");
            //row.Append(model.IncomingTrustName);
            //row.Append(",");
            //row.Append(model.LocalAuthority);
            //row.Append(",");
            //row.Append(model.Region);
            //row.Append(",");
            //row.Append(model.Diocese);
            //row.Append(",");
            //row.Append(model.ProvisionalConversionDate);
            //row.Append(",");
            //row.Append(model.ConfirmedConversionDate);
            //row.Append(",");
            //row.Append(model.AcademyOrderType);
            //row.Append(",");
            //row.Append(model.TwoRequiresImprovement);
            //row.Append(",");
            //row.Append(model.AdvisoryBoardDate);
            //row.Append(",");
            //row.Append(model.AdvisoryBoardConditions);
            //row.Append(",");
            //row.Append(model.RiskProtectionArrangement);
            //row.Append(",");
            //row.Append(model.ReasonForCommercialInsurance);
            //row.Append(",");
            //row.Append(model.AllConditionsMet);
            //row.Append(",");
            //row.Append(model.CompletedGrantPaymentCertificateReceived);
            //row.Append(",");
            //row.Append(model.SchoolType);
            //row.Append(",");
            //row.Append(model.SchoolAgeRange);
            //row.Append(",");
            //row.Append(model.SchoolPhase);
            //row.Append(",");
            //row.Append(model.ProposedCapacityForPupilsInReceptionToYear6);
            //row.Append(",");
            //row.Append(model.ProposedCapacityForPupilsInYears7To11);
            //row.Append(",");
            //row.Append(model.ProposedCapacityForStudentsInYear12OrAbove);
            //row.Append(",");
            //row.Append(model.SchoolAddress1);
            //row.Append(",");
            //row.Append(model.SchoolAddress2);
            //row.Append(",");
            //row.Append(model.SchoolAddress3);
            //row.Append(",");
            //row.Append(model.SchoolTown);
            //row.Append(",");
            //row.Append(model.SchoolCounty);
            //row.Append(",");
            //row.Append(model.SchoolPostcode);
            //row.Append(",");
            //row.Append(model.SchoolSharepointFolder);
            //row.Append(",");
            //row.Append(model.ConversionType);
            //row.Append(",");
            //row.Append(model.IncomingTrustUKPRN);
            //row.Append(",");
            //row.Append(model.IncomingTrustGroupIdentifier);
            //row.Append(",");
            //row.Append(model.IncomingTrustCompaniesHouseNumber);
            //row.Append(",");
            //row.Append(model.IncomingTrustAddress1);
            //row.Append(",");
            //row.Append(model.IncomingTrustAddress2);
            //row.Append(",");
            //row.Append(model.IncomingTrustAddress3);
            //row.Append(",");
            //row.Append(model.IncomingTrustAddressTown);
            //row.Append(",");
            //row.Append(model.IncomingTrustAddressCounty);
            //row.Append(",");
            //row.Append(model.IncomingTrustAddressPostcode);
            //row.Append(",");
            //row.Append(model.IncomingTrustSharepointLink);
            //row.Append(",");
            //row.Append(model.ProjectCreatedBy);
            //row.Append(",");
            //row.Append(model.ProjectCreatedByEmailAddress);
            //row.Append(",");
            //row.Append(model.AssignedToName);
            //row.Append(",");
            //row.Append(model.TeamManagingTheProject);
            //row.Append(",");
            //row.Append(model.ProjectMainContactName);
            //row.Append(",");
            //row.Append(model.HeadteacherName);
            //row.Append(",");
            //row.Append(model.HeadteacherRole);
            //row.Append(",");
            //row.Append(model.HeadteacherEmail);
            //row.Append(",");
            //row.Append(model.LocalAuthorityContactName);
            //row.Append(",");
            //row.Append(model.LocalAuthorityContactEmail);
            //row.Append(",");
            //row.Append(model.PrimaryContactForIncomingTrustName);
            //row.Append(",");
            //row.Append(model.PrimaryContactForIncomingTrustEmail);
            //row.Append(",");
            //row.Append(model.PrimaryContactForOutgoingTrustName);
            //row.Append(",");
            //row.Append(model.PrimaryContactForOutgoingTrustEmail);
            //row.Append(",");
            //row.Append(model.IncomingTrustCEOName);
            //row.Append(",");
            //row.Append(model.IncomingTrustCEORole);
            //row.Append(",");
            //row.Append(model.IncomingTrustCEOEmail);
            //row.Append(",");
            //row.Append(model.SolicitorContactName);
            //row.Append(",");
            //row.Append(model.SolicitorContactEmail);
            //row.Append(",");
            //row.Append(model.DioceseContactName);
            //row.Append(",");
            //row.Append(model.DioceseContactEmail);
            //row.Append(",");
            //row.Append(model.DirectorOfChildServicesName);
            //row.Append(",");
            //row.Append(model.DirectorOfChildServicesEmail);
            //row.Append(",");
            //row.Append(model.DirectorOfChildServicesRole);

            return row.ToString();
        }

        private string BlankIfNull(string? value)
        {
            return value ?? string.Empty;
        }

        private string BlankIfNull(int? value)
        {
            return value == null ? string.Empty: value.Value.ToString();
        }
    }
}
