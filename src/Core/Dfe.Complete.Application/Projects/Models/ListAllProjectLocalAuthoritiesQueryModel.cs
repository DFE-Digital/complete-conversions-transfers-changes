using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Projects.Models;

public record ListAllProjectLocalAuthoritiesQueryModel(LocalAuthority LocalAuthority, Project Project);