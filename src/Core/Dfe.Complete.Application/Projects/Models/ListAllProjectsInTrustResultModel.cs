using Dfe.Complete.Application.Projects.Models;

namespace Dfe.Complete.Application.Projects.Model;

public record ListAllProjectsInTrustResultModel(
    string TrustName, IEnumerable<ListAllProjectsResultModel> Projects);