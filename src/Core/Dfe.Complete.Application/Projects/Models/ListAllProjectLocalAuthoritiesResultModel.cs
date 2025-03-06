using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Projects.Models;

public record ListAllProjectLocalAuthoritiesResultModel(LocalAuthority LocalAuthority, string LocalAuthorityCode, int Conversions, int Transfers);