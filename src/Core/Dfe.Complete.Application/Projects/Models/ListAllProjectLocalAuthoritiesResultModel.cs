using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Projects.Models;

public record ListAllProjectLocalAuthoritiesResultModel(string LocalAuthorityName, string LocalAuthorityCode, int Conversions, int Transfers);