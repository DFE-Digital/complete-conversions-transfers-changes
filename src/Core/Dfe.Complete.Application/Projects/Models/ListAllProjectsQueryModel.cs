using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Projects.Model;

public record ListAllProjectsQueryModel(Project? Project, GiasEstablishment? Establishment);