using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Helpers
{
    public static class ExternalContactMapper
    {
        public static ContactCategory MapContactTypeToCategory(ExternalContactType contactType)
        {
            return contactType switch
            {
                ExternalContactType.HeadTeacher or ExternalContactType.ChairOfGovernors or ExternalContactType.SchoolOrAcademy => ContactCategory.SchoolOrAcademy,
                ExternalContactType.IncomingTrust => ContactCategory.IncomingTrust,
                ExternalContactType.OutgoingTrust => ContactCategory.OutgoingTrust,
                ExternalContactType.LocalAuthority => ContactCategory.LocalAuthority,
                ExternalContactType.Solicitor => ContactCategory.Solicitor,
                ExternalContactType.Diocese => ContactCategory.Diocese,
                _ => ContactCategory.Other
            };
        }
        public static ExternalContactType MapCategoryToContactType(ContactCategory contactCategory)
        {
            return contactCategory switch
            {
                ContactCategory.SchoolOrAcademy => ExternalContactType.SchoolOrAcademy,
                ContactCategory.IncomingTrust => ExternalContactType.IncomingTrust,
                ContactCategory.OutgoingTrust => ExternalContactType.OutgoingTrust,
                ContactCategory.LocalAuthority => ExternalContactType.LocalAuthority,
                ContactCategory.Solicitor => ExternalContactType.Solicitor,
                ContactCategory.Diocese => ExternalContactType.Diocese,
                _ => ExternalContactType.Other
            };
        }

        public static string GetRoleByContactType(ExternalContactType contactType)
        {
            if (contactType == ExternalContactType.IncomingTrust || contactType == ExternalContactType.OutgoingTrust)
            {
                return "CEO";
            }

            return contactType.ToDisplayDescription();
        }
    }
}
