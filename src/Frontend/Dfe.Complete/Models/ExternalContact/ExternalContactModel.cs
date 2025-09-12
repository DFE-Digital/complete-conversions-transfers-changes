using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Models.ExternalContact;

public record ExternalContactModel(
    Contact Contact,
    bool IsEditable,
    bool? IsProjectMainContact = null,
    bool? IsOrganisationPrimaryContact = null,
    bool? ShowOrganisation = false);