using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Projects.Models;

public record ListMatResultModel(string identifier, string trustName, IEnumerable<ListAllProjectsQueryModel> projectModels);