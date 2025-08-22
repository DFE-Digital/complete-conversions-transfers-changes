using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Helpers
{
    public static class ExternalContactHelper
    {
        public static ContactCategory GetCategoryByContactType(ExternalContactType contactType)
        {
            return contactType switch
            {
                ExternalContactType.HeadTeacher or ExternalContactType.ChairOfGovernors => ContactCategory.SchoolOrAcademy,                
                ExternalContactType.IncomingTrustCEO => ContactCategory.IncomingTrust,
                ExternalContactType.OutgoingTrustCEO => ContactCategory.OutgoingTrust,
                ExternalContactType.LocalAuthority => ContactCategory.LocalAuthority,
                ExternalContactType.Solicitor => ContactCategory.Solicitor,
                ExternalContactType.Diocese => ContactCategory.Diocese,
                _ => ContactCategory.Other
            };
        }

        public static string GetRoleByContactType(ExternalContactType contactType)
        {
            if(contactType == ExternalContactType.IncomingTrustCEO || contactType == ExternalContactType.OutgoingTrustCEO)
            {
                return "CEO";
            }

            return contactType.ToDisplayDescription();
        }
    }
}
