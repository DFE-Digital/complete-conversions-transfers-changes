using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Projects.Models;

public record ListMatResultModel(string Identifier, string TrustName, IEnumerable<ListAllProjectsResultModel> ProjectModels);