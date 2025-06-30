using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Pages.Projects.ExternalContacts;

public record ExternalContactModel(Contact Contact, bool IsEditable, bool? IsProjectMainContact = null, bool? IsOrganisationPrimaryContact = null);