using Dfe.Complete.Application.Common.Models;

namespace Dfe.Complete.Application.Services.CsvExport.Conversion
{
    public class ConversionHeaderGenerator : IHeaderGenerator<ConversionCsvModel>
    {
        public string GenerateHeader()
        {
            return "School name,School URN,Project type,Academy name,Academy URN,Academy DfE number/LAESTAB,Incoming trust name,Local authority,Region,Diocese,Provisional conversion date,Confirmed conversion date,Academy order type,2RI (Two Requires Improvement),Advisory board date,Advisory board conditions,Risk protection arrangement,Reason for commercial insurance,All conditions met,Completed grant payment certificate received,School type,School age range,School phase,Proposed capacity for pupils in reception to year 6,Proposed capacity for pupils in years 7 to 11,Proposed capacity for students in year 12 or above,School address 1,School address 2,School address 3,School town,School county,School postcode,School sharepoint folder,Conversion type,Incoming trust UKPRN,Incoming trust group identifier,Incoming trust companies house number,Incoming trust address 1,Incoming trust address 2,Incoming trust address 3,Incoming trust address town,Incoming trust address county,Incoming trust address postcode,Incoming trust sharepoint link,Project created by name,Project created by email address,Assigned to name,Team managing the project,Project main contact name,Headteacher name,Headteacher role,Headteacher email,Local authority contact name,Local authority contact email,Primary contact for incoming trust name,Primary contact for incoming trust email,Primary contact for outgoing trust name,Primary contact for outgoing trust email,Incoming trust CEO name,Incoming trust CEO role,Incoming trust CEO email,Solicitor contact name,Solicitor contact email,Diocese contact name,Diocese contact email,Director of child services name,Director of child services email,Director of child services role";
        }
    }
}
