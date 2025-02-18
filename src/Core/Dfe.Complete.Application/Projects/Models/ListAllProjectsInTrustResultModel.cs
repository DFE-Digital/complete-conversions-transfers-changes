using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Model;

public record ListAllProjectsInTrustResultModel(
    string trustName, IEnumerable<ListAllProjectsResultModel> projects);