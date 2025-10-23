using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.LocalAuthorities.Models;

public record CreateLocalAuthorityDto(
    LocalAuthorityId LocalAuthorityId, 
    ContactId? ContactId); 
